# OptionalValues

A .NET library that provides an `OptionalValue<T>` type, representing a value that may or may not be specified, with comprehensive support for JSON serialization.

## Overview

The `OptionalValue<T>` struct is designed to represent a value that can be in one of three states:

- **Unspecified**: The value has not been specified.
- **Specified with a non-null value**: The value has been specified and is not `null`.
- **Specified with a `null` value**: The value has been specified and is `null`.

This differs from `Nullable<T>`, which can only distinguish between the presence or absence of a value, and cannot differentiate between an unspecified value and a specified `null` value.

## Features

- **Distinguish Between Unspecified and Null Values**: Clearly differentiate when a value is intentionally `null` versus when it has not been specified at all.
- **JSON Serialization Support**: Includes a custom JSON converter that correctly handles serialization and deserialization, ensuring unspecified values are omitted from JSON outputs.
- **FluentValidation Extensions**: Provides extension methods to simplify the validation of `OptionalValue<T>` properties using FluentValidation.
- **Patch Operation Support**: Ideal for API patch operations where fields can be updated to `null` or remain unchanged.

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package PACKAGE_NAME
```

## Usage

### Creating an OptionalValue

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

### Checking If a Value Is Specified

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

### Accessing the Value

- **Value**: Gets the value if specified; returns `null` if unspecified.
- **SpecifiedValue**: Gets the specified value; throws `InvalidOperationException` if the value is unspecified.
- **GetSpecifiedValueOrDefault()**: Gets the specified value or the default value of `T` if unspecified.
- **GetSpecifiedValueOrDefault(T defaultValue)**: Gets the specified value or the provided default value if unspecified.

```csharp
var optionalValue = new OptionalValue<string>("Example");

// Using Value
string? value = optionalValue.Value;

// Using SpecifiedValue
string specifiedValue = optionalValue.SpecifiedValue;

// Using GetSpecifiedValueOrDefault
string valueOrDefault = optionalValue.GetSpecifiedValueOrDefault("Default Value");
```

### Implicit Conversions

`OptionalValue<T>` supports implicit conversions to and from `T`:

```csharp
// From T to OptionalValue<T>
OptionalValue<int> optionalInt = 42;

// From OptionalValue<T> to T (returns null if unspecified)
int? value = optionalInt;
```

### Equality Comparisons

Equality checks consider both the `IsSpecified` property and the `Value`:

```csharp
var value1 = new OptionalValue<string>("Test");
var value2 = new OptionalValue<string>("Test");
var unspecified = new OptionalValue<string>();

bool areEqual = value1 == value2; // True
bool areUnspecifiedEqual = unspecified == new OptionalValue<string>(); // True
```

### JSON Serialization with System.Text.Json

`OptionalValue<T>` includes a custom JSON converter to handle serialization and deserialization of optional values.
To properly serialize `OptionalValue<T>` properties, annotate them with `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]`.

#### Serialization Behavior

- **Unspecified Values**: Omitted from the JSON output.
- **Specified Null Values**: Serialized with a `null` value.
- **Specified Non-Null Values**: Serialized with the actual value.

```csharp
public class Person
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OptionalValue<string> FirstName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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

#### Deserialization Behavior

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

### FluentValidation Extensions

The `PACKAGE_NAME.FluentValidation` package provides extension methods to simplify the validation of `OptionalValue<T>` properties using FluentValidation.

#### Installation

Install the package using the .NET CLI:

```bash
dotnet add package OptionalValues.FluentValidation
```

#### Using OptionalRuleFor

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

#### How It Works

The `OptionalRuleFor` method:

- Takes an expression specifying the `OptionalValue<T>` property.
- Accepts a configuration function where you define your validation rules using the standard FluentValidation syntax.
- Internally, it checks if the value is specified (`IsSpecified`) before applying the validation rules.

#### Example Usage

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

## Use Cases

### API Patch Operations

When updating resources via API endpoints, it's crucial to distinguish between fields that should be updated to `null` and fields that should remain unchanged.

```csharp
public class UpdateUserRequest
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OptionalValue<string?> Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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

## Limitations

- **JSON Serialization**: You must annotate properties of type `OptionalValue<T>` with `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]` to avoid serialization errors for unspecified values.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests on the [GitHub repository](https://github.com/desjoerd/OptionalValues).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
