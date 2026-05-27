using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OptionalValues.Mvc;

internal sealed class OptionalValueSpecifiedValueModelValidator : IModelValidator
{
    private static readonly ConcurrentDictionary<Type, Accessors> AccessorCache = new();

    internal static OptionalValueSpecifiedValueModelValidator Instance { get; } = new();

    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var model = context.Model;
        if (model is null)
        {
            return [];
        }

        Type modelType = model.GetType();
        if (!OptionalValue.IsOptionalValueType(modelType))
        {
            return [];
        }

        Accessors accessors = AccessorCache.GetOrAdd(modelType, static type => new(
            type.GetProperty(nameof(OptionalValue<object>.IsSpecified)) ?? throw new InvalidOperationException(),
            type.GetProperty(nameof(OptionalValue<object>.Value)) ?? throw new InvalidOperationException()));

        if (accessors.IsSpecifiedProperty.GetValue(model) is not true)
        {
            return [];
        }

        var value = accessors.ValueProperty.GetValue(model);
        if (value is null)
        {
            return [];
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(
            value,
            context.ActionContext.HttpContext.RequestServices,
            items: null);

        Validator.TryValidateObject(value, validationContext, validationResults, validateAllProperties: true);

        return validationResults.SelectMany(static result =>
        {
            var memberNames = result.MemberNames.DefaultIfEmpty(string.Empty);
            return memberNames.Select(memberName => new ModelValidationResult(memberName, result.ErrorMessage ?? string.Empty));
        });
    }

    private readonly record struct Accessors(PropertyInfo IsSpecifiedProperty, PropertyInfo ValueProperty);
}
