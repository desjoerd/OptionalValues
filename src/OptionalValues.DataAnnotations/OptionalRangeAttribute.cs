using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using OptionalValues.DataAnnotations.Internal;

namespace OptionalValues.DataAnnotations;

/// <inheritdoc />
public class OptionalRangeAttribute : RangeAttribute
{
    /// <inheritdoc />
    public OptionalRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
    {
    }

    /// <inheritdoc />
    public OptionalRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
    {
    }

    /// <inheritdoc />
    public OptionalRangeAttribute(
        [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "In the base class")]
        Type type,
        string minimum,
        string maximum) : base(type, minimum, maximum)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(object? value)
        => OverrideHelper.OptionalIsValid(value, base.IsValid);
}