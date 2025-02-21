using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using InterpretBank.GlossaryExchangeService.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;

namespace InterpretBank.GlossaryExchangeService.ExchangeFormats;

public class TbxExport : IExport
{
	public TbxExport(IXmlReaderWriterWrapper tbxDocument, string path)
	{
		TbxDocument = tbxDocument;
		Path = path;
	}

	private CultureInfo[] CultureData { get; } = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
	private string Path { get; }
	private IXmlReaderWriterWrapper TbxDocument { get; }

	public void ExportTerms(IEnumerable<string[]> terms, string glossaryName = null, string subGlossaryName = null)
	{
		var tableHeader = terms.First();
		if ((tableHeader.Length - 1) % 3 != 0) return;

		var lang = XName.Get("lang", XNamespace.Xml.ToString());

		using var tbxDocument = TbxDocument.CreateTbx(Path, glossaryName, subGlossaryName);
		foreach (var term in terms.Skip(1))
		{
			var termEntry = GetTerm(tableHeader, lang, term);
			termEntry.WriteTo(tbxDocument);
		}
	}

	private static XElement GetLangSet(XName lang, string[] term, int i, string language)
	{
		return new("langSet",
			new XAttribute(lang, language),
			new XElement("tig",
				new XElement("term", term[i]),
				new XElement("extra1", term[i + 1]),
				new XElement("extra2", term[i + 2])));
	}

	private XElement GetTerm(string[] tableHeader, XName lang, string[] term)
	{
		var langSets = new List<XElement>();
		var termLength = tableHeader.Length;
		for (var i = 0; i < termLength - 1; i += 3)
		{
			var language = GetTwoLetterLanguageName(tableHeader[i]);
			var langSet = GetLangSet(lang, term, i, language);

			langSets.Add(langSet);
		}

		return new XElement("termEntry",
			new XElement("CommentAll", term[termLength - 1]), langSets);
	}

	private string GetTwoLetterLanguageName(string languageName) => CultureData
		.FirstOrDefault(c => c.DisplayName.StartsWith(languageName))
		?.TwoLetterISOLanguageName.ToLower();
}