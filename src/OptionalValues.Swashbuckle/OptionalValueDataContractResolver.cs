using System.Reflection;
using System.Text.Json;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace OptionalValues.Swashbuckle;

/// <summary>
/// A <see cref="ISerializerDataContractResolver"/> that adds support for <see cref="OptionalValue{T}"/>.
/// It will unwrap the <see cref="OptionalValue{T}"/> and use the underlying type for resolving the data contract.
/// </summary>
public class OptionalValueDataContractResolver : ISerializerDataContractResolver
{
    private readonly ISerializerDataContractResolver _inner;

    /// <summary>
    /// Creates a new instance of <see cref="OptionalValueDataContractResolver"/> by using the JsonSerializerOptions.
    /// It will wrap a new <see cref="JsonSerializerDataContractResolver"/> with the provided options.
    /// </summary>
    /// <param name="serializerOptions">The <see cref="JsonSerializerOptions"/> which will be used to create the inner <see cref="JsonSerializerDataContractResolver"/>.</param>
    public OptionalValueDataContractResolver(JsonSerializerOptions serializerOptions)
        : this(new JsonSerializerDataContractResolver(serializerOptions))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="OptionalValueDataContractResolver"/> by wrapping the provided <see cref="ISerializerDataContractResolver"/>.
    /// </summary>
    /// <param name="inner">The inner <see cref="ISerializerDataContractResolver"/> to use for resolving the data contracts.</param>
    public OptionalValueDataContractResolver(ISerializerDataContractResolver inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
    }

    /// <inheritdoc />
    public virtual DataContract GetDataContractForType(Type type)
    {
        Type effectiveType = type;
        if (OptionalValue.IsOptionalValueType(type))
        {
            effectiveType = OptionalValue.GetUnderlyingType(type);
        }

        DataContract? dataContract = _inner.GetDataContractForType(effectiveType);
        if (dataContract is not
            { DataType: DataType.Object })
        {
            return dataContract;
        }

        var effectiveProperties = new List<DataProperty>();
        foreach (DataProperty property in dataContract.ObjectProperties)
        {
            DataProperty effectiveProperty = property;
            if (OptionalValue.IsOptionalValueType(property.MemberType))
            {
                Type underLyingType = OptionalValue.GetUnderlyingType(property.MemberType);
                var isNullable = Nullable.GetUnderlyingType(underLyingType) != null
                    || GetOptionalValueIsNullable(property.MemberInfo);

                var isRequired = property.MemberInfo.GetCustomAttributes(true).Any(a => a.GetType().FullName == "OptionalValues.DataAnnotations.SpecifiedAttribute");
                effectiveProperty = new DataProperty(property.Name, property.MemberType, isRequired, isNullable, property.IsReadOnly, property.IsWriteOnly, property.MemberInfo);
            }

            effectiveProperties.Add(effectiveProperty);
        }

        dataContract = DataContract.ForObject(
            dataContract.UnderlyingType,
            effectiveProperties,
            dataContract.ObjectExtensionDataType,
            dataContract.ObjectTypeNameProperty,
            dataContract.ObjectTypeNameProperty,
            dataContract.JsonConverter);

        return dataContract;
    }

    private static bool GetOptionalValueIsNullable(ICustomAttributeProvider? memberInfo)
    {
        NullabilityInfo? nullabilityInfo = GetOptionalValueNullabilityInfo(memberInfo);
        if (nullabilityInfo == null)
        {
            return false;
        }

        return nullabilityInfo.ReadState == NullabilityState.Nullable;
    }

    private static NullabilityInfo? GetOptionalValueNullabilityInfo(ICustomAttributeProvider? memberInfo)
    {
        if (memberInfo == null)
        {
            return null;
        }

        NullabilityInfo? nullabilityInfo = GetNullabilityInfo(memberInfo);
        if (nullabilityInfo == null
            || !OptionalValue.IsOptionalValueType(nullabilityInfo.Type))
        {
            return null;
        }

        return nullabilityInfo.GenericTypeArguments[0];
    }

    private static NullabilityInfo? GetNullabilityInfo(ICustomAttributeProvider memberInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        return memberInfo switch
        {
            PropertyInfo propertyInfo => nullabilityInfoContext.Create(propertyInfo),
            FieldInfo fieldInfo => nullabilityInfoContext.Create(fieldInfo),
            ParameterInfo parameterInfo => nullabilityInfoContext.Create(parameterInfo),
            _ => null,
        };
    }
}