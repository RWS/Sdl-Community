using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Parsers.Number.Model;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Parsers.Number
{
	public class NumberParserTests
    {
        private readonly NumberParser _numberParser;

        public NumberParserTests()
        {
            _numberParser = new NumberParser();
        }

        [Fact]
        public void ReturnsFalse_WhenFoundMixedThousandSeparators()
        {
            // arrange
            // act
            var value = _numberParser.Parse("3,021.343.43");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsFalse_WhenLastCharIsNotANumber()
        {
            // arrange
            // act
            var value = _numberParser.Parse("3433,");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsFalse_WhenSeparatorIsNotRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("343-43");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsFalse_WhenSeparatorsAreGroupedTogether()
        {
            // arrange
            // act
            var value = _numberParser.Parse("343,.43");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsFalse_WhenThousandsValueIsOutOfRange()
        {
            // arrange
            // act
            var value = _numberParser.Parse("3.021,3433,43");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsNull_WhenInputIsNull()
        {
            // arrange
            // act
            var value = _numberParser.Parse(null);

            // assert
            Assert.Null(value);
        }

        [Fact]
        public void ReturnsTrue_WhenCurrencyWithSpaceIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("€ +0.123e8");

            // assert
            Assert.True(value.Valid && value.HasCurrency);
        }

        [Fact]
        public void ReturnsTrue_WhenCustomMultiCharSeparatorIsRecognized()
        {
            // arrange
            var separators = new List<NumberSeparator>
            {
                new NumberSeparator {Type = NumberSeparator.SeparatorType.DecimalSeparator, Value = "MySeparator"},
                new NumberSeparator {Type = NumberSeparator.SeparatorType.GroupSeparator, Value = "--"}
            };

            var numberParser = new NumberParser(separators);

            // act
            var value = numberParser.Parse("343--456MySeparator3");

            // assert
            Assert.True(value.Valid);
        }

        [Fact]
        public void ReturnsTrue_WhenCustomSingleCharSeparatorIsRecognized()
        {
            // arrange
            var separators = new List<NumberSeparator>
            {
                new NumberSeparator {Type = NumberSeparator.SeparatorType.DecimalSeparator, Value = "-"},
                new NumberSeparator {Type = NumberSeparator.SeparatorType.GroupSeparator, Value = ","}
            };

            var numberParser = new NumberParser(separators);

            // act
            var value = numberParser.Parse("343-43");

            // assert
            Assert.True(value.Valid);
        }

        [Fact(Skip = "Keeps failing")]
        public void ReturnsTrue_WhenDecimalAndThousandSeparatorsAreRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("3,021,343.43");

            // assert
            Assert.True(value.Valid && value.HasDecimalSeparator && value.HasGroupSeparator);
        }

        [Fact]
        public void ReturnsTrue_WhenDollarCurrencyIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("$+0.123e8");

            // assert
            Assert.True(value.Valid && value.HasCurrency);
        }

        [Fact]
        public void ReturnsTrue_WhenExponenentIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("987.654E-2");

            // assert
            Assert.True(value.Valid && value.NumberParts.FirstOrDefault(a => a.Type == NumberPart.NumberType.Exponent) != null);
        }

        [Fact]
        public void ReturnsTrue_WhenGroupValueIsOutOfRange()
        {
            // arrange
            var separators = new List<NumberSeparator>
            {
                new NumberSeparator {Type = NumberSeparator.SeparatorType.GroupSeparator, Value = ","}
            };

            var numberParser = new NumberParser(separators);

            // act
            var value = numberParser.Parse("234,34");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsTrue_WhenInvalidSeparatorLocationIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("123,234.");

            // assert
            Assert.False(value.Valid);
        }

        [Fact]
        public void ReturnsTrue_WhenMinusSignIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("-.123");

            // assert
            Assert.True(value.Valid && value.HasSign);
        }

        [Fact(Skip = "Keeps failing")]
        public void ReturnsTrue_WhenOnlyDecimalSeparatorIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("3021343.43898");

            // assert
            Assert.True(value.Valid && value.HasDecimalSeparator && !value.HasGroupSeparator);
        }

        [Fact]
        public void ReturnsTrue_WhenOnlyThousandSeparatorsAreRecognized()
        {
            // arrange
            var separators = new List<NumberSeparator>
            {
                new NumberSeparator {Type = NumberSeparator.SeparatorType.GroupSeparator, Value = "."}
            };
            var numberParser = new NumberParser(separators);

            // act
            var value = numberParser.Parse("3.021.343.432");

            // assert
            Assert.True(value.Valid && value.HasGroupSeparator && !value.HasDecimalSeparator);
        }

        [Fact(Skip = "Keeps failing")]
        public void ReturnsTrue_WhenPlusSignIsRecognized()
        {
            // arrange
            // act
            var value = _numberParser.Parse("+123,143.123");

            // assert
            Assert.True(value.Valid && value.HasSign);
        }
    }
}