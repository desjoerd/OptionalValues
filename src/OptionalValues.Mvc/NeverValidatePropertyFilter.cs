using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OptionalValues.Mvc;

internal sealed class NeverValidatePropertyFilter : IPropertyValidationFilter
{
    internal static NeverValidatePropertyFilter Instance { get; } = new();

    public bool ShouldValidateEntry(ValidationEntry entry, ValidationEntry parentEntry) => false;
}
