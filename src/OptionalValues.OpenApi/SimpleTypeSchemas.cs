using System.Diagnostics.CodeAnalysis;

using Microsoft.OpenApi.Models;

namespace OptionalValues.OpenApi;

internal static class SimpleTypeSchemas
{
    private static readonly Dictionary<Type, OpenApiSchema> SimpleTypeToOpenApiSchema = new()
    {
        [typeof(bool)] = new()
        {
            Type = "boolean"
        },
        [typeof(byte)] = new()
        {
            Type = "integer", Format = "uint8"
        },
        [typeof(byte[])] = new()
        {
            Type = "string", Format = "byte"
        },
        [typeof(int)] = new()
        {
            Type = "integer", Format = "int32"
        },
        [typeof(uint)] = new()
        {
            Type = "integer", Format = "uint32"
        },
        [typeof(long)] = new()
        {
            Type = "integer", Format = "int64"
        },
        [typeof(ulong)] = new()
        {
            Type = "integer", Format = "uint64"
        },
        [typeof(short)] = new()
        {
            Type = "integer", Format = "int16"
        },
        [typeof(ushort)] = new()
        {
            Type = "integer", Format = "uint16"
        },
        [typeof(float)] = new()
        {
            Type = "number", Format = "float"
        },
        [typeof(double)] = new()
        {
            Type = "number", Format = "double"
        },
        [typeof(decimal)] = new()
        {
            Type = "number", Format = "double"
        },
        [typeof(DateTime)] = new()
        {
            Type = "string", Format = "date-time"
        },
        [typeof(DateTimeOffset)] = new()
        {
            Type = "string", Format = "date-time"
        },
        [typeof(Guid)] = new()
        {
            Type = "string", Format = "uuid"
        },
        [typeof(char)] = new()
        {
            Type = "string", Format = "char"
        },
        [typeof(Uri)] = new()
        {
            Type = "string", Format = "uri"
        },
        [typeof(string)] = new()
        {
            Type = "string"
        },
        [typeof(TimeOnly)] = new()
        {
            Type = "string", Format = "time"
        },
        [typeof(DateOnly)] = new()
        {
            Type = "string", Format = "date"
        },
    };

    internal static bool TryGetSimpleTypeSchema(Type type, [NotNullWhen(true)] out OpenApiSchema? schema)
        => SimpleTypeToOpenApiSchema.TryGetValue(type, out schema);
}