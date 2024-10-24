using System.Linq.Expressions;

using FluentValidation;

namespace OptionalValues.FluentValidation;

/// <summary>
/// Extensions for FluentValidation to support <see cref="OptionalValue{T}"/>.
/// </summary>
public static class OptionalRuleExtensions
{
    /// <summary>
    /// Adds a rule to the validator for the <see cref="OptionalValue{T}"/> property.
    /// The rule is only applied when the value is specified.
    /// </summary>
    /// <param name="validator">The validator instance</param>
    /// <param name="propertySelector">Simple property accessor expression</param>
    /// <param name="configure">Rule Builder to specify the validation rules</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the property selector is not a simple property accessor.
    /// </exception>
    public static void OptionalRuleFor<T, TProperty>(
        this AbstractValidator<T> validator,
        Expression<Func<T, OptionalValue<TProperty?>>> propertySelector,
        Func<IRuleBuilderInitial<T, TProperty>, IRuleBuilderOptions<T, TProperty>> configure)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(propertySelector);
        ArgumentNullException.ThrowIfNull(configure);

        if (propertySelector.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException("Invalid property selector");
        }

        // Create a more deep property selector which selects the .Value property of the Defined<TProperty?>
        // For example if the propertySelector is x => x.Property then the deepPropertySelector will be x => x.Property.Value
        var deepPropertySelector = Expression.Lambda<Func<T, TProperty>>(
            Expression.Property(memberExpression, nameof(OptionalValue<object>.Value)),
            propertySelector.Parameters);

        // Create rules but apply them only when the value is defined
        validator.When(x => propertySelector.Compile()(x).IsSpecified, () =>
        {
            var propertySelectorObject = Expression.Lambda<Func<T, object>>(Expression.Convert(memberExpression, typeof(object)), propertySelector.Parameters);

            configure(validator.RuleFor(deepPropertySelector))
                .OverridePropertyName(propertySelectorObject);
        });
    }
}