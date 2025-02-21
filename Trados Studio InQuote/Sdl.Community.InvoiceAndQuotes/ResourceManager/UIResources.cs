using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Sdl.Community.InvoiceAndQuotes.ResourceManager
{
    public class UIResources
    {
        #region public props
        public string CreateQuote { get { return GetString("CreateQuote"); } }
        public string Templates { get { return GetString("Templates"); } }
        public string UserDetails { get { return GetString("UserDetails"); } }
        public string SelectProject { get { return GetString("SelectProject"); } }
        public string SelectCustomer { get { return GetString("SelectCustomer"); } }
        public string ReportType { get { return GetString("ReportType"); } }
        public string MSWord { get { return GetString("MSWord"); } }
        public string MSExcel { get { return GetString("MSExcel"); } }
        public string Clipboard { get { return GetString("Clipboard"); } }
        public string Customer { get { return GetString("Customer"); } }
        public string CustomerName { get { return GetString("CustomerName"); } }
        public string Street { get { return GetString("Street"); } }
        public string City { get { return GetString("City"); } }
        public string State { get { return GetString("State"); } }
        public string Zip { get { return GetString("Zip"); } }
        public string Country { get { return GetString("Country"); } }
        public string GenerateQuote { get { return GetString("GenerateQuote"); } }
        public string SimpleWordAnalysis { get { return GetString("SimpleWordAnalysis"); } }
        public string StandardLines { get { return GetString("StandardLines"); } }
        public string GroupedAnalysis { get { return GetString("GroupedAnalysis"); } }
        public string StudioAnalysisBands { get { return GetString("StudioAnalysisBands"); } }
        public string UserName { get { return GetString("UserName"); } }
        public string Phone { get { return GetString("Phone"); } }
        public string Mobile { get { return GetString("Mobile"); } }
        public string Email { get { return GetString("Email"); } }
        public string Skype { get { return GetString("Skype"); } }
        public string WebAddress { get { return GetString("WebAddress"); } }
        public string Twitter { get { return GetString("Twitter"); } }
        public string Customers { get { return GetString("Customers"); } }
        public string Add { get { return GetString("Add"); } }
        public string Edit { get { return GetString("Edit"); } }
        public string Delete { get { return GetString("Delete"); } }
        public string Select { get { return GetString("Select"); } }
        public string Close { get { return GetString("Close"); } }
        public string Information { get { return GetString("Information"); } }
        public string QuoteGeneratedClipboard { get { return GetString("QuoteGeneratedClipboard"); } }
        public string QuoteGeneratedWord { get { return GetString("QuoteGeneratedWord"); } }
        public string QuoteGeneratedExcel { get { return GetString("QuoteGeneratedExcel"); } }
        public string Summary { get { return GetString("Summary"); } }
        public string Type { get { return GetString("Type"); } }
        public string Rates { get { return GetString("Rates"); } }
        public string Words { get { return GetString("Words"); } }
        public string Value { get { return GetString("Value"); } }
        public string Characters { get { return GetString("Characters"); } }
        public string LinesByCharacters { get { return GetString("LinesByCharacters"); } }
        public string LinesByKeystrokes { get { return GetString("LinesByKeystrokes"); } }
        public string PaymentByCharacters { get { return GetString("PaymentByCharacters"); } }
        public string PaymentByKeystrokes { get { return GetString("PaymentByKeystrokes"); } }
        public string JustLines { get { return GetString("JustLines"); } }
        public string Euros { get { return GetString("Euros"); } }
        public string GeneratedBy { get { return GetString("GeneratedBy"); } }
        public string GeneratedFor { get { return GetString("GeneratedFor"); } }
        public string PerfectMatch { get { return GetString("PerfectMatch"); } }
        public string ContextMatch { get { return GetString("ContextMatch"); } }
        public string Repetitions { get { return GetString("Repetitions"); } }
        public string Percent100 { get { return GetString("Percent100"); } }
        public string Percent95 { get { return GetString("Percent95"); } }
        public string Percent85 { get { return GetString("Percent85"); } }
        public string Percent75 { get { return GetString("Percent75"); } }
        public string Percent50 { get { return GetString("Percent50"); } }
        public string New { get { return GetString("New"); } }
        public string RepsAnd100Percent { get { return GetString("RepsAnd100Percent"); } }
        public string FuzzyMatches { get { return GetString("FuzzyMatches"); } }
        public string NoMatch { get { return GetString("NoMatch"); } }
        public string Tags { get { return GetString("Tags"); } }
        public string CharactersPerLine { get { return GetString("CharactersPerLine"); } }
        public string RatePerLine { get { return GetString("RatePerLine"); } }
        public string SummaryReportType { get { return GetString("SummaryReportType"); } }
        public string LanguagePair { get { return GetString("LanguagePair"); } }
        public string BreakdownAndSummary { get { return GetString("BreakdownAndSummary"); } }
        public string ProjectsXMLPath { get { return GetString("ProjectsXMLPath"); } }

        #endregion

        #region prive things and .ctor
        private readonly string _culture;

        private string File
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "InvoiceAndQuotes.{0}.resx"); }
        }

        private DataSet _resources;

        public UIResources(String culture)
        {
            _culture = culture;
            LoadResources();
        }

        private void LoadResources()
        {
            if (_resources == null)
            {
                _resources = new DataSet();
                _resources.ReadXml(String.Format(File, _culture));
            }
        }

        public  String GetString(String token)
        {
            DataTable values = _resources.Tables["data"];
            DataRow[] rows = values.Select(String.Format("name = '{0}'", token));
            if (rows.Length == 0)
            {
                return token;
            }
            return rows[0]["Value"].ToString();
        }
        #endregion
    }
}

