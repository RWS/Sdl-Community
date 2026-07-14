using VerifyFilesAuditReport.Extension;

namespace VerifyFilesAuditReport_Tests.Tests;

public class StringExtensionTests
{
    [Theory]
    [InlineData("Setting12", true)]
    [InlineData("Setting", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void EndsWithDigits_DetectsTrailingDigits(string input, bool expected)
    {
        Assert.Equal(expected, input.EndsWithDigits());
    }

    [Fact]
    public void TrimEndingDigits_RemovesTrailingDigits()
    {
        Assert.Equal("Setting", "Setting12".TrimEndingDigits());
    }

    [Fact]
    public void GetEndingDigits_ReturnsTrailingDigits()
    {
        Assert.Equal("12", "Setting12".GetEndingDigits());
    }

    [Fact]
    public void GetEndingDigits_ReturnsEmptyWhenNone()
    {
        Assert.Equal(string.Empty, "Setting".GetEndingDigits());
    }
}
