using QATracker.Components.SettingsProvider.Components;
using QATracker.Components.SettingsProvider.Model;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace CaptureQARuleState_Tests;

public class DefaultSettingsTests
{
    [Fact]
    public void XmlTransformationTest()
    {
        var xsltUri = GetUriOfResourceFile(TestResources.DefaultSettingsStyle);
        var xmlUri = GetUriOfResourceFile(TestResources.DefaultQaVerifierSettings);

        var xslt = new XslCompiledTransform();
        xslt.Load(xsltUri);

        using var xmlReader = XmlReader.Create(xmlUri); 
        var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false };
        using var htmlWriter = XmlWriter.Create("output.html", settings);

        xslt.Transform(xmlReader, htmlWriter);
    }
    
    [Fact]
    public void GetQaVerifierDefaultSettings()
    {
        var defaultSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(Constants.QaVerificationSettings);

        var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
        using var stringWriter = new StringWriter();

        serializer.Serialize(stringWriter, defaultSettings);
        var xmlSettingsString = stringWriter.ToString();

        Assert.Equal(TestResources.DefaultQaVerifierSettings, xmlSettingsString);
    }

    private static string GetUriOfResourceFile(string resourceFile)
    {
        var temp = Path.Combine(Path.GetTempPath(), $"{DateTime.Now.Millisecond}temp.file");

        File.WriteAllText(temp, resourceFile);
        return new Uri(temp, UriKind.Absolute).ToString();
    }

    [Fact]
    public void GetTagVerifierDefaultSettings()
    {
        var defaultSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(Constants.SettingsTagVerifier);

        var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
        using var stringWriter = new StringWriter();

        serializer.Serialize(stringWriter, defaultSettings);
        var xmlSettingsString = stringWriter.ToString();

        Assert.Equal(TestResources.DefaultTagVerifierSettings, xmlSettingsString);
    }

    [Fact]
    public void GetTermVerifierDefaultSettings()
    {
        var defaultSettings = DefaultSettingsProvider.GetDefaultSettingsForVerifier(Constants.SettingsTermVerifier);

        var serializer = new XmlSerializer(typeof(VerificationSettingsTreeNode));
        using var stringWriter = new StringWriter();

        serializer.Serialize(stringWriter, defaultSettings);
        var xmlSettingsString = stringWriter.ToString();

        Assert.Equal(TestResources.DefaultTermVerifierSettings, xmlSettingsString);
    }
}