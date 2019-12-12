using System.Globalization;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;
using Xunit;

namespace Sdl.Community.CleanUpTasks.Tests
{
	public class TextUtilitiesTests
    {
        [Fact]
        public void NormalStringCompareDoesNotReturnMatchCaseInSensitiveCultureInfo()
        {
            // https://msdn.microsoft.com/en-us/library/ms973919.aspx#stringsinnet20_topic5
            var original = "file";
            var searchText = "FILE";

            Assert.False(original.NormalStringCompare(searchText, false, new CultureInfo("tr-TR")));
        }

        [Fact]
        public void NormalStringCompareDoesNotReturnMatchCaseSensitive()
        {
            var original = "Here is some text";
            var searchText = "SoMe TeXt";

            Assert.False(original.NormalStringCompare(searchText, true));
        }

        [Fact]
        public void NormalStringCompareDoesNotReturnMatchCaseSensitiveCultureInfo()
        {
            // Width is not ignored
            var original = "カタカナ";
            var searchText = "ｶﾀｶﾅ";

            Assert.False(original.NormalStringCompare(searchText, true, new CultureInfo("ja-JP")));
        }

        [Fact]
        public void NormalStringCompareDoesReturnMatchCaseInSensitiveCultureInfo()
        {
            // https://msdn.microsoft.com/en-us/library/ms973919.aspx#stringsinnet20_topic5
            var original = "file";
            var searchText = "FILE";

            Assert.True(original.NormalStringCompare(searchText, false, new CultureInfo("en-US")));
        }

        [Fact]
        public void NormalStringCompareReturnsMatchCaseSensitive()
        {
            var original = "Here is SoMe TeXt";
            var searchText = "SoMe TeXt";

            Assert.True(original.NormalStringCompare(searchText, true));
        }

        [Fact]
        public void NormalStringReplaceCaseInSensitiveReturnsCorrectResult()
        {
            // Arrange
            var original = "うらにわにはにわにわにはにわにわとりがいる";
            var searchText = "はにわ";
            var replacement = "埴輪";

            // Act
            var result = original.NormalStringReplace(searchText, replacement, false);

            // Assert
            Assert.Equal("うらにわに埴輪にわに埴輪にわとりがいる", result);
        }

        [Fact]
        public void NormalStringReplaceCaseSensitiveReturnsCorrectResult()
        {
            // Arrange
            var original = "The quick brown FoX JumPs Over the lazy dog";
            var searchText = "FoX JumPs Over";
            var replacement = "fox jumps over";

            // Act
            var result = original.NormalStringReplace(searchText, replacement, true);

            // Assert
            Assert.Equal("The quick brown fox jumps over the lazy dog", result);
        }

        [Fact]
        public void RegexCompareCaseInSensitiveReturnsMatch()
        {
            // Arrange
            var original = "2016/04/05";
            var searchText = @"\d{1,4}\/[01]?\d\/[0-3]\d$";

            // Act and Assert
            Assert.True(original.RegexCompare(searchText, false));
        }

        [Fact]
        public void RegexCompareCaseSensitiveReturnsMatch()
        {
            // Arrange
            // http://stackoverflow.com/questions/4740984/c-sharp-regex-matches-example
            var original = "Lorem ipsum dolor sit %download%#456 amet, consectetur adipiscing %download%#3434 elit. Duis non nunc nec mauris feugiat porttitor. Sed tincidunt blandit dui a viverra%download%#298. Aenean dapibus nisl %download%#893434 id nibh auctor vel tempor velit blandit.";
            var searchText = @"(?<=%download%#)\d+";

            // Act and Assert
            Assert.True(original.RegexCompare(searchText, true));
        }

        [Fact]
        public void RegexCompareInvalidExpressionReturnsFalse()
        {
            // Arrange
            var original = "Whatever";
            var invalid = ".)";

            // Act and Assert
            Assert.False(original.RegexCompare(invalid, false));
        }

        [Fact]
        public void RegexReplaceCaseSensitiveToUpperReturnsMatch()
        {
            // Arrange
            var original = "make the first letter of every word capitalized";
            var search = @"\b[a-z]{1}";
            var replacement = new ReplacementText()
            {
                ToUpper = true
            };

            // Act
            var result = original.RegexReplace(search, replacement, true);

            // Assert
            Assert.Equal("Make The First Letter Of Every Word Capitalized", result);
        }
    }
}