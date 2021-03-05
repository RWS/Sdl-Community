using System.Collections.Generic;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IFileService
	{
		string MapFileLanguage(string fileExtension);
		bool IsTransitTm(string filePath);
		bool IsTransitFile(string filePath);
		//Check for segments with empty tags or with empty tags in tags
		bool IsValidNode(XmlNode originalXmlNode);
		Language[] GetStudioTargetLanguages(List<LanguagePair> languagePairs);
	}
}
