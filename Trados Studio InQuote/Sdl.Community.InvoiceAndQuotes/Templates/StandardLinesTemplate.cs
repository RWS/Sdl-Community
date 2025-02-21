using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.OpenXML.Excel;
using Sdl.Community.InvoiceAndQuotes.Projects;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    public class StandardLinesTemplate : TemplateRatesBase
    {
        public StandardLinesTemplate()
        {
            Name = "StandardLinesTemplate";
            ExcludedType = StandardType.Global;
        }

        public override List<RateValue> GetAdditionalStandardRates()
        {
            return new List<RateValue>()
                {
                    new RateValue() {ResourceToken = "CharactersPerLine", Type = Templates.CharactersPerLine, Rate = 0},
                    new RateValue() {ResourceToken = "RatePerLine", Type = Templates.RatePerLine, Rate = 0}
                };
        }

        public override void GenerateExcelData(List<ProjectFile> files, Customer customer, User user, String fileToSave, String template)
        {
            StandardLinesExcelHelper xmlHelper = new StandardLinesExcelHelper();
            if (String.IsNullOrEmpty(template))
                xmlHelper.CreatePackage(fileToSave, files, customer, user);
            else
                xmlHelper.UpdatePackage(template, fileToSave, files, customer, user);
        }

        public override string GenerateLine(ProjectProperty porjectProperty)
        {
            return String.Format("<TR><TD>{0}</TD> <TD>{1}</TD> <TD>{2}</TD> <TD>{3}</TD> <TD>{4}</TD> <TD>{5}</TD></TR>", 
                                    porjectProperty.Type,
                                 porjectProperty.Rate.ToString(),
                                 porjectProperty.LinesByCharacters.ToString(),
                                 porjectProperty.ValueByLbC.ToString(),
                                 porjectProperty.LinesByKeyStrokes.ToString(),
                                 porjectProperty.ValueByLbK.ToString());
        }

        public override string GenerateTotal(List<ProjectProperty> porjectProperties)
        {
            return String.Format("<TR><TD></TD> <TD> </TD> <TD><b>{0}</b></TD> <TD><b>{1}</b></TD> <TD><b>{2}</b></TD> <TD><b>{3}</b></TD></TR>", 
                                 porjectProperties.Where(property => property.StandardType != ExcludedType)
                                                  .Sum(property => property.LinesByCharacters)
                                                  .ToString(),
                                 porjectProperties.Where(property => property.StandardType != ExcludedType)
                                                  .Sum(property => property.ValueByLbC)
                                                  .ToString(),
                                 porjectProperties.Where(property => property.StandardType != ExcludedType)
                                                  .Sum(property => property.LinesByKeyStrokes)
                                                  .ToString(),
                                 porjectProperties.Where(property => property.StandardType != ExcludedType)
                                                  .Sum(property => property.ValueByLbK)
                                                  .ToString());
        }

        public override String GenerateHeader(ProjectFile file)
        {
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GenerateJustLines(file));
            sb.AppendLine("<br>");
            sb.AppendLine("<TABLE><tbody>");
            sb.AppendLine(String.Format("<TR><TD><b>{0}</b></TD> <TD><b>{1}</b></TD> <TD><b>{2}</b></TD> <TD><b>{3}</b></TD> <TD><b>{4}</b></TD> <TD><b>{5}</b></TD></TR>", 
                resources.Type, resources.Rates,
                                        resources.LinesByCharacters, resources.Value,
                                        resources.LinesByKeystrokes, resources.Value));
            return sb.ToString();
        }

        private String GenerateJustLines(ProjectFile file)
        {
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<TABLE><tbody>");
            sb.AppendLine(String.Format("<TR><TD><b>{0}</b></TD> <TD><b>{1}</b></TD></TR>", resources.Type.PadRight(30, ' '), resources.Rates));
            foreach (var rate in AdditionalRates)
            {
                sb.AppendLine(String.Format("<TR><TD>{0}</TD> <TD>{1}</TD></TR>", rate.Type.PadRight(30, ' '), rate.Rate.ToString()));
            }
            sb.AppendLine("</tbody></TABLE>");
            sb.AppendLine("<br>"); 
            sb.AppendLine(resources.JustLines);
            sb.AppendLine("<br>");
            sb.AppendLine("<TABLE><tbody>");
            sb.AppendLine(String.Format("<TR><TD><b>{0}:</b></TD> <TD>{1} {2}</TD> <TD>{3} {4}</TD></TR>", resources.PaymentByCharacters, file.LinesByCharacters.ToString(), 
                resources.StandardLines, file.ValueByLbC.ToString(), resources.Euros));
            sb.AppendLine(String.Format("<TR><TD><b>{0}:</b></TD> <TD>{1} {2}</TD> <TD>{3} {4}</TD></TR>", resources.PaymentByKeystrokes, file.LinesByKeyStrokes.ToString(),
                resources.StandardLines, file.ValueByLbK.ToString(), resources.Euros));
            sb.AppendLine("</tbody></TABLE>");
            sb.AppendLine("<br>");
            return sb.ToString();
        }
    }
}
