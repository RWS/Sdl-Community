using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NLog;
using Sdl.Core.Globalization;
using TMX_Lib.Utils;
using TMX_Lib.XmlSplit;

namespace TMX_Lib.TmxFormat
{
	
	public class TmxParser : IDisposable
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private string _fileName;
		// if non-empty -> error parsing the file
		private string _error = "";

		// simple mechanism to handle errors during parsing
		// example: sometimes the language entry (xml:lang) is incorrect
		private int _invalidNodeCount = 0;

		private int _successCount = 0;

		public int QuickImportSplitSize = 16 * 1024 * 1024;

		public bool HasError => _error != "";
		public string Error => _error;

		// invalid nodes
		public int InvalidNodeCount => _invalidNodeCount;
		// nodes with invalid chars
		public int InvalidCharsNodeCount => _splitter?.InvalidCharsNodeCount ?? 0;
		public int SuccessCount => _successCount;

		private XmlSplitter _splitter;
		private XmlDocument _headerDocument;

		public IReadOnlyList<string> Languages()
		{
			return _languagesSet.ToList();
		}
        private HashSet<string> _languagesSet = new HashSet<string>();
        private CultureDictionary _languages = new CultureDictionary();
        private bool _quickImport;

        public string FileName => _fileName;

		private TmxHeader _header;
        public TmxHeader Header {
	        get
	        {
				lock(this)
					return _header;
			}
        }

        public double Progress() => _splitter?.Progress() ?? 0;

        public TmxParser(string fileName, bool quickImport = false)
		{
			_fileName = fileName;
			_quickImport = quickImport;
			// note: loading async is a really bad idea, since all calls to the language direction are sync
			// processing the nodes is async, but at the end of this function, we'll know the source + target languages
			LoadHeader();
		}

        private void ParseHeader(XmlDocument document)
        {
            string sourceLanguage = document.SelectSingleNode("tmx/header/@srclang")?.InnerText ?? "";
            string targetLanguage = document.SelectSingleNode("tmx/body/tu[1]/tuv[2]")?.Attributes?[0].InnerText ?? "";
            List<string> domains = new List<string>();

            var domainsStr = document.SelectSingleNode("tmx/header/prop[@type='x-Domain:SinglePicklist']")?.InnerText ?? "";
            if (domainsStr != "")
                domains = domainsStr.Split(',').Select(s => s.Trim()).ToList();
            var xml = document.SelectSingleNode("tmx/header")?.OuterXml ?? "";
            var creationDateStr = GetAttribute(document.SelectSingleNode("tmx/header"), "creationdate");
            var author = GetAttribute(document.SelectSingleNode("tmx/header"), "creationid");
            DateTime? creationDate = null;
            if (creationDateStr != "")
                creationDate = Iso8601Date(creationDateStr);

			lock(this)
				_header = new TmxHeader(sourceLanguage, targetLanguage, domains, creationDate, author, xml);
        }

        private void LoadHeader()
		{
			_error = "";
			try
			{
				_splitter = new XmlSplitter(_fileName) { QuickImport = _quickImport };
				if (_quickImport)
					_splitter.SplitSize = QuickImportSplitSize;
				_headerDocument = _splitter.TryGetNextSubDocument();
				ParseHeader(_headerDocument);
			}
			catch (Exception e)
			{
				_splitter = null;
				log.Error($"There has been an error parsing {_fileName}: {e.Message}");
				_error = $"There has been an error parsing {Path.GetFileName(_fileName)}: {e.Message}";
			}
		}

        public IReadOnlyList<TmxTranslationUnit> TryReadNextTUs()
        {
	        if (_splitter == null)
				// if splitter was null, there was an error parsing the header
		        return null;

	        var document = _headerDocument ?? _splitter.TryGetNextSubDocument();
	        _headerDocument = null;

	        if (document == null)
		        return null;

	        List<TmxTranslationUnit> translations = new List<TmxTranslationUnit>();
	        foreach (XmlNode item in document.SelectNodes("//tu"))
		        try
		        {
			        translations.Add(NodeToTU(item));
			        ++_successCount;
		        }
				catch (Exception e)
				{
					++_invalidNodeCount;
				}
	        return translations;
        }


		// if not found, returns ""
		private static string GetAttribute(XmlNode node, string attributeName)
		{
			if (node == null)
				return "";
            var found = node.Attributes?.OfType<XmlAttribute>().FirstOrDefault(a => a.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
            var value = found?.Value;
            return value ?? "";
        }

        private static readonly string[] iso8061formats = { 
            "yyyyMMddTHHmmsszzz",
            "yyyyMMddTHHmmsszz",
            "yyyyMMddTHHmmssZ",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:sszz",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyyMMddTHHmmzzz",
            "yyyyMMddTHHmmzz",
            "yyyyMMddTHHmmZ",
            "yyyy-MM-ddTHH:mmzzz",
            "yyyy-MM-ddTHH:mmzz",
            "yyyy-MM-ddTHH:mmZ",
            "yyyyMMddTHHzzz",
            "yyyyMMddTHHzz",
            "yyyyMMddTHHZ",
            "yyyy-MM-ddTHHzzz",
            "yyyy-MM-ddTHHzz",
            "yyyy-MM-ddTHHZ"
        };
        private static DateTime Iso8601Date(string date)
        {
	        try
	        {
		        return DateTime.ParseExact(date, iso8061formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
	        }
			catch 
	        { }

	        try
	        {
				//fallback
				return DateTime.Parse(date);
	        }
	        catch
	        { }

	        return DateTime.MinValue;
        }

        private TmxText TuvToText(XmlNode xmlTuv)
        {
	        var language = GetAttribute(xmlTuv, "xml:lang");
	        var seg = xmlTuv.SelectSingleNode("seg");
			// note: the TextToDbText will also convert to lower-case
			var text = Util.TextToDbText(seg.InnerText, _languages.Culture(language)) ;
	        var formattedText = seg.InnerXml;
	        return new TmxText
	        {
				Language = language,
				LocaseText = text,
				FormattedText = formattedText,
	        };
        }

		private TmxTranslationUnit NodeToTU(XmlNode xmlUnit)
		{
            var tu = new TmxTranslationUnit();
            var properties = xmlUnit.SelectNodes("prop");
            if (properties != null)
	            tu.XmlProperties = string.Join("\r\n", properties.OfType<XmlNode>().Select(p => p.OuterXml));
            if (xmlUnit.Attributes != null)
	            tu.TuAttributes = string.Join(" ", xmlUnit.Attributes.OfType<XmlAttribute>().Select(a => $"{a.Name}=\"{Util.XmlAttribute(a.Value)}\""));

            var tuv = xmlUnit.SelectNodes("tuv");
			if (tuv != null)
				foreach (XmlNode item in tuv)
				{
					var text = TuvToText(item);
					tu.Texts.Add(text);
					_languagesSet.Add(text.Language.ToLowerInvariant());
				}

			var creationDate = GetAttribute(xmlUnit, "creationdate");
            var creationAuthor = GetAttribute(xmlUnit, "creationid");
            var changeDate = GetAttribute(xmlUnit, "changedate");
            var changeAuthor = GetAttribute(xmlUnit, "changeid");

			if (creationDate != "")
                tu.CreationTime = Iso8601Date(creationDate);
            tu.CreationAuthor = creationAuthor;
            if (changeDate != "")
                tu.ChangeTime = Iso8601Date(changeDate);
            tu.ChangeAuthor = changeAuthor;

			// confirmation level
			var confirmationLevel = xmlUnit.SelectSingleNode("prop[@type='x-ConfirmationLevel']")?.InnerText ?? "";
			if (confirmationLevel != "" && Enum.TryParse<ConfirmationLevel>(confirmationLevel, out var cf))
				tu.ConfirmationLevel = cf;
            // domain
            var domain = xmlUnit.SelectSingleNode("prop[@type='x-Domain:SinglePicklist']");
            tu.Domain = domain?.InnerText ?? "";
            return tu;
        }

		public void Dispose()
		{
		}
	}
}
