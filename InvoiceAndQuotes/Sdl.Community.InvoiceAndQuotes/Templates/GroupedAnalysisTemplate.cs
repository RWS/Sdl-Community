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
    public class GroupedAnalysisTemplate : TemplateRatesBase
    {
        public GroupedAnalysisTemplate()
        {
            Name = "GroupedAnalysisTemplate";
            ExcludedType = StandardType.Standard;
        }

        public override List<RateValue> GetStandardRates()
        {
            return new List<RateValue>()
                {
                    new RateValue() {ResourceToken = "RepsAnd100Percent", Type = Templates.RepsAnd100Percent, Rate = 0},
                    new RateValue() {ResourceToken = "FuzzyMatches", Type = Templates.FuzzyMatches, Rate = 0},
                    new RateValue() {ResourceToken = "NoMatch", Type = Templates.NoMatch, Rate = 0},
                    new RateValue() {ResourceToken = "Tags", Type = Templates.Tags, Rate = 0}
                };
        }

        public override void GenerateExcelData(List<ProjectFile> files, Customer customer, User user, String fileToSave, String template)
        {
            GroupedExcelHelper xmlHelper = new GroupedExcelHelper();
            if (String.IsNullOrEmpty(template))
                xmlHelper.CreatePackage(fileToSave, files, customer, user);
            else
                xmlHelper.UpdatePackage(template, fileToSave, files, customer, user);
        }

        public override string GenerateLine(ProjectProperty porjectProperty)
        {

            return String.Format("<TR><TD>{0}</TD> <TD>{1}</TD> <TD>{2}</TD> <TD>{3}</TD></TR>", porjectProperty.Type,
                                 porjectProperty.Rate.ToString(),
                                 porjectProperty.Words.ToString(),
                                 porjectProperty.ValueByWords.ToString());
        }

        public override string GenerateTotal(List<ProjectProperty> porjectProperties)
        {
            return String.Format("<TR><TD></TD> <TD> </TD> <TD><b>{0}</b></TD> <TD><b>{1}</b></TD></TR>", 
                                 porjectProperties.Where(property => property.StandardType != ExcludedType).Sum(property => property.Words).ToString(),
                                  porjectProperties.Where(property => property.StandardType != ExcludedType).Sum(property => property.ValueByWords).ToString());
        }

        public override String GenerateHeader(ProjectFile file)
        {
            UIResources resources = new UIResources(Settings.GetSavedCulture());
                        StringBuilder sb = new StringBuilder();
            sb.AppendLine("<TABLE><tbody>");
            sb.AppendLine(
                String.Format(
                    "<TR><TD><b>{0}</b></TD> <TD><b>{1}</b></TD> <TD><b>{2}</b></TD> <TD><b>{3}</b></TD></TR>",
                    resources.Type, resources.Rates,
                    resources.Words, resources.Value));
            return sb.ToString();
        }
    }
}
