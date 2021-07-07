using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IFileService
	{
		string MapFileLanguage(string fileExtension);
		string MapStarTransitLanguage(string fileExtension);
		string[] GetTransitCorrespondingExtension(CultureInfo languageCulture);
		string[] GetTransitCorrespondingExtension(string fileExtension);
		bool IsTransitFile(string filePath);
		//Check for segments with empty tags or with empty tags in tags
		bool IsValidNode(XmlNode originalXmlNode);
		bool IsValidNode(string dataAttribute);
		Language[] GetStudioTargetLanguages(List<LanguagePair> languagePairs);
		bool AreFilesExtensionsSupported(string sourceFileExtension, string targetFileExtension);
		string ConvertStringToHex(string textToConvert, Encoding encoding);
	}
}
