using System.ComponentModel.DataAnnotations;
using System.Globalization;

using Microsoft.OpenApi.Models;

namespace OptionalValues.OpenApi;

internal static class OpenApiSchemaExtensions
{
    internal static void ApplyValidationAttributes(this OpenApiSchema schema, IEnumerable<Attribute> validationAttributes)
    {
        foreach (Attribute attribute in validationAttributes)
        {
            if (attribute is Base64StringAttribute)
            {
                schema.Type = "string";
                schema.Format = "byte";
            }
            else if (attribute is RangeAttribute rangeAttribute)
            {
                // Use InvariantCulture if explicitly requested or if the range has been set via the
                // RangeAttribute(double, double) or RangeAttribute(int, int) constructors.
                CultureInfo targetCulture = rangeAttribute.ParseLimitsInInvariantCulture || rangeAttribute.Minimum is double || rangeAttribute.Maximum is int
                    ? CultureInfo.InvariantCulture
                    : CultureInfo.CurrentCulture;

                var minString = rangeAttribute.Minimum.ToString();
                var maxString = rangeAttribute.Maximum.ToString();

                if (decimal.TryParse(minString, NumberStyles.Any, targetCulture, out var minDecimal))
                {
                    schema.Minimum = minDecimal;
                }

                if (decimal.TryParse(maxString, NumberStyles.Any, targetCulture, out var maxDecimal))
                {
                    schema.Maximum = maxDecimal;
                }
            }
            else if (attribute is RegularExpressionAttribute regularExpressionAttribute)
            {
                schema.Pattern = regularExpressionAttribute.Pattern;
            }
            else if (attribute is MaxLengthAttribute maxLengthAttribute)
            {
                if (schema.Type == "array")
                {
                    schema.MaxItems = maxLengthAttribute.Length;
                }
                else
                {
                    schema.MaxLength = maxLengthAttribute.Length;
                }
            }
            else if (attribute is MinLengthAttribute minLengthAttribute)
            {
                if (schema.Type == "array")
                {
                    schema.MinItems = minLengthAttribute.Length;
                }
                else
                {
                    schema.MinLength = minLengthAttribute.Length;
                }
            }
            else if (attribute is LengthAttribute lengthAttribute)
            {
                if (schema.Type == "array")
                {
                    schema.MinItems = lengthAttribute.MinimumLength;
                    schema.MaxItems = lengthAttribute.MaximumLength;
                }
                else
                {
                    schema.MinLength = lengthAttribute.MinimumLength;
                    schema.MaxLength = lengthAttribute.MaximumLength;
                }
            }
            else if (attribute is UrlAttribute)
            {
                schema.Type = "string";
                schema.Format = "uri";
            }
            else if (attribute is StringLengthAttribute stringLengthAttribute)
            {
                schema.MinLength = stringLengthAttribute.MinimumLength;
                schema.MaxLength = stringLengthAttribute.MaximumLength;
            }
        }
    }
}