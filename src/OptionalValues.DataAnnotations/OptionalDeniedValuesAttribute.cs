using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalDeniedValuesAttribute : DeniedValuesAttribute
{
    /// <inheritdoc />
    public OptionalDeniedValuesAttribute(
        [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "In the base class")]
        params object[] deniedValues) : base(deniedValues)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}