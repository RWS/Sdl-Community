using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Multilingual.XML.FileType.Extensions;
using Multilingual.XML.FileType.FileType.Settings;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
	public class XmlFileSniffer : INativeFileSniffer
	{
		private LanguageMappingSettings _languageMappingSettings;
		private readonly DefaultNamespaceHelper _defaultNamespaceHelper;
		private readonly XmlReaderFactory _xmlReaderFactory;

		public XmlFileSniffer(DefaultNamespaceHelper defaultNamespaceHelper, XmlReaderFactory xmlReaderFactory)
		{
			_defaultNamespaceHelper = defaultNamespaceHelper;
			_xmlReaderFactory = xmlReaderFactory;
		}

		private void OverrideSettings(ISettingsGroup settingsGroup)
		{
			if (settingsGroup == null)
			{
				return;
			}

			_languageMappingSettings = new LanguageMappingSettings();
			_languageMappingSettings.PopulateFromSettingsBundle(settingsGroup.SettingsBundle, Constants.FileTypeDefinitionId);
		}

		public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, Codepage suggestedCodepage,
			INativeTextLocationMessageReporter messageReporter, ISettingsGroup settingsGroup)
		{
			OverrideSettings(settingsGroup);

			var info = new SniffInfoWithNativePath(nativeFilePath);
			Encoding suggestedEncoding = null;
			if (suggestedCodepage != null)
				suggestedEncoding = suggestedCodepage.Encoding;

			if (suggestedEncoding == null && suggestedSourceLanguage?.CultureInfo != null)
			{
				var codepage = FileEncoding.GetDefaultAnsiCodepage(suggestedSourceLanguage.CultureInfo);
				if (codepage != 0)
					suggestedEncoding = Encoding.GetEncoding(codepage);
			}

			info.DetectedEncoding = FileEncoding.Detect(nativeFilePath, suggestedEncoding, out var lineBreakType, out var hasUTF8Bom);
			info.SetMetaData("LineBreakType", lineBreakType);
			info.SetMetaData("HasUTF8Bom", hasUTF8Bom.ToString());

			var encoding = info.DetectedEncoding.First.Encoding ?? Encoding.UTF8;
			var namespaceUri = _defaultNamespaceHelper.GetXmlNameSpaceUri(nativeFilePath, info.DetectedEncoding.First.Encoding ?? Encoding.UTF8);
			if (!string.IsNullOrEmpty(namespaceUri))
			{
				info.SetMetaData(Constants.DefaultNamespace, namespaceUri);
			}

			if (File.Exists(nativeFilePath))
			{
				info.IsSupported = IsFileSupported(nativeFilePath, namespaceUri, encoding);
			}

			return info;
		}

		private bool IsFileSupported(string filePath, string namespaceUri, Encoding encoding)
		{
			bool supported;

			if (string.IsNullOrEmpty(_languageMappingSettings?.LanguageMappingLanguagesXPath))
			{
				return false;
			}

			var document = new XmlDocument();
			using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (var reader = _xmlReaderFactory.CreateReader(stream, encoding, true))
				{
					document.Load(reader);

					var nsmgr = new XmlNamespaceManager(document.NameTable);
					document.AddAllNamespaces(nsmgr);

                    if (!string.IsNullOrEmpty(namespaceUri))
					{
						var defaultNameSpace = new XmlNameSpace { Name = Constants.DefaultNamespace, Value = namespaceUri };
						_defaultNamespaceHelper.AddXmlNameSpacesFromDocument(nsmgr, document, defaultNameSpace);

						
						_languageMappingSettings.LanguageMappingLanguagesXPath =
							_defaultNamespaceHelper.UpdateXPathWithNamespace(
								_languageMappingSettings.LanguageMappingLanguagesXPath, Constants.DefaultNamespace);

						foreach (var mappingLanguage in _languageMappingSettings.LanguageMappingLanguages)
						{
							mappingLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
								mappingLanguage.XPath, Constants.DefaultNamespace);

							mappingLanguage.CommentXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
								mappingLanguage.CommentXPath, Constants.DefaultNamespace);
						}
					}

					var nodes = document.SelectNodes(_languageMappingSettings.LanguageMappingLanguagesXPath, nsmgr);
					foreach (XmlNode node in nodes)
					{
						if (_languageMappingSettings.LanguageMappingLanguages.Select(language =>
                        node?.SafeSelectSingleNode(language.XPath, nsmgr)).Any(target => target != null) == true)
						{
							return true;
						}
						
					}
				}
			}

			return false;
		}
	}
}
