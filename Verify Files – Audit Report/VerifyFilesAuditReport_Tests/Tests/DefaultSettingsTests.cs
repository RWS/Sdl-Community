using VerifyFilesAuditReport.Components.SettingsProvider;
using VerifyFilesAuditReport.Components.SettingsProvider.Components;
using VerifyFilesAuditReport.Components.SettingsProvider.Model;
using System.Xml.Serialization;
using VerifyFilesAuditReport_Tests;

namespace CaptureQARuleState_Tests.Tests;

public class DefaultSettingsTests
{
    //[Fact]
    //public void GetQaVerifierDefaultSettings()
    //{
    //    var defaultSettings = CategoryMap.CreateVerificationSettings(Constants.QaVerificationSettings);

    //    var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
    //    using var stringWriter = new StringWriter();

    //    serializer.Serialize(stringWriter, defaultSettings);
    //    var xmlSettingsString = stringWriter.ToString();

    //    Assert.Equal(TestResources.DefaultQaVerifierSettings, xmlSettingsString);
    //}

    //[Fact]
    //public void GetTagVerifierDefaultSettings()
    //{
    //    var defaultSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(Constants.SettingsTagVerifier);

    //    var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
    //    using var stringWriter = new StringWriter();

    //    serializer.Serialize(stringWriter, defaultSettings);
    //    var xmlSettingsString = stringWriter.ToString();

    //    Assert.Equal(TestResources.DefaultTagVerifierSettings, xmlSettingsString);
    //}

    //[Fact]
    //public void GetTermVerifierDefaultSettings()
    //{
    //    var defaultSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(Constants.SettingsTermVerifier);

    //    var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
    //    using var stringWriter = new StringWriter();

    //    serializer.Serialize(stringWriter, defaultSettings);
    //    var xmlSettingsString = stringWriter.ToString();

    //    Assert.Equal(TestResources.DefaultTermVerifierSettings, xmlSettingsString);
    //}
}