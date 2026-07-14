using VerifyFilesAuditReport.Components.SegmentMetadata;

namespace VerifyFilesAuditReport_Tests.Tests;

public class SegmentStatusesTests
{
    [Theory]
    [InlineData("ApprovedTranslation", "Translation Approved")]
    [InlineData("ApprovedSignOff", "Signed Off")]
    [InlineData("RejectedSignOff", "Sign-off Rejected")]
    [InlineData("Draft", "Draft")]
    public void ToUiName_MapsKnownStatuses(string status, string expected)
    {
        Assert.Equal(expected, SegmentStatuses.ToUiName(status));
    }

    [Fact]
    public void ToUiName_ReturnsInputForUnknownStatus()
    {
        Assert.Equal("SomethingNew", SegmentStatuses.ToUiName("SomethingNew"));
    }

    [Fact]
    public void ToUiName_ReturnsNullForNull()
    {
        Assert.Null(SegmentStatuses.ToUiName(null));
    }
}
