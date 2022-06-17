using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitSniffer : INativeFileSniffer
	{
		private string _srcFileExtension;
		private string _trgFileExtension;
		private IFileService _fileService = new FileService();
		private readonly PackageService _packageService = new PackageService();

		public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage,
			Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter,
			ISettingsGroup settingsGroup)
		{
			var info = new SniffInfo();
			_fileService = new FileService();
			var packageModel = _packageService.GetPackageModel();
			var sourceLanguageExtension = string.Empty;
			if (packageModel != null)
			{
				sourceLanguageExtension = packageModel.LanguagePairs[0].SourceLanguage.ThreeLetterWindowsLanguageName;
			}

			if (File.Exists(nativeFilePath))
			{
				// call method to check if file is supported
				info.IsSupported = IsFileSupported(nativeFilePath, sourceLanguageExtension);
				// call method to determine the file language pair
				SetFileLanguages(ref info);
			}
			else
			{
				info.IsSupported = false;
			}
			return info;
		}

		// determine whether a given file is supported based on the
		// root element
		private bool IsFileSupported(string nativeFilePath, string sourceLanguageExtension)
		{
			var isTransitFile = _fileService.IsTransitFile(nativeFilePath);
			if (!isTransitFile) return false;
			var sourceFileFound = false;

			var directoryPath = Path.GetDirectoryName(nativeFilePath);
			if (string.IsNullOrEmpty(directoryPath)) return false;

			// check whether source file with the same name exists
			// if it does not exist, the file cannot be opened in Studio
			var files = Directory.GetFiles(directoryPath).ToList();
			if (files.Count != 0)
			{
				var sourceFileName = Path.GetFileNameWithoutExtension(nativeFilePath) + "." + sourceLanguageExtension;
				var sourceFilesFromFolder = files.Where(s => s.EndsWith(sourceLanguageExtension)).ToList();

				var sourceFile = sourceFilesFromFolder.FirstOrDefault(f => f.Contains(sourceFileName));
				if (sourceFile != null)
				{
					sourceFileFound = true;
					_srcFileExtension = sourceLanguageExtension;
					_trgFileExtension = Path.GetExtension(nativeFilePath).Replace(".", "");
				}
			}
			return sourceFileFound;
		}

		private void SetFileLanguages(ref SniffInfo info)
		{
			info.DetectedSourceLanguage = new Pair<Language, DetectionLevel>(new Language(_fileService.MapFileLanguage(_srcFileExtension)), DetectionLevel.Certain);
			info.DetectedTargetLanguage = new Pair<Language, DetectionLevel>(new Language(_fileService.MapFileLanguage(_trgFileExtension)), DetectionLevel.Certain);
		}
	}
}