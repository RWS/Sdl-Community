using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Reports
{
   public  class Report
    {

       internal enum ReportType
       {
           Xml = 0,
           Html
       }

       private const string DefaultXsltName = "TausTM.Provider.Debug.StyleSheet.xslt";

       public void CreateSearchSegmentResultXmlReport(string reportFilePath, SearchSegmentResult searchResult, bool transformReport)
       {
           var xmlTxtWriter = new XmlTextWriter(reportFilePath, Encoding.UTF8)
           {
               Formatting = Formatting.None,
               Indentation = 3,
               Namespaces = false
           };
           xmlTxtWriter.WriteStartDocument(true);

                 

           xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + DefaultXsltName + "'");
           xmlTxtWriter.WriteComment("Taus TM Provider by Patrick Hartnett, 2011");


           xmlTxtWriter.WriteStartElement("segments");
           xmlTxtWriter.WriteAttributeString("count", searchResult.Segments.Count.ToString());

           if (searchResult.Segments.Count > 0)
           {
               xmlTxtWriter.WriteAttributeString("sourceLanguage.id", searchResult.Segments[0].SourceLanguage.Id);
               xmlTxtWriter.WriteAttributeString("sourceLanguage.name", searchResult.Segments[0].SourceLanguage.Name);
               xmlTxtWriter.WriteAttributeString("targetLanguage.id", searchResult.Segments[0].TargetLanguage.Id);
               xmlTxtWriter.WriteAttributeString("targetLanguage.name", searchResult.Segments[0].TargetLanguage.Name);

               xmlTxtWriter.WriteAttributeString("date", DateTime.Now.ToString(CultureInfo.InvariantCulture));
               foreach (var segment in searchResult.Segments)
               {

                   xmlTxtWriter.WriteStartElement("segment");

                   xmlTxtWriter.WriteAttributeString("id", segment.Id);


                   xmlTxtWriter.WriteAttributeString("sourceLanguage.id", segment.SourceLanguage.Id);
                   xmlTxtWriter.WriteAttributeString("sourceLanguage.name", segment.SourceLanguage.Name);

                   xmlTxtWriter.WriteAttributeString("targetLanguage.id", segment.TargetLanguage.Id);
                   xmlTxtWriter.WriteAttributeString("targetLanguage.name", segment.TargetLanguage.Name);

                   xmlTxtWriter.WriteAttributeString("provider.id", segment.Provider.Id);
                   xmlTxtWriter.WriteAttributeString("provider.name", segment.Provider.Name);

                   xmlTxtWriter.WriteAttributeString("industry.id", segment.Industry.Id);
                   xmlTxtWriter.WriteAttributeString("industry.name", segment.Industry.Name);

                   xmlTxtWriter.WriteAttributeString("contentType.id", segment.ContentType.Id);
                   xmlTxtWriter.WriteAttributeString("contentType.name", segment.ContentType.Name);



                   xmlTxtWriter.WriteAttributeString("owner.id", segment.Owner.Id);
                   xmlTxtWriter.WriteAttributeString("owner.name", segment.Owner.Name);

                   xmlTxtWriter.WriteAttributeString("product.id", segment.Product.Id);
                   xmlTxtWriter.WriteAttributeString("product.name", segment.Product.Name);


                   xmlTxtWriter.WriteAttributeString("matchPercentage", segment.MatchPercentage + "%");


                   xmlTxtWriter.WriteStartElement("sourceText");
                   xmlTxtWriter.WriteString(segment.SourceText);
                   xmlTxtWriter.WriteEndElement();//sourceText

                   xmlTxtWriter.WriteStartElement("targetText");
                   xmlTxtWriter.WriteString(segment.TargetText);
                   xmlTxtWriter.WriteEndElement();//sourceText

                   xmlTxtWriter.WriteEndElement();//segment

               }
           }

           xmlTxtWriter.WriteEndElement();//segments

           xmlTxtWriter.WriteEndDocument();
           xmlTxtWriter.Flush();
           xmlTxtWriter.Close();

           WriteReportResourcesToDirectory(Path.GetDirectoryName(reportFilePath));

           if (transformReport)
               TransformXmlReport(reportFilePath);
       }

       private static void TransformXmlReport(string reportFilePath)
       {
           if (reportFilePath != null)
           {
               var filePathXslt = Path.Combine(Path.GetDirectoryName(reportFilePath), DefaultXsltName);

               var myXPathDoc = new XPathDocument(reportFilePath);
               var myXslTrans = new XslCompiledTransform();
               myXslTrans.Load(filePathXslt);
               var myWriter = new XmlTextWriter(reportFilePath + ".html", Encoding.UTF8);
               myXslTrans.Transform(myXPathDoc, null, myWriter);

               myWriter.Flush();
               myWriter.Close();




               File.Delete(filePathXslt);
           }
           File.Delete(reportFilePath);

       }

       private static void WriteReportResourcesToDirectory(string reportDirectory)
       {

           var filePathXslt = Path.Combine(reportDirectory, DefaultXsltName);


         
           var assembly = Assembly.GetExecutingAssembly();

           const string templateXsltName = "Taus.TM.Provider.Reports.Taus.TM.Provider.Debug.StyleSheet.xslt";


           using (var inputStream = assembly.GetManifestResourceStream(templateXsltName))
           {

               Stream outputStream = File.Open(filePathXslt, FileMode.Create);

               if (inputStream != null)
               {
                   var bsInput = new BufferedStream(inputStream);
                   var bsOutput = new BufferedStream(outputStream);

                   var buffer = new byte[1024];
                   int bytesRead;

                   while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
                   {
                       bsOutput.Write(buffer, 0, bytesRead);
                   }

                   bsInput.Flush();
                   bsOutput.Flush();
                   bsInput.Close();
                   bsOutput.Close();
               }
           }
        


       }
    }
}
