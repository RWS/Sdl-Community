using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class PackageService:IPackageService
	{
		private readonly List<KeyValuePair<string, string>> _dictionaryPropetries =
			new List<KeyValuePair<string, string>>();
		private readonly Dictionary<string, List<KeyValuePair<string, string>>> _pluginDictionary =
			new Dictionary<string, List<KeyValuePair<string, string>>>();
		private static PackageModel _package = new PackageModel();
		private const char LanguageTargetSeparator = ' ';
		private readonly IFileService _fileService = new FileService();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Opens a ppf package and saves to files to temp folder
		/// </summary>
		public async Task<PackageModel> OpenPackage(string packagePath, string pathToTempFolder)
		{
			try
			{
				var entryName = string.Empty;
				if (File.Exists(packagePath))
				{
					using (var archive = ZipFile.OpenRead(packagePath))
					{
						foreach (var entry in archive.Entries)
						{
							var subdirectoriesPath = Path.GetDirectoryName(entry.FullName);

							if (subdirectoriesPath != null)
							{
								Directory.CreateDirectory(Path.Combine(pathToTempFolder, subdirectoriesPath));
							}

							entry.ExtractToFile(Path.Combine(pathToTempFolder, entry.FullName));

							if (entry.FullName.EndsWith(".PRJ", StringComparison.OrdinalIgnoreCase))
							{
								entryName = entry.FullName;
							}
						}
					}

					return await ReadProjectMetadata(pathToTempFolder, entryName);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}

			return new PackageModel();
		}

		/// <summary>
		/// Reads the metadata from .PRJ file
		/// </summary>
		private async Task<PackageModel> ReadProjectMetadata(string pathToTempFolder, string fileName)
		{
			var filePath = Path.Combine(pathToTempFolder, fileName);
			var keyProperty = string.Empty;

			using (var reader = new StreamReader(filePath, Encoding.Default))
			{
				string line;
				while ((line = await reader.ReadLineAsync()) != null)
				{
					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						var valuesDictionaries = new List<KeyValuePair<string, string>>();
						if (keyProperty != string.Empty && _dictionaryPropetries.Any())
						{
							valuesDictionaries.AddRange(
								_dictionaryPropetries.Select(
									property => new KeyValuePair<string, string>(property.Key, property.Value)));
							if (_pluginDictionary.ContainsKey(keyProperty))
							{
								_pluginDictionary[keyProperty].AddRange(valuesDictionaries);
							}
							else
							{
								_pluginDictionary.Add(keyProperty, valuesDictionaries);
							}

							_dictionaryPropetries.Clear();
							if (keyProperty.Equals("Languages"))
								break; // we don't want to read the irrelevant information after language info
						}

						var firstPosition = line.IndexOf("[", StringComparison.Ordinal) + 1;
						var lastPosition = line.IndexOf("]", StringComparison.Ordinal) - 1;
						keyProperty = line.Substring(firstPosition, lastPosition);
					}
					else
					{
						var properties = line.Split('=');
						if (!string.IsNullOrEmpty(properties[1]))
						{
							_dictionaryPropetries.Add(
								new KeyValuePair<string, string>(properties[0], properties[1]));
						}
					}
				}
			}

			var packageModel = await CreateModel(pathToTempFolder);
			packageModel.PathToPrjFile = filePath;

			_package = packageModel;
			return packageModel;
		}

		public PackageModel GetPackageModel()
		{
			return _package;
		}

		public bool PackageContainsTms(PackageModel packageModel)
		{
			return packageModel.LanguagePairs.Any(pair => pair.StarTranslationMemoryMetadatas.Count != 0);
		}

		/// <summary>
		/// Creates a package model based on info read from .prj file
		/// </summary>
		private async Task<PackageModel> CreateModel(string pathToTempFolder)
		{
			var model = new PackageModel();
			CultureInfo sourceLanguageCultureInfo = null;
			var languagePairList = new List<LanguagePair>();

			if (_pluginDictionary.ContainsKey("Admin"))
			{
				var propertiesDictionary = _pluginDictionary["Admin"];
				foreach (var item in propertiesDictionary)
				{
					if (item.Key != "ProjectName") continue;
					model.Name = item.Value;
					break;
				}
			}

			if (_pluginDictionary.ContainsKey("Languages"))
			{
				var propertiesDictionary = _pluginDictionary["Languages"];
				foreach (var item in propertiesDictionary)
				{
					if (item.Key == "SourceLanguage")
					{
						var sourceLanguageCode = int.Parse(item.Value);
						sourceLanguageCultureInfo = new CultureInfo(sourceLanguageCode);
					}

					if (item.Key != "TargetLanguages") continue;
					//we assume languages code are separated by " "
					var languages = item.Value.Split(LanguageTargetSeparator);

					foreach (var language in languages)
					{
						var targetLanguageCode = int.Parse(language);
						var targetCultureInfo = new CultureInfo(targetLanguageCode);
						var pair = new LanguagePair
						{
							LanguagePairId = Guid.NewGuid(),
							SourceLanguage = sourceLanguageCultureInfo,
							TargetLanguage = targetCultureInfo,
							TargetFlag = new Language(targetCultureInfo).GetFlagImage(),
							SourceFlag = new Language(sourceLanguageCultureInfo).GetFlagImage(),
							TmsForMainTm = new List<StarTranslationMemoryMetadata>(),
							IndividualTms = new List<StarTranslationMemoryMetadata>(),
							GroupedTmsByPenalty = new List<IGrouping<int, StarTranslationMemoryMetadata>>(),
							NoTm = true
						};
						languagePairList.Add(pair);
					}
				}
			}

			model.LanguagePairs = languagePairList;
			if (model.LanguagePairs.Count > 0)
			{
				//for source
				var sourceFilesAndTmsPath = GetFilesAndTmsFromTempFolder(pathToTempFolder, sourceLanguageCultureInfo);
				var filesAndMetadata = ReturnSourceFilesNameAndMetadata(sourceFilesAndTmsPath);

				//for target
				foreach (var languagePair in model.LanguagePairs)
				{
					var targetFilesAndTmsPath =
						GetFilesAndTmsFromTempFolder(pathToTempFolder, languagePair.TargetLanguage);
					AddFilesAndTmsToModel(languagePair, filesAndMetadata, targetFilesAndTmsPath);
				}
			}

			model.SourceLanguage = model.LanguagePairs[0].SourceLanguage;
			model.SourceFlag = new Language(model.SourceLanguage).GetFlagImage();
			var containsTms = PackageContainsTms(model);
			model.PackageContainsTms = containsTms;
			return model;
		}

		private void AddFilesAndTmsToModel(LanguagePair languagePair,
			Tuple<List<string>, List<StarTranslationMemoryMetadata>> sourceFilesAndTmsPath,
			List<string> targetFilesAndTmsPath)
		{
			var pathToTargetFiles = new List<string>();
			var tmMetaDatas = new List<StarTranslationMemoryMetadata>();
			var sourcefileList = sourceFilesAndTmsPath.Item1;
			var tmMetadataList = sourceFilesAndTmsPath.Item2;

			languagePair.HasTm = tmMetadataList.Count > 0;
			languagePair.SourceFile = sourcefileList;

			foreach (var file in targetFilesAndTmsPath)
			{
				var isTm = _fileService.IsTransitTm(file);
				if (isTm)
				{
					var targetFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
					//selects the source tm which has the same id with the target tm id
					var metaData = tmMetadataList.FirstOrDefault(x =>
						Path.GetFileNameWithoutExtension(x.SourceFile).Equals(targetFileNameWithoutExtension));

					if (metaData != null)
					{
						metaData.TargetFile = file;
						metaData.Name = targetFileNameWithoutExtension;
						metaData.IsMtFile = targetFileNameWithoutExtension.Contains("_AEXTR_MT_");
					}

					tmMetaDatas.Add(metaData);
				}
				else
				{
					pathToTargetFiles.Add(file);
				}
			}

			languagePair.StarTranslationMemoryMetadatas = tmMetaDatas;
			languagePair.TargetFile = pathToTargetFiles;
		}

		private List<string> GetFilesAndTmsFromTempFolder(string pathToTempFolder, CultureInfo language)
		{
			var filesAndTms = new List<string>();

			var extension = language.ThreeLetterWindowsLanguageName;
			extension = _fileService.MapStarTransitLanguage(extension);

			// used for following scenario: for one Windows language (Ex: Nigeria), Star Transit might use different extensions (eg: EDO,EFI)
			var multiLanguageExtensions = extension.Split(',');
			foreach (var multiLangExtension in multiLanguageExtensions)
			{
				var files = Directory.GetFiles(pathToTempFolder, $"*.{multiLangExtension.Trim()}",
					SearchOption.TopDirectoryOnly).ToList();
				filesAndTms.AddRange(files);
			}

			return filesAndTms;
		}

		private Tuple<List<string>, List<StarTranslationMemoryMetadata>> ReturnSourceFilesNameAndMetadata(
			List<string> filesAndTmsList)
		{
			var translationMemoryMetadataList = new List<StarTranslationMemoryMetadata>();
			var fileNames = new List<string>();

			foreach (var file in filesAndTmsList)
			{
				var isTm = _fileService.IsTransitTm(file);
				if (isTm)
				{
					var metadata = new StarTranslationMemoryMetadata {SourceFile = file};
					translationMemoryMetadataList.Add(metadata);
				}
				else
				{
					fileNames.Add(file);
				}
			}

			return new Tuple<List<string>, List<StarTranslationMemoryMetadata>>(fileNames,
				translationMemoryMetadataList);
		}
	}
}