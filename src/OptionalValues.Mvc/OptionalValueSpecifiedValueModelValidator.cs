using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OptionalValues.Mvc;

internal sealed class OptionalValueSpecifiedValueModelValidator : IModelValidator
{
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

        var isSpecifiedProperty = modelType.GetProperty(nameof(OptionalValue<object>.IsSpecified));
        if (isSpecifiedProperty?.GetValue(model) is not true)
        {
            return [];
        }

        var value = modelType.GetProperty(nameof(OptionalValue<object>.Value))?.GetValue(model);
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
}
