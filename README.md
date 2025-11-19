# OptionalValues

A .NET library that provides an `OptionalValue<T>` type, representing a value that may or may not be specified, with comprehensive support for JSON serialization. e.g. (`undefined`, `null`, `"value"`)

[![NuGet](https://img.shields.io/nuget/v/OptionalValues.svg)](https://www.nuget.org/packages/OptionalValues)
[![License](https://img.shields.io/github/license/desjoerd/OptionalValues)](https://github.com/desjoerd/OptionalValues/blob/main/LICENSE)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/desjoerd/OptionalValues/.github%2Fworkflows%2Fci.yml)


| Package                                                                                           | Version                                                                                                                                        |
| ------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| [OptionalValues](https://www.nuget.org/packages/OptionalValues)                                   | [![NuGet](https://img.shields.io/nuget/v/OptionalValues.svg)](https://www.nuget.org/packages/OptionalValues)                                   |
| [OptionalValues.Swashbuckle](https://www.nuget.org/packages/OptionalValues.Swashbuckle)           | [![NuGet](https://img.shields.io/nuget/v/OptionalValues.Swashbuckle.svg)](https://www.nuget.org/packages/OptionalValues.Swashbuckle)           |
| [OptionalValues.NSwag](https://www.nuget.org/packages/OptionalValues.NSwag)                       | [![NuGet](https://img.shields.io/nuget/v/OptionalValues.NSwag.svg)](https://www.nuget.org/packages/OptionalValues.NSwag)                       |
| [OptionalValues.DataAnnotations](https://www.nuget.org/packages/OptionalValues.DataAnnotations)   | [![NuGet](https://img.shields.io/nuget/v/OptionalValues.DataAnnotations.svg)](https://www.nuget.org/packages/OptionalValues.DataAnnotations)   |
| [OptionalValues.FluentValidation](https://www.nuget.org/packages/OptionalValues.FluentValidation) | [![NuGet](https://img.shields.io/nuget/v/OptionalValues.FluentValidation.svg)](https://www.nuget.org/packages/OptionalValues.FluentValidation) |

## Overview

The `OptionalValue<T>` struct is designed to represent a value that can be in one of three states:

- **Unspecified**: The value has not been specified. (e.g. `undefined`)
- **Specified with a non-null value**: The value has been specified and is `not null`.
- **Specified with a `null` value**: The value has been specified and is `null`.

### Why
When working with Json it's currently difficult to know whether a property was omitted or explicitly `null`. This makes it hard to support older clients that don't send all properties in a request. By using `OptionalValue<T>` you can distinguish between `null` and `Unspecified` values.

```csharp
using System.Text.Json;
using OptionalValues;

var jsonSerializerOptions = new JsonSerializerOptions()
    .AddOptionalValueSupport();

var json =
    """
    {
      "FirstName": "John",
      "LastName": null
    }
    """;

var person1 = JsonSerializer.Deserialize<Person>(json, jsonSerializerOptions);

// equals:
var person2 = new Person
{
    FirstName = "John",
    LastName = null,
    Address = OptionalValue<string>.Unspecified // or default
};

bool areEqual = person1 == person2; // True

string serialized = JsonSerializer.Serialize(person2, jsonSerializerOptions);
// Output: {"FirstName":"John","LastName":null}

public record Person
{
    public OptionalValue<string> FirstName { get; set; }
    public OptionalValue<string?> LastName { get; set; }
    public OptionalValue<string> Address { get; set; }
}
```

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues
```

For JSON serialization support, configure the `JsonSerializerOptions` to include the `OptionalValue<T>` converter:

```csharp
var options = new JsonSerializerOptions()
    .AddOptionalValueSupport();
```

Optionally, install one or more extension packages:

```bash
dotnet add package OptionalValues.Swashbuckle
dotnet add package OptionalValues.NSwag
dotnet add package OptionalValues.DataAnnotations
dotnet add package OptionalValues.FluentValidation
```

## Features

- **Distinguish Between Unspecified and Null Values**: Clearly differentiate when a value is intentionally `null` versus when it has not been specified at all. This allows for mapping `undefined` values in JSON to `Unspecified` values in C#.
- **JSON Serialization Support**: Includes a custom JSON converter and TypeResolverModifier that correctly handles serialization and deserialization, ensuring unspecified values are omitted from JSON outputs.
- **Dictionary Extensions**: Extension methods for working with dictionaries and `OptionalValue<T>`, including `GetOptionalValue`, `AddOptionalValue`, `TryAddOptionalValue`, and `SetOptionalValue`.
- **Optional DataAnnotations**: An extension library that provides support for DataAnnotations validation attributes on `OptionalValue<T>` properties.
- **FluentValidation Extensions**: Provides extension methods to simplify the validation of `OptionalValue<T>` properties using FluentValidation.
- **OpenApi/Swagger Support**: 
  - **Swashbuckle** Includes a custom data contract resolver for Swashbuckle to generate accurate OpenAPI/Swagger documentation.
  - **NSwag**: Support for NSwag is available through the `OptionalValues.NSwag` package. It includes an `OptionalValueTypeMapper` to map the `OptionalValue<T>` to it's underlying type `T` in the generated OpenAPI schema.
- **Patch Operation Support**: Ideal for API patch operations where fields can be updated to `null` or remain unchanged.

# Table of Contents

- [OptionalValues](#optionalvalues)
  - [Overview](#overview)
    - [Why](#why)
  - [Installation](#installation)
  - [Features](#features)
- [Table of Contents](#table-of-contents)
- [Usage](#usage)
  - [Creating an OptionalValue](#creating-an-optionalvalue)
  - [Checking If a Value Is Specified](#checking-if-a-value-is-specified)
  - [Accessing the Value](#accessing-the-value)
  - [Implicit Conversions](#implicit-conversions)
  - [Equality Comparisons](#equality-comparisons)
  - [Dictionary Extensions](#dictionary-extensions)
  - [JSON Serialization with System.Text.Json](#json-serialization-with-systemtextjson)
    - [Serialization Behavior](#serialization-behavior)
    - [Deserialization Behavior](#deserialization-behavior)
    - [Respect nullable annotations](#respect-nullable-annotations)
- [Library support](#library-support)
  - [ASP.NET Core](#aspnet-core)
  - [Swashbuckle](#swashbuckle)
    - [Installation](#installation-1)
  - [NSwag](#nswag)
    - [Installation](#installation-2)
  - [System.ComponentModel.DataAnnotations](#systemcomponentmodeldataannotations)
  - [FluentValidation](#fluentvalidation)
    - [Installation](#installation-3)
    - [Using OptionalRuleFor](#using-optionalrulefor)
    - [How It Works](#how-it-works)
    - [Example Usage](#example-usage)
- [Use Cases](#use-cases)
  - [API Patch Operations](#api-patch-operations)
- [Current Limitations](#current-limitations)
- [Contributing](#contributing)
- [License](#license)
- [Benchmarks](#benchmarks)


# Usage

## Creating an OptionalValue

You can create an `OptionalValue<T>` in several ways:

- **Unspecified Value**:

  ```csharp
  var unspecified = new OptionalValue<string>();
  // or
  var unspecified = OptionalValue<string>.Unspecified;
  // or
  OptionalValue<string> unspecified = default;
  ```

- **Specified Value**:

  ```csharp
  var specifiedValue = new OptionalValue<string>("Hello, World!");
  // or using implicit conversion
  OptionalValue<string> specifiedValue = "Hello, World!";
  ```

- **Specified Null Value**:

  ```csharp
  var specifiedNull = new OptionalValue<string?>(null);
  // or using implicit conversion
  OptionalValue<string?> specifiedNull = null;
  ```

## Checking If a Value Is Specified

Use the `IsSpecified` property to determine if the value has been specified:

```csharp
if (optionalValue.IsSpecified)
{
    Console.WriteLine("Value is specified.");
}
else
{
    Console.WriteLine("Value is unspecified.");
}
```

## Accessing the Value

- `.Value`: Gets the value if specified; returns `null` if unspecified.
- `.SpecifiedValue`: Gets the specified value; throws `InvalidOperationException` if the value is unspecified.
- `.GetSpecifiedValueOrDefault()`: Gets the specified value or the default value of `T` if unspecified.
- `.GetSpecifiedValueOrDefault(T defaultValue)`: Gets the specified value or the provided default value if unspecified.

```csharp
var optionalValue = new OptionalValue<string>("Example");

// Using Value
string? value = optionalValue.Value;

// Using SpecifiedValue
string specifiedValue = optionalValue.SpecifiedValue;

// Using GetSpecifiedValueOrDefault
string valueOrDefault = optionalValue.GetSpecifiedValueOrDefault("Default Value");
```

## Implicit Conversions

`OptionalValue<T>` supports implicit conversions to and from `T`:

```csharp
// From T to OptionalValue<T>
OptionalValue<int> optionalInt = 42;

// From OptionalValue<T> to T (returns null if unspecified)
int? value = optionalInt;
```

## Equality Comparisons

Equality checks consider both the `IsSpecified` property and the `Value`:

```csharp
var value1 = new OptionalValue<string>("Test");
var value2 = new OptionalValue<string>("Test");
var unspecified = new OptionalValue<string>();

bool areEqual = value1 == value2; // True
bool areUnspecifiedEqual = unspecified == new OptionalValue<string>(); // True
```

## Dictionary Extensions

The library includes extension methods for working with dictionaries and `OptionalValue<T>`, making it easy to retrieve, add, and update values optionally.

### GetOptionalValue

Retrieves a value from a dictionary as an `OptionalValue<T>`. If the key exists, returns a specified value; otherwise, returns `OptionalValue<T>.Unspecified`.

```csharp
using OptionalValues.Extensions;

var settings = new Dictionary<string, int>
{
    ["timeout"] = 30
};

OptionalValue<int> timeout = settings.GetOptionalValue("timeout");
// timeout.IsSpecified == true, timeout.SpecifiedValue == 30

OptionalValue<int> retries = settings.GetOptionalValue("retries");
// retries.IsSpecified == false
```

### AddOptionalValue

Adds a key-value pair to the dictionary only if the value is specified. Throws an exception if the key already exists.

```csharp
var settings = new Dictionary<string, int>();

settings.AddOptionalValue("timeout", new OptionalValue<int>(30));
// Adds timeout with value 30

settings.AddOptionalValue("retries", OptionalValue<int>.Unspecified);
// Does nothing - unspecified values are not added
```

### TryAddOptionalValue

Attempts to add a key-value pair to the dictionary if the value is specified. Returns `true` if the value was added, `false` otherwise.

```csharp
var settings = new Dictionary<string, int>();

bool added1 = settings.TryAddOptionalValue("timeout", new OptionalValue<int>(30));
// Returns true, timeout is added

bool added2 = settings.TryAddOptionalValue("retries", OptionalValue<int>.Unspecified);
// Returns false, unspecified value is not added

bool added3 = settings.TryAddOptionalValue("timeout", new OptionalValue<int>(60));
// Returns false, key already exists
```

### SetOptionalValue

Sets or updates a value in the dictionary only if the value is specified. If the value is unspecified, the dictionary remains unchanged.

```csharp
var settings = new Dictionary<string, int>
{
    ["timeout"] = 30
};

settings.SetOptionalValue("timeout", new OptionalValue<int>(60));
// Updates timeout to 60

settings.SetOptionalValue("timeout", OptionalValue<int>.Unspecified);
// Does nothing - timeout remains 60

settings.SetOptionalValue("retries", new OptionalValue<int>(3));
// Adds retries with value 3 (key didn't exist)
```

## JSON Serialization with System.Text.Json

`OptionalValue<T>` includes a custom JSON converter and JsonTypeInfoResolver Modifier to handle serialization and deserialization of optional values.
To properly serialize `OptionalValue<T>` properties, add it to the `JsonSerializerOptions`:

```csharp
var newOptionsWithSupport = JsonSerializerOptions.Default
    .WithOptionalValueSupport();

// or
var options = new JsonSerializerOptions();
options.AddOptionalValueSupport();
```

### Serialization Behavior

- **Unspecified Values**: Omitted from the JSON output.
- **Specified Null Values**: Serialized with a `null` value.
- **Specified Non-Null Values**: Serialized with the actual value.

```csharp
public class Person
{
    public OptionalValue<string> FirstName { get; set; }

    public OptionalValue<string> LastName { get; set; }
}

// Creating a Person instance
var person = new Person
{
    FirstName = "John", // Specified non-null value
    LastName = new OptionalValue<string>() // Unspecified
};

// Serializing to JSON
string json = JsonSerializer.Serialize(person);
// Output: {"FirstName":"John"}
```

### Deserialization Behavior

- **Missing Properties**: Deserialized as unspecified values.
- **Properties with `null`**: Deserialized as specified with a `null` value.
- **Properties with Values**: Deserialized as specified with the given value.

```csharp
string jsonInput = @"{""FirstName"":""John"",""LastName"":null}";
var person = JsonSerializer.Deserialize<Person>(jsonInput);

bool isFirstNameSpecified = person.FirstName.IsSpecified; // True
string firstName = person.FirstName.SpecifiedValue; // "John"

bool isLastNameSpecified = person.LastName.IsSpecified; // True
string lastName = person.LastName.SpecifiedValue; // null
```

### Respect nullable annotations

`OptionalValue<T>` has support for respecting nullable annotations when enabling `RespectNullableAnnotations = true` in the `JsonSerializerOptions`. When enabled, when deserializing a `null` value on an `OptionalValue` which is NOT nullable, it will throw a `JsonException` with a message indicating that the value is not nullable.

```csharp
JsonSerializerOptions Options = new JsonSerializerOptions
{
    RespectNullableAnnotations = true,
}.AddOptionalValueSupport();

var json = """
           {
               "NotNullable": null
           }
           """;

var model = JsonSerializer.Deserialize<Model>(json, Options); // Throws JsonException

private class Model
{
    public OptionalValue<string> NotNullable { get; init; }
}
```

There are a few limitations to this feature:
- It only works when NOT using generics.

```csharp
// it does not work with this, because the type is generic and we cannot determine if it is nullable or not as this information is not available at runtime.
public class Model<T>
{
    public OptionalValue<T> NotNullable { get; init; }
}
```

# Library support

## ASP.NET Core

The `OptionalValues` library integrates seamlessly with ASP.NET Core, allowing you to use `OptionalValue<T>` properties in your API models.

You only need to configure the `JsonSerializerOptions` to include the `OptionalValue<T>` converter:

```csharp
// For Minimal API
builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    // Make sure that AddOptionalValueSupport() is the last call when you are using the `TypeInfoResolverChain` of the `SerializerOptions`.
    jsonOptions.SerializerOptions.AddOptionalValueSupport();
});

// For MVC
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AddOptionalValueSupport();
    });
```

## Swashbuckle

The `OptionalValues.Swashbuckle` package provides a custom data contract resolver for Swashbuckle to generate accurate OpenAPI/Swagger documentation for `OptionalValue<T>` properties.

It correctly unwraps the `OptionalValue<T>` type and generates the appropriate schema for the underlying type `T`.

### Installation

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues.Swashbuckle
```

Configure the Swashbuckle services to use the `OptionalValueDataContractResolver`:

```csharp
builder.Services.AddSwaggerGen();
// after AddSwaggerGen when you want it to use an existing custom ISerializerDataContractResolver.
builder.Services.AddSwaggerGenOptionalValueSupport();
```

## NSwag

The `OptionalValues.NSwag` package provides an `OptionalValueTypeMapper` to map the `OptionalValue<T>` to its underlying type `T` in the generated OpenAPI schema.

### Installation

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues.NSwag
```

Configure the NSwag SchemaSettings to use the `OptionalValueTypeMapper`:

```csharp
builder.Services.AddOpenApiDocument(options =>
{
    // Add OptionalValue support to NSwag
    options.SchemaSettings.AddOptionalValueSupport();
});
```

## System.ComponentModel.DataAnnotations

The `OptionalValues.DataAnnotations` package provides DataAnnotations validation attributes for `OptionalValue<T>` properties. They are all overrides of the standard DataAnnotations attributes and prefixed with `Optional`. The key difference is that the validation rules are only applied when the value is `specified` (which is close to the default behavior which only applies it when it's not null).

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues.DataAnnotations
```

**Presence Validators:**

- `[Specified]`: Ensures the `OptionalValue<T>` is specified (present), but allows `null` or empty values.
- `[RequiredValue]`: Ensures the `OptionalValue<T>` is specified and its value is not `null` or empty. This should be used instead of the standard `[Required]` attribute.

Example usage:
```csharp
public class ExampleModel
{
    [OptionalAllowedValues("a")]
    public OptionalValue<string> AllowedValues { get; set; }

    [OptionalDeniedValues("a")]
    public OptionalValue<string> DeniedValues { get; set; }

    [OptionalLength(1, 5)]
    public OptionalValue<int[]> LengthCollection { get; set; }

    [OptionalLength(1, 5)]
    public OptionalValue<string> LengthString { get; set; }

    [OptionalMaxLength(5)]
    public OptionalValue<int[]> MaxLengthCollection { get; set; }

    [OptionalMaxLength(5)]
    public OptionalValue<string> MaxLengthString { get; set; }

    [OptionalMinLength(1)]
    public OptionalValue<int[]> MinLengthCollection { get; set; }

    [OptionalMinLength(1)]
    public OptionalValue<string> MinLengthString { get; set; }

    [OptionalRange(5, 42)]
    public OptionalValue<int> Range { get; set; }

    [OptionalRegularExpression("^something$")]
    public OptionalValue<string> RegularExpression { get; set; }

    [Specified]
    public OptionalValue<string?> Specified { get; set; }

    [RequiredValue]
    public OptionalValue<string> SpecifiedRequired { get; set; }

    [OptionalStringLength(5)]
    public OptionalValue<string> StringLength { get; set; }
}
```

## FluentValidation

The `OptionalValues.FluentValidation` package provides extension methods to simplify the validation of `OptionalValue<T>` properties using FluentValidation.

### Installation

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues.FluentValidation
```

### Using OptionalRuleFor

The `OptionalRuleFor` extension method allows you to define validation rules for `OptionalValue<T>` properties that are only applied when the value is specified.

```csharp
using FluentValidation;
using OptionalValues.FluentValidation;

public class UpdateUserRequest
{
    public OptionalValue<string?> Email { get; set; }
    public OptionalValue<int> Age { get; set; }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        this.OptionalRuleFor(x => x.Email, x => x
            .NotEmpty()
            .EmailAddress());

        this.OptionalRuleFor(x => x.Age, x => x
            .GreaterThan(18));
    }
}
```

In this example:

- The validation rules for `Email` and `Age` are applied only if the corresponding `OptionalValue<T>` is specified.
- If the value is unspecified, the validation rules are skipped.

### How It Works

The `OptionalRuleFor` method:

- Takes an expression specifying the `OptionalValue<T>` property.
- Accepts a configuration function where you define your validation rules using the standard FluentValidation syntax.
- Internally, it checks if the value is specified (`IsSpecified`) before applying the validation rules.

### Example Usage

```csharp
var validator = new UpdateUserRequestValidator();

// Valid request with specified values
var validRequest = new UpdateUserRequest
{
    Email = "user@example.com",
    Age = 25
};

var result = validator.Validate(validRequest);
// result.IsValid == true

// Invalid request with specified values
var invalidRequest = new UpdateUserRequest
{
    Email = "invalid-email",
    Age = 17
};

var resultInvalid = validator.Validate(invalidRequest);
// resultInvalid.IsValid == false
// Errors for Email and Age

// Request with unspecified values
var unspecifiedRequest = new UpdateUserRequest
{
    Email = default,
    Age = default
};

var resultUnspecified = validator.Validate(unspecifiedRequest);
// resultUnspecified.IsValid == true
// Validation rules are skipped for unspecified values
```

# Use Cases

## API Patch Operations

When updating resources via API endpoints, it's crucial to distinguish between fields that should be updated to `null` and fields that should remain unchanged.

```csharp
public class UpdateUserRequest
{
    public OptionalValue<string?> Email { get; set; }

    public OptionalValue<string?> PhoneNumber { get; set; }
}

[HttpPatch("{id}")]
public IActionResult UpdateUser(int id, UpdateUserRequest request)
{
    if (request.Email.IsSpecified)
    {
        // Update email to request.Email.SpecifiedValue
    }

    if (request.PhoneNumber.IsSpecified)
    {
        // Update phone number to request.PhoneNumber.SpecifiedValue
    }

    // Unspecified fields remain unchanged

    return Ok();
}
```

# Current Limitations

- **DataAnnotations**: The `OptionalValue<T>` type does not support DataAnnotations validation attributes because they are tied to specific .NET Types (e.g. string).
  - **"Workaround"**: Use the FluentValidation extensions to define validation rules for `OptionalValue<T>` properties.
- **Support for other libraries**: Because `OptionalValue<T>` is a wrapper type it requires mapping to the underlying type for some libraries. Let me know if you have a specific library in mind that you would like to see support for.

# Contributing

Contributions are welcome! Please feel free to submit issues or pull requests on the [GitHub repository](https://github.com/desjoerd/OptionalValues).

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

# Benchmarks

The project is benchmarked with [BenchmarkDotNet](https://benchmarkdotnet.org/) to check any additional overhead that the `OptionalValue<T>` type might introduce. They are located in the `/test/OptionalValues.Benchmarks` directory.

Below are the results of the benchmarks for the `OptionalValue<T>` serialization performance on my machine:

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2605)
13th Gen Intel Core i9-13900H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method                                         |      Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
| ---------------------------------------------- | --------: | -------: | -------: | ----: | ------: | -----: | --------: | ----------: |
| SerializePrimitiveModel                        | 102.10 ns | 1.296 ns | 1.212 ns |  1.00 |    0.02 | 0.0088 |     112 B |        1.00 |
| SerializeOptionalValueModel                    | 108.55 ns | 1.324 ns | 1.238 ns |  1.06 |    0.02 | 0.0134 |     168 B |        1.50 |
| SerializePrimitiveModelWithSourceGenerator     |  75.65 ns | 1.554 ns | 1.727 ns |  0.74 |    0.02 | 0.0088 |     112 B |        1.00 |
| SerializeOptionalValueModelWithSourceGenerator |  93.47 ns | 1.690 ns | 1.581 ns |  0.92 |    0.02 | 0.0134 |     168 B |        1.50 |

*1ns = 1/1,000,000,000 seconds*

It is comparing the serialization performance between these two models:
```csharp
public class PrimitiveModel
{
    public int Age { get; set; } = 42;
    public string FirstName { get; set; } = "John";
    public string? LastName { get; set; } = null;
}

public class OptionalValueModel
{
    public OptionalValue<int> Age { get; set; } = 42;
    public OptionalValue<string> FirstName { get; set; } = "John";
    public OptionalValue<string> LastName { get; set; } = default;
}
```