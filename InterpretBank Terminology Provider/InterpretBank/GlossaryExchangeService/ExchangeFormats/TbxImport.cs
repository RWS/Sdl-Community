using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using InterpretBank.GlossaryExchangeService.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;

namespace InterpretBank.GlossaryExchangeService.ExchangeFormats;

public class TbxImport : IImport
{
	public TbxImport(IXmlReaderWriterWrapper tbxDocument, string path)
	{
		TbxDocument = tbxDocument;
		Path = path;
	}

	private string Path { get; }

	private IXmlReaderWriterWrapper TbxDocument { get; }

	public IEnumerable<string[]> ImportTerms()
	{
		var termEntries = TbxDocument.GetTermElements(Path);

		yield return GetTableHeader(termEntries.First());
		foreach (var xmlTermEntry in termEntries)
		{
			var langSets = xmlTermEntry.Elements("langSet");
			var langSetsTotal = langSets.Count();
			var termEntryLength = langSetsTotal * 3 + 1;

			var termEntry = new string[termEntryLength];
			for (var langSetIndex = 0; langSetIndex < langSetsTotal; langSetIndex++)
			{
				var currentLangSet = langSets.ElementAt(langSetIndex);
				var termData = currentLangSet.Element("tig");

				var term = termData.Element("term").Value;
				var extraA = termData.Element("extra1").Value;
				var extraB = termData.Element("extra2").Value;

				var index = langSetIndex * 3;

				termEntry[index] = GetNormalizedValue(term);
				termEntry[index + 1] = GetNormalizedValue(extraA);
				termEntry[index + 2] = GetNormalizedValue(extraB);
			}

			var commentAll = xmlTermEntry.Element("CommentAll").Value;
			termEntry[termEntry.Length - 1] = GetNormalizedValue(commentAll);
			yield return termEntry;
		}
	}

	private string GetNormalizedValue(string value) => !string.IsNullOrEmpty(value) ? value : null;

	private static string[] GetTableHeader(XElement termEntry)
	{
		var langSets = termEntry.Elements("langSet");
		var langSetsTotal = langSets.Count();
		var termEntryLength = langSetsTotal * 3 + 1;

		var tableHeader = new string[termEntryLength];
		for (var langSetIndex = 0; langSetIndex < langSetsTotal; langSetIndex++)
		{
			var currentLangSet = langSets.ElementAt(langSetIndex);

			var language = new CultureInfo(currentLangSet.Attribute(XNamespace.Xml + "lang").Value).EnglishName;

			var index = langSetIndex * 3;

			tableHeader[index] = language;
			tableHeader[index + 1] = $"{language} ExtraA";
			tableHeader[index + 2] = $"{language} ExtraB";
		}

		tableHeader[tableHeader.Length - 1] = "CommentAll";
		return tableHeader;
	}
}