using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IFileService
	{
		string MapFileLanguage(string fileExtension);
		bool IsTransitTm(string filePath);
		Language[] GetStudioTargetLanguages(List<LanguagePair> languagePairs);
	}
}
