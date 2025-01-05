namespace OptionalValues.Benchmarks.TestObjects;

public class OptionalValueModel
{
    public OptionalValue<int> Age { get; set; } = 42;
    public OptionalValue<string> FirstName { get; set; } = "John";
    public OptionalValue<string> LastName { get; set; } = default;
}