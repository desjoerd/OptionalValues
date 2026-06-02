#if NET11_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OptionalValues.Tests;

public class UnionTest
{
    [Fact]
    public void CanPatternMatch()
    {
        var optionalValue = new OptionalValue<string?>("Hello");

        var result = optionalValue switch
        {
            string => "has value",
            Unspecified => "is unspecified",
        };

        Assert.Equal("has value", result);
    }

    [Fact]
    public void CanUseUnionMembers()
    {
        Result<string> result = Result<string>.IUnionMembers.Create("Hello");

        var value = result switch
        {
            string s => $"has value: {s}",
            Exception e => $"has exception: {e.Message}",
        };
    }
}

[Union]
public record class Result<T> : Result<T>.IUnionMembers
{
    object? _value;

    public interface IUnionMembers
    {
        public static Result<T> Create(T value) => new() { _value = value };
        public static Result<T> Create(Exception value) => new() { _value = value };

        public object? Value { get; }
    }

    object? IUnionMembers.Value => _value;
}

#endif