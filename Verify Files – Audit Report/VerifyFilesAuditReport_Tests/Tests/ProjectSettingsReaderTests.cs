using System.Xml.Linq;
using VerifyFilesAuditReport.Components.SettingsProvider.Components;

namespace VerifyFilesAuditReport_Tests.Tests;

public class ProjectSettingsReaderTests
{
    private const string SampleProjectXml =
        """
        <SdlProject SettingsBundleGuid="project-bundle">
          <LanguageDirections>
            <LanguageDirection SourceLanguageCode="en-US" TargetLanguageCode="de-DE" SettingsBundleGuid="language-bundle" />
          </LanguageDirections>
          <SettingsBundles>
            <SettingsBundle Guid="project-bundle">
              <SettingsBundle>
                <SettingsGroup Id="QAVerificationSettings">
                  <Setting Id="Enabled">True</Setting>
                  <Setting Id="Nested"><Sub>x</Sub></Setting>
                </SettingsGroup>
                <SettingsGroup Id="UnrelatedGroup">
                  <Setting Id="Ignored">1</Setting>
                </SettingsGroup>
              </SettingsBundle>
            </SettingsBundle>
            <SettingsBundle Guid="language-bundle">
              <SettingsBundle>
                <SettingsGroup Id="SettingsTagVerifier">
                  <Setting Id="AddedTagsErrorLevel">False</Setting>
                </SettingsGroup>
              </SettingsBundle>
            </SettingsBundle>
          </SettingsBundles>
          <TermbaseConfiguration>
            <Termbases>
              <Name> MainTermbase </Name>
            </Termbases>
          </TermbaseConfiguration>
        </SdlProject>
        """;

    private readonly ProjectSettingsReader _reader = new();
    private readonly XDocument _document = XDocument.Parse(SampleProjectXml);

    [Fact]
    public void ReadProjectVerificationSettings_ReadsProjectLevelVerificationGroups()
    {
        var settings = _reader.ReadProjectVerificationSettings(_document);

        Assert.Equal("True", settings["QAVerificationSettings"]["Enabled"]);
        Assert.DoesNotContain("UnrelatedGroup", settings.Keys);
    }

    [Fact]
    public void ReadProjectVerificationSettings_KeepsNestedSettingsAsXml()
    {
        var settings = _reader.ReadProjectVerificationSettings(_document);

        Assert.Equal("""<Setting Id="Nested"><Sub>x</Sub></Setting>""", settings["QAVerificationSettings"]["Nested"]);
    }

    [Fact]
    public void ReadProjectVerificationSettings_ReadsLanguageSpecificBundle()
    {
        var settings = _reader.ReadProjectVerificationSettings(_document, "de-DE");

        Assert.Equal("False", settings["SettingsTagVerifier"]["AddedTagsErrorLevel"]);
        Assert.DoesNotContain("QAVerificationSettings", settings.Keys);
    }

    [Fact]
    public void ReadProjectVerificationSettings_AddsTrimmedTermbaseName()
    {
        var settings = _reader.ReadProjectVerificationSettings(_document);

        Assert.Equal("MainTermbase", settings["SettingsTermVerifier"]["TermbaseName"]);
    }

    [Fact]
    public void ReadProjectVerificationSettings_UnknownBundleGuid_ReturnsEmpty()
    {
        var document = XDocument.Parse("""<SdlProject SettingsBundleGuid="missing" />""");

        Assert.Empty(_reader.ReadProjectVerificationSettings(document));
    }
}
