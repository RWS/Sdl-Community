using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TMX_Lib.Db;

namespace TMX_Lib.XmlSplit
{
	/*
	 *
	 * Assumptions:
	 * - the xml syntax is simple: we have the _root element, that contains the _body, and the _body contains an array of _elements
	 * - the header, if present, is always fully present in the first block
	 */
	public class XmlSplitter : IDisposable
	{
		private string _fileName;

		// the idea for the pad extra - just in case we read, at the end a partial UTF8 char, decoding it can trigger an exception
		// in that case, we simply need a few extra bytes, so that the last UTF8 char is fully read
		private int PAD_EXTRA = 128;

		public int SplitSize = 2 * 1024 * 1024;

		private XmlDocument _document;

		private FileStream _stream;

		private string _rootXmlName ;
		private string _bodyXmlName;
		private string _elementXmlName;

		private string _firstLine = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";

		private bool _firstBlock = true;
		private string _remainingFromLastBlock = "";

		private bool _eofReached = false;
		private byte[] _buffer;
		private int _offset;

		public XmlSplitter(string fileName, string rootXmlName = "tmx", string bodyXmlName = "body", string elementXmlName = "tu")
		{
			_fileName = fileName;
			_rootXmlName = rootXmlName;
			_bodyXmlName = bodyXmlName;
			_elementXmlName = elementXmlName;
			if (!File.Exists(fileName))
				throw new TmxException($"file not found: {fileName}");

			try
			{
				_stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			catch (Exception e)
			{
				throw new TmxException($"can't open file for reading {fileName}");
			}
		}

		// tries to get the same sub-document
		// if it returns null, there are no more sub-documents
		public string TryGetNextString()
		{
			if (_eofReached)
				return null;

			StringBuilder builder = new StringBuilder();
			if (_firstBlock)
				_buffer = new byte[SplitSize + PAD_EXTRA];

			var readByteCount = _stream.Read(_buffer, 0, SplitSize);
			if (readByteCount < 1)
			{
				_eofReached = true;
				return null;
			}

			_offset += readByteCount;
			string curString = "";
			while(true)
				try
				{
					// this can throw if the last UTF8 char hasn't been fully read
					curString = Encoding.UTF8.GetString(_buffer, 0, readByteCount);
					break;
				}
				catch
				{
					var readByte = _stream.Read(_buffer, readByteCount, 1);
					++readByteCount;
					++_offset;
					if (readByte != 1)
						throw new TmxException($"The file {_fileName} contains invalid characters");
				}

			var xmlHeader = _firstLine;
			if (curString.StartsWith("<?"))
				xmlHeader = ""; // already have the header

			if (!_firstBlock)
				xmlHeader += $"<{_rootXmlName}> <{_bodyXmlName}> ";

			curString = xmlHeader + _remainingFromLastBlock + curString;
			var hasEnd = curString.Contains($"</{_rootXmlName}>");
			var hasEndBody = curString.Contains($"</{_bodyXmlName}>");

			if (!hasEnd && !hasEndBody)
			{
				var endElementXml = $"</{_elementXmlName}>";
				var lastElementIdx = curString.LastIndexOf(endElementXml);
				if (lastElementIdx < 0)
					throw new TmxException($"Invalid file {_fileName} - did not find closing {endElementXml}");

				lastElementIdx += endElementXml.Length;
				_remainingFromLastBlock = curString.Substring(lastElementIdx);
				curString = curString.Substring(0, lastElementIdx) + $"</{_bodyXmlName}></{_rootXmlName}>";
			}
			else
			{
				// has end of end-of-body
				_remainingFromLastBlock = "";

				if (!hasEndBody)
					curString += $"</{_bodyXmlName}>";
				if (!hasEnd)
					curString += $"</{_rootXmlName}>";
			}

			_firstBlock = false;
			return curString;
		}

		// tries to get the same sub-document
		// if it returns null, there are no more sub-documents
		public XmlDocument TryGetNextSubDocument()
		{
			var str = TryGetNextString();
			if (str == null)
				return null;

			try
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.XmlResolver = null;
				settings.DtdProcessing = DtdProcessing.Ignore;

				var bytes = Encoding.UTF8.GetBytes(str);
				using (var memoryStream = new MemoryStream(bytes))
				using (var reader = new StreamReader(memoryStream))
				using (var xmlReader = XmlTextReader.Create(reader, settings))
				{
					var document = new XmlDocument();
					document.Load(xmlReader);
					return document;
				}
			}
			catch (Exception e)
			{
				throw new TmxException($"Error getting sub-document", e);
			}
		}

		public void Dispose()
		{
			_document = null;
			_stream?.Dispose();
			_stream = null;
		}
	}
}
