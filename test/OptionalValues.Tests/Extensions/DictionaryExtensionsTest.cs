using OptionalValues.Extensions;

namespace OptionalValues.Tests.Extensions;

public class DictionaryExtensionsTest
{
    public class GetOptionalValue : DictionaryExtensionsTest
    {
        [Fact]
        public void ReturnsUnspecifiedWhenKeyIsNotFound()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            OptionalValue<int> result = dictionary.GetOptionalValue("key");

            // Assert
            Assert.False(result.IsSpecified);
        }

        [Fact]
        public void ReturnsSpecifiedValueWhenKeyIsFound()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            OptionalValue<int> result = dictionary.GetOptionalValue("key");

            // Assert
            Assert.True(result.IsSpecified);
            Assert.Equal(42, result.SpecifiedValue);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenDictionaryIsNull()
        {
            // Arrange
            IDictionary<string, int> dictionary = null!;

            // Act
            Action action = () => dictionary.GetOptionalValue("key");

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("dictionary", exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenKeyIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            Action action = () => dictionary.GetOptionalValue(null!);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("key", exception.ParamName);
        }
    }

    public class AddOptionalValue : DictionaryExtensionsTest
    {
        [Fact]
        public void AddsValueWhenSpecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            dictionary.AddOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            Assert.Equal(42, dictionary["key"]);
        }

        [Fact]
        public void DoesNotAddValueWhenUnspecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            dictionary.AddOptionalValue("key", OptionalValue<int>.Unspecified);

            // Assert
            Assert.Empty(dictionary);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenDictionaryIsNull()
        {
            // Arrange
            IDictionary<string, int> dictionary = null!;

            // Act
            Action action = () => dictionary.AddOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("dictionary", exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenKeyIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            Action action = () => dictionary.AddOptionalValue(null!, new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("key", exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenKeyAlreadyExists()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            Action action = () => dictionary.AddOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            Assert.Throws<ArgumentException>(action);
        }
    }

    public class TryAddOptionalValue : DictionaryExtensionsTest
    {
        [Fact]
        public void AddsValueWhenSpecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            var result = dictionary.TryAddOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            Assert.True(result);
            Assert.Equal(42, dictionary["key"]);
        }

        [Fact]
        public void DoesNotAddValueWhenUnspecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            var result = dictionary.TryAddOptionalValue("key", OptionalValue<int>.Unspecified);

            // Assert
            Assert.False(result);
            Assert.Empty(dictionary);
        }

        [Fact]
        public void DoesNotAddWhenKeyExists()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            var result = dictionary.TryAddOptionalValue("key", new OptionalValue<int>(9000));

            // Assert
            Assert.False(result);
            Assert.Equal(42, dictionary["key"]);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenDictionaryIsNull()
        {
            // Arrange
            IDictionary<string, int> dictionary = null!;

            // Act
            Action action = () => dictionary.TryAddOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("dictionary", exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenKeyIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            Action action = () => dictionary.TryAddOptionalValue(null!, new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("key", exception.ParamName);
        }
    }

    public class SetOptionalValue : DictionaryExtensionsTest
    {
        [Fact]
        public void SetsValueWhenSpecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            dictionary.SetOptionalValue("key", new OptionalValue<int>(9000));

            // Assert
            Assert.Equal(9000, dictionary["key"]);
        }

        [Fact]
        public void DoesNotSetValueWhenUnspecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            dictionary.SetOptionalValue("key", OptionalValue<int>.Unspecified);

            // Assert
            Assert.Equal(42, dictionary["key"]);
        }

        [Fact]
        public void OverwritesValueWhenSpecified()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>
            {
                ["key"] = 42
            };

            // Act
            dictionary.SetOptionalValue("key", new OptionalValue<int>(9000));

            // Assert
            Assert.Equal(9000, dictionary["key"]);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenDictionaryIsNull()
        {
            // Arrange
            IDictionary<string, int> dictionary = null!;

            // Act
            Action action = () => dictionary.SetOptionalValue("key", new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("dictionary", exception.ParamName);
        }

        [Fact]
        public void ThrowsArgumentNullExceptionWhenKeyIsNull()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            Action action = () => dictionary.SetOptionalValue(null!, new OptionalValue<int>(42));

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Equal("key", exception.ParamName);
        }
    }
}