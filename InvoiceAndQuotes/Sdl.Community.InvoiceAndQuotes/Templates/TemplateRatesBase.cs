using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.OpenXML.Word;
using Sdl.Community.InvoiceAndQuotes.Projects;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    [Serializable]
    [XmlInclude(typeof(GroupedAnalysisTemplate))]
    [XmlInclude(typeof(SimpleWordTemplate))]
    [XmlInclude(typeof(StandardLinesTemplate))]
    public class TemplateRatesBase : ITemplateRates
    {
        public string Name { get; set; }
        public List<RateValue> Rates { get; set; }
        public List<RateValue> AdditionalRates { get; set; }
        protected StandardType ExcludedType { get; set; }

        public TemplateRatesBase()
        {
            Rates = new List<RateValue>();
            AdditionalRates = new List<RateValue>();
        }

        public virtual List<RateValue> GetStandardRates()
        {
            return Templates.GetStandardRates();
        }

        public virtual List<RateValue> GetAdditionalStandardRates()
        {
            return new List<RateValue>();
        }

        public List<ProjectProperty> FillRatesForProject(List<ProjectProperty> porjectProperties)
        {
            if (Rates != null && Rates.Count > 0)
            {
                foreach (var rateValue in Rates)
                {
                    ProjectProperty prop = porjectProperties.FirstOrDefault(prjProp => prjProp.Type == rateValue.Type);
                    if (prop != null)
                        prop.Rate = rateValue.Rate;
                }
            }
            if (AdditionalRates != null && AdditionalRates.Count > 0)
            {
                foreach (var rateValue in AdditionalRates)
                {
                    ProjectProperty prop = porjectProperties.FirstOrDefault(prjProp => prjProp.Type == rateValue.Type);
                    if (prop != null)
                        prop.Rate = rateValue.Rate;
                }
            }

            return porjectProperties;
        }

        public void ClearRates()
        {
            Rates.Clear();
            AdditionalRates.Clear();
        }

        public virtual void GenerateClipboardData(List<ProjectFile> projectFiles, Customer customer, User user)
        {
            Encoding enc = Encoding.UTF8;
            const string begin = "Version:0.9\r\nStartHTML:{0:000000}\r\nEndHTML:{1:000000}\r\nStartFragment:{2:000000}\r\nEndFragment:{3:000000}\r\n";
            String htmlBegin = "<html>\r\n<head>\r\n"
                                + "<meta http-equiv=\"Content-Type\""
                                + " content=\"text/html; charset=" + enc.WebName + "\">\r\n"
                                + "<title>HTML clipboard</title>\r\n</head>\r\n<body>\r\n"
                                + "<!--StartFragment-->";

            var sb = new StringBuilder();
            sb.AppendLine(GenerateCustomerData(customer));
            sb.AppendLine("<br>");
            foreach (var porjectFile in projectFiles)
            {
                sb.AppendLine(porjectFile.FileName.Replace(".sdlxliff", ""));
                sb.AppendLine(GenerateHeader(porjectFile));
                foreach (var porjectProperty in porjectFile.ProjectProperties)
                {
                    if (porjectProperty.StandardType != ExcludedType)
                    {
                        sb.AppendLine(GenerateLine(porjectProperty));
                    }
                }
                sb.AppendLine( GenerateTotal(porjectFile.ProjectProperties));
                sb.AppendLine("</tbody></TABLE>");
                sb.AppendLine("<br>");
            }
            sb.AppendLine(GenerateUserData(user));
            String html = sb.ToString();
            const string htmlEnd = "<!--EndFragment-->\r\n</body>\r\n</html>\r\n";

            string beginSample = String.Format(begin, 0, 0, 0, 0);

            int countBegin = enc.GetByteCount(beginSample);
            int countHtmlBegin = enc.GetByteCount(htmlBegin);
            int countHtml = enc.GetByteCount(html);
            int countHtmlEnd = enc.GetByteCount(htmlEnd);

            string htmlTotal = String.Format(
               begin
               , countBegin
               , countBegin + countHtmlBegin + countHtml + countHtmlEnd
               , countBegin + countHtmlBegin
               , countBegin + countHtmlBegin + countHtml
               ) + htmlBegin + html + htmlEnd;

            DataObject obj = new DataObject();
            obj.SetData(DataFormats.Html, new System.IO.MemoryStream(
               enc.GetBytes(htmlTotal)));
            Clipboard.SetDataObject(obj, true);
        }

        public virtual void GenerateExcelData(List<ProjectFile> files, Customer customer, User user, String fileToSave, String template)
        {
        }

        public virtual void GenerateWordData(List<ProjectFile> files, Customer customer, User user, string fileToSave, string template)
        {
            WordHelperBase xmlHelper = new WordHelperBase();
            xmlHelper.UpdatePackage(template, fileToSave, files, customer, user);
        }

        private string GenerateUserData(User user)
        {
            if (user == null)
                return String.Empty;
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            var sb = new StringBuilder();
            sb.AppendLine("<br>");
            sb.AppendLine(resources.GeneratedBy);
            if (!String.IsNullOrEmpty(user.Name))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Name, resources.UserName));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Street))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Street, resources.Street));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.City))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.City, resources.City));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.State))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.State, resources.State));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Zip))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Zip, resources.Zip));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Country))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Country, resources.Country));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Phone))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Phone, resources.Phone));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Mobile))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Mobile, resources.Mobile));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Email))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Email, resources.Email));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Skype))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Skype, resources.Skype));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.WebAddress))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.WebAddress, resources.WebAddress));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(user.Twitter))
            {
                sb.AppendLine(String.Format("   {1}: {0}", user.Twitter, resources.Twitter));
                sb.AppendLine("<br>");
            }

            return sb.ToString();
        }

        private string GenerateCustomerData(Customer customer)
        {
            if (customer == null)
                return String.Empty;
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            var sb = new StringBuilder();
            sb.AppendLine(resources.GeneratedFor);
            sb.AppendLine("<br>");
            if (!String.IsNullOrEmpty(customer.Name))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.Name, resources.CustomerName));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(customer.Street))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.Street, resources.Street));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(customer.City))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.City, resources.City));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(customer.State))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.State, resources.State));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(customer.Zip))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.Zip, resources.Zip));
                sb.AppendLine("<br>");
            }
            if (!String.IsNullOrEmpty(customer.Country))
            {
                sb.AppendLine(String.Format("   {1}: {0}", customer.Country, resources.Country));
                sb.AppendLine("<br>");
            }

            return sb.ToString();
        }

        public virtual string GenerateLine(ProjectProperty porjectProperty)
        {
            return String.Empty;
        }

        public virtual string GenerateTotal(List<ProjectProperty> porjectProperties)
        {
            return String.Empty;
        }

        public virtual String GenerateHeader(ProjectFile file)
        {
            return String.Empty;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Rates", Settings.SerializeObject(Rates));
            info.AddValue("AdditionalRates", Settings.SerializeObject(AdditionalRates));
        }
    }
}