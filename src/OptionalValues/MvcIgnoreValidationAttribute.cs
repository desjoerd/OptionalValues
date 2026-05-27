using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OptionalValues;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class MvcIgnoreValidationAttribute : Attribute, IPropertyValidationFilter
{
    public bool ShouldValidateEntry(ValidationEntry entry, ValidationEntry parentEntry) => false;
}
