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
    [Theory]
    [InlineData(null, "is null")]
    [InlineData("Hello", "has value")]
    [InlineData(false, "is unspecified")]
    public void CanPatternMatchNullable(object? value, string expected)
    {
        OptionalValue<string> sut;
        if (value is bool and false)
        {
            sut = Unspecified.Value;
        }
        else
        {
            sut = new OptionalValue<string>((string?)value!);
        }

        var result = sut switch
        {
            null => "is null",
            string s => $"has value",
            Unspecified => "is unspecified",
        };

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello", "has value")]
    [InlineData(false, "is unspecified")]
    public void CanPatternMatchNotNull(object? value, string expected)
    {
        OptionalValue<string> sut;
        if (value is bool and false)
        {
            sut = Unspecified.Value;
        }
        else
        {
            sut = new OptionalValue<string>((string?)value!);
        }

        var result = sut switch
        {
            string s => $"has value",
            Unspecified => "is unspecified",
        };

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "has value")]
    [InlineData(false, "is unspecified")]
    public void CanPatternMatchValueType(object? value, string expected)
    {
        OptionalValue<int> sut;
        if (value is bool and false)
        {
            sut = Unspecified.Value;
        }
        else
        {
            sut = new OptionalValue<int>((int)value!);
        }

        var result = sut switch
        {
            int => $"has value",
            Unspecified => "is unspecified",
        };

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "has value")]
    [InlineData(null, "is null")]
    [InlineData(false, "is unspecified")]
    public void CanPatternMatchValueTypeNullable(object? value, string expected)
    {
        OptionalValue<int?> sut;
        if (value is bool and false)
        {
            sut = Unspecified.Value;
        }
        else
        {
            sut = new OptionalValue<int?>((int?)value);
        }

        var result = sut switch
        {
            int => $"has value",
            null => "is null",
            Unspecified => "is unspecified",
        };

        Assert.Equal(expected, result);
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