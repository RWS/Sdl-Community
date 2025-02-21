using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TMX_Lib.Db;
using TMX_Lib.Utils;

namespace TMX_Lib.Writer
{
	// IMPORTANT:
	//
	// we're actually writing the whole xml by hand, since for non-trivial databases (1M+ entries), dealing with XmlDocument is pretty much impossible (it's too much memory + too slow)
	public class TmxWriter : IDisposable
	{
		private const string FIRST_LINES = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<tmx version=\"1.4\">\r\n";

		private string _fileName;
		private FileStream _stream;
		private StreamWriter _writer;
		private LanguageArray _languageArray;

		// indenting - normally, turn it on for testing
		public bool Indent = false;

		// if language array not set, I'll get it from the database
		public TmxWriter(string fileName, LanguageArray languageArray = null)
		{
			_fileName = fileName;
			_languageArray = languageArray ?? new LanguageArray();
			_stream = new FileStream(_fileName, FileMode.Create, FileAccess.Write);
			_writer = new StreamWriter(_stream, Encoding.UTF8);
		}

		private string IndentStr(int indentCount)
		{
			if (!Indent)
				return "";
			if (indentCount == 0)
				return "";
			return new string(' ', 4 * indentCount);
		}

		private void WriteStart(string header)
		{
			_writer.Write(FIRST_LINES);
			if (header != null)
				_writer.Write(header);
			_writer.Write($"\r\n{IndentStr(1)}<body>\r\n");
		}

		private void Write(TmxText text)
		{
			var language = text.LanguageAndLocale();
			_writer.Write($"{IndentStr(3)}<tuv xml:lang=\"{language}\">\r\n");
			_writer.Write($"{IndentStr(4)}<seg>{Util.XmlValue(text.FormattedText)}</seg>\r\n");
			_writer.Write($"{IndentStr(3)}</tuv>\r\n");
		}

		private static string Iso6001Date(DateTime t)
		{
			return t.ToString("yyyyMMddTHHmmssZ");
		}

		private static string Attributes(TmxTranslationUnit tu)
		{
			if (tu.TuAttributes != "")
				return tu.TuAttributes;
			var attributes = "";
			if (tu.CreationDate != null)
				attributes += $" creationdate=\"{Iso6001Date(tu.CreationDate.Value)}\"";
			if (tu.CreationAuthor != "")
				attributes += $" creationid=\"{Util.XmlAttribute(tu.CreationAuthor)}\"";
			if (tu.ChangeDate != null)
				attributes += $" changedate=\"{Iso6001Date(tu.ChangeDate.Value)}\"";
			if (tu.ChangeAuthor != "")
				attributes += $" changeid=\"{Util.XmlAttribute(tu.ChangeAuthor)}\"";
			return attributes.Trim();
		}

		private string Properties(TmxTranslationUnit tu)
		{
			if (!Indent)
				return tu.XmlProperties;

			var indent = IndentStr(3);
			return indent + tu.XmlProperties.Replace("\r\n", $"\r\n{indent}");
		}

		private void Write(TmxTranslationUnit tu, IReadOnlyList<TmxText> texts)
		{
			var attributes = Attributes(tu);
			_writer.Write(attributes != "" ? $"{IndentStr(2)}<tu {attributes}>\r\n" : $"{IndentStr(2)}<tu>\r\n");
			if (tu.XmlProperties != "")
				_writer.Write($"{Properties(tu)}\r\n");
			foreach (var text in texts)
				Write(text);
			_writer.Write($"{IndentStr(2)}</tu>\r\n");
		}

		private void WriteEnd()
		{
			_writer.Write($"\r\n{IndentStr(1)}</body>\r\n</tmx>\r\n");
		}

		public void Dispose()
		{
			_writer?.Dispose();
			_stream?.Dispose();
		}

		// writeBlock - after writing a block, we'll ask if we should continue or not
		public async Task WriteAsync(TmxMongoDb db, Func<double, bool> continueFunc = null, ulong writeBlock = 100)
		{
			_languageArray = new LanguageArray(); 
			_languageArray.LoadLanguages(await db.GetAllLanguagesAsync());

			var headerMeta = await db.TryFindMetaAsync("Header");
			WriteStart(headerMeta?.Value);
			var maxId = db.MaxTranslationId();
			for (ulong id = 1; id < maxId; ++id)
			{
				var tu = await db.TryFindTranslationUnitAsync(id);
				if (tu != null)
				{
					var texts = await db.FindTextsAsync(id);
					Write(tu, texts);
				}

				if ((id % writeBlock) == 0)
					if (continueFunc != null && !continueFunc( (double)id / (double)maxId))
						// we've been asked to cancel
						break;
			}
			WriteEnd();
		}
	}
}
