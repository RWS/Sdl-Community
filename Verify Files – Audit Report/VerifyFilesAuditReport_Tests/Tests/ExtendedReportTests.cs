using System.Xml;
using VerifyFilesAuditReport.Components.Reporting;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport_Tests.Tests;

public class ExtendedReportTests
{
    private const string FileGuid = "7a1d3c9e-0000-0000-0000-000000000001";

    private static string SampleReportXml =>
        $"""
         <task name="verification">
           <taskInfo taskId="t1">
             <project name="Sample" />
           </taskInfo>
           <file guid="{FileGuid}" name="file1.docx.sdlxliff">
             <Message><SegmentId>1</SegmentId></Message>
             <Message><SegmentId>2</SegmentId><Status>Draft</Status></Message>
             <Message><SegmentId>3</SegmentId><Status>Translated</Status></Message>
           </file>
         </task>
         """;

    private static XmlDocument Parse(string xml)
    {
        var document = new XmlDocument();
        document.LoadXml(xml);
        return document;
    }

    [Fact]
    public void AddProjectFilesTotal_AddsAttributeToProjectNode()
    {
        var report = new ExtendedReport(SampleReportXml);

        report.AddProjectFilesTotal(5);

        var document = Parse(report.GetExtendedReportXmlString());
        var projectNode = document.SelectSingleNode("//taskInfo/project");
        Assert.Equal("5", projectNode?.Attributes?["projectFilesTotal"]?.Value);
    }

    [Fact]
    public void AddProjectFilesTotal_OverwritesExistingAttribute()
    {
        var report = new ExtendedReport(SampleReportXml);

        report.AddProjectFilesTotal(5);
        report.AddProjectFilesTotal(7);

        var document = Parse(report.GetExtendedReportXmlString());
        var projectNode = document.SelectSingleNode("//taskInfo/project");
        Assert.Equal("7", projectNode?.Attributes?["projectFilesTotal"]?.Value);
    }

    [Fact]
    public void AddProjectFilesTotal_WithoutProjectNode_LeavesReportUntouched()
    {
        var report = new ExtendedReport("<task><file /></task>");

        report.AddProjectFilesTotal(5);

        Assert.DoesNotContain("projectFilesTotal", report.GetExtendedReportXmlString());
    }

    [Fact]
    public void AddStatuses_CreatesStatusNodeForMatchingSegment()
    {
        var report = new ExtendedReport(SampleReportXml);
        var statuses = new List<Segment> { new() { Id = "1", Status = "Translated" } };

        report.AddStatuses(statuses, Guid.Parse(FileGuid));

        var document = Parse(report.GetExtendedReportXmlString());
        var statusNode = document.SelectSingleNode("//Message[SegmentId='1']/Status");
        Assert.Equal("Translated", statusNode?.InnerText);
    }

    [Fact]
    public void AddStatuses_OverwritesExistingStatusNode()
    {
        var report = new ExtendedReport(SampleReportXml);
        var statuses = new List<Segment> { new() { Id = "2", Status = "Signed Off" } };

        report.AddStatuses(statuses, Guid.Parse(FileGuid));

        var document = Parse(report.GetExtendedReportXmlString());
        var statusNode = document.SelectSingleNode("//Message[SegmentId='2']/Status");
        Assert.Equal("Signed Off", statusNode?.InnerText);
    }

    [Fact]
    public void AddStatuses_WithUnknownFileGuid_LeavesMessagesUntouched()
    {
        var report = new ExtendedReport(SampleReportXml);
        var statuses = new List<Segment> { new() { Id = "1", Status = "Translated" } };

        report.AddStatuses(statuses, Guid.NewGuid());

        var document = Parse(report.GetExtendedReportXmlString());
        Assert.Null(document.SelectSingleNode("//Message[SegmentId='1']/Status"));
    }

    [Fact]
    public void AddStatuses_WithNullStatuses_DoesNotThrow()
    {
        var report = new ExtendedReport(SampleReportXml);

        report.AddStatuses(null, Guid.Parse(FileGuid));

        Assert.NotNull(report.GetExtendedReportXmlString());
    }

    [Fact]
    public void FilterMessages_KeepsOnlyMessagesWithSelectedStatuses()
    {
        var report = new ExtendedReport(SampleReportXml);

        report.FilterMessages(["draft"]); // case-insensitive

        var document = Parse(report.GetExtendedReportXmlString());
        var messages = document.SelectNodes("//Message");
        Assert.Equal(1, messages?.Count);
        Assert.Equal("Draft", document.SelectSingleNode("//Message/Status")?.InnerText);
    }

    [Fact]
    public void FilterMessages_RemovesMessagesWithoutStatusNode()
    {
        var report = new ExtendedReport(SampleReportXml);

        report.FilterMessages(["Draft", "Translated"]);

        var document = Parse(report.GetExtendedReportXmlString());
        Assert.Null(document.SelectSingleNode("//Message[SegmentId='1']"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new object[] { new string[0] })]
    public void FilterMessages_WithNoSelection_LeavesAllMessages(string[] statuses)
    {
        var report = new ExtendedReport(SampleReportXml);

        report.FilterMessages(statuses?.ToList());

        var document = Parse(report.GetExtendedReportXmlString());
        Assert.Equal(3, document.SelectNodes("//Message")?.Count);
    }

    [Fact]
    public void FilterMessages_WithAllStatusesSelected_LeavesAllMessages()
    {
        var report = new ExtendedReport(SampleReportXml);
        var allStatuses = new List<string>
        {
            "Not Translated", "Draft", "Translated", "Translation Approved",
            "Translation Rejected", "Signed Off", "Sign-off Rejected", "Locked Segments"
        };

        report.FilterMessages(allStatuses);

        var document = Parse(report.GetExtendedReportXmlString());
        Assert.Equal(3, document.SelectNodes("//Message")?.Count);
    }

    [Fact]
    public void AddActiveQaProviders_AppendsSettingsToReportRoot()
    {
        var report = new ExtendedReport(SampleReportXml);
        var providerSettings = new VerificationProviderSettings
        {
            ProjectVerificationProviders = new VerificationSettingsTreeNode
            {
                Name = "Verification Settings",
                Values = [new() { Name = "QA Checker", Value = "True" }]
            }
        };

        report.AddActiveQaProviders(providerSettings);

        var document = Parse(report.GetExtendedReportXmlString());
        var settingsNode = document.SelectSingleNode("/task/VerificationSettings");
        Assert.NotNull(settingsNode);
        Assert.Equal("Verification Settings", settingsNode.Attributes?["Name"]?.Value);
        Assert.Equal("QA Checker", settingsNode.SelectSingleNode("Setting")?.Attributes?["Name"]?.Value);
    }
}
