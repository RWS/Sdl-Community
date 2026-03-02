using System.Xml;
using System.Xml.Xsl;

namespace CaptureQARuleState_Tests.Tests
{
    public class TransformationTest
    {
        [Fact]
        public void XmlTransformationTest()
        {
            var xsltUri = Helper.GetUriOfResourceFile(TestResources.DefaultSettingsStyle);
            var xmlUri = Helper.GetUriOfResourceFile(TestResources.DefaultQaVerifierSettings);

            var xslt = new XslCompiledTransform();
            xslt.Load(xsltUri);

            using var xmlReader = XmlReader.Create(xmlUri);
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false };
            using var htmlWriter = XmlWriter.Create("output.html", settings);

            xslt.Transform(xmlReader, htmlWriter);
        }
    }
}