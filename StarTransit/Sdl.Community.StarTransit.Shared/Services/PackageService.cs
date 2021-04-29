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

							//We don't want to read any more information after we get the language and the files info
							if (_pluginDictionary.ContainsKey("Files") &&
							    _pluginDictionary.ContainsKey("Languages")) break;
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

		public List<string> GetFilesNamesFromPrjFile(List<KeyValuePair<string,string>> files)
		{
			var fileNames = new List<string>();
			if (files != null)
			{
				foreach (var file in files)
				{
					var textWithoutLastPipe = file.Value.Substring(0, file.Value.LastIndexOf("|", StringComparison.Ordinal));
					var fileName =
						textWithoutLastPipe.Substring(textWithoutLastPipe.LastIndexOf("|", StringComparison.Ordinal) + 1);

					var fileExists = fileNames.Any(f => f.Equals(fileName));
					if (!fileExists)
					{
						fileNames.Add(fileName);
					}
				}
			}
			else
			{
				_logger.Error("Could read any files information from [File]");
			}
			return fileNames;
		}

		public List<string> GetFilesPath(string pathToExtractedProject, CultureInfo language, List<string> fileNames)
		{
			var filePaths = new List<string>();

			// used for following scenario: for one Windows language (Ex: Nigeria), Star Transit might use different extensions (eg: EDO,EFI)
			var multiLanguageExtensions = _fileService.GetTransitCorrespondingExtension(language);
			foreach (var multiLangExtension in multiLanguageExtensions)
			{
				foreach (var file in fileNames)
				{
					var path = Path.Combine(pathToExtractedProject, $"{file}.{multiLangExtension}");
					var pathExists = filePaths.Any(f => f.Equals(path));
					if (!pathExists)
					{
						filePaths.Add(path);
					}
				}
			}
			return filePaths;
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
			var filesNames = new List<string>();
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
							GroupedTmsByPenalty = new List<IGrouping<int, StarTranslationMemoryMetadata>>(),
							NoTm = true
						};
						languagePairList.Add(pair);
					}
				}
			}

			if (_pluginDictionary.ContainsKey("Files"))
			{
				var filesList = _pluginDictionary["Files"];
				filesNames.AddRange(GetFilesNamesFromPrjFile(filesList));
			}

			model.LanguagePairs = languagePairList;
			if (model.LanguagePairs.Count > 0)
			{
				//for source
				var sourceFilesAndTmsPath = GetFilesAndTmsFromTempFolder(pathToTempFolder, sourceLanguageCultureInfo);
				var filesAndMetadata = ReturnSourceFilesNameAndMetadata(sourceFilesAndTmsPath);
				var sourceFiles = GetFilesPath(pathToTempFolder, sourceLanguageCultureInfo, filesNames);

				//for target
				foreach (var languagePair in model.LanguagePairs)
				{
					var targetFiles = GetFilesPath(pathToTempFolder, languagePair.TargetLanguage, filesNames);
					languagePair.SourceFile = sourceFiles;
					languagePair.TargetFile = targetFiles;
					//TODO: Apeleaza metoda care se uita in doate directoarele dupa tms
					//TODO: DAca exista tms mapeaza la language pair
					var tms = GetTransitTms(pathToTempFolder, model.Name, languagePair);
					languagePair.StarTranslationMemoryMetadatas = new List<StarTranslationMemoryMetadata>(tms);

					//TODO: Crate methods for reading and adding tms info;

					//TODO: Remove this for final implementation
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

		//TODO: ar trebui sa primim ca parametru lista de tms available si doar sa cautam pt lang pair specific
		private List<StarTranslationMemoryMetadata> GetTransitTms(string pathToTempFolder, string packageName, LanguagePair languagePair)
		{
			//TODO: asta ar trebui scoasa in alta metoda
			var tempDirInfo = new DirectoryInfo(pathToTempFolder);

			var allTms = tempDirInfo.GetFiles("_AEXTR_*", SearchOption.TopDirectoryOnly).ToList();
			var refFolderPath = Path.Combine(pathToTempFolder, "REF");
			if (Directory.Exists(refFolderPath))
			{
				var refDir = new DirectoryInfo(refFolderPath);
				var refTms = refDir.GetFiles("*", SearchOption.AllDirectories).ToList();
				if (refTms.Any())
				{
					allTms.AddRange(refTms);
				}
			}

			//We need to take source files based on the target files. Other wise we'll end up with more source files because we take for all language pairs
			if (allTms.Any())
			{
				var availableTms = new List<StarTranslationMemoryMetadata>();
				var transitSourceExtensions =
					_fileService.GetTransitCorrespondingExtension(languagePair.SourceLanguage);

				var transitTargetExtensions =
					_fileService.GetTransitCorrespondingExtension(languagePair.TargetLanguage);

				var sourceTmFilesPath = new List<string>();
				var targetTmFilesPath = new List<string>();
				var sourceMtFilesPath = new List<string>();
				var targetMtFilesPath = new List<string>();

				foreach (var targetExtension in transitTargetExtensions)
				{
					var tmTargetFiles = allTms.Where(f => f.Extension.Contains(targetExtension) && !f.Name.Contains("MT")).Select(f=>f.FullName);
					targetTmFilesPath.AddRange(tmTargetFiles);
					var mtTargetFiles = allTms
						.Where(f => f.Extension.Contains(targetExtension) && f.Name.Contains("MT"))
						.Select(f => f.FullName);
					targetMtFilesPath.AddRange(mtTargetFiles);
				}
				//foreach (var sourceExtension in transitSourceExtensions)
				//{
				//	var tmSourceFiles = allTms.Where(t => t.EndsWith(sourceExtension) && !t.Contains("MT"));
				//	sourceTmFilesPath.AddRange(tmSourceFiles);

				//	var mtSourceFiles = allTms.Where(t => t.EndsWith(sourceExtension) && t.Contains("MT"));
				//	sourceMtFilesPath.AddRange(mtSourceFiles);
				//}



				var sourceLangCode = languagePair.SourceLanguage.TwoLetterISOLanguageName;
				var targetLangCode = languagePair.TargetLanguage.TwoLetterISOLanguageName;

				if (sourceTmFilesPath.Any())
				{
					var tm = new StarTranslationMemoryMetadata
					{
						Name = $"{packageName}.{sourceLangCode}-{targetLangCode}",
						TransitTmsSourceFilesPath = new List<string>(sourceTmFilesPath),
						TransitTmsTargeteFilesPath = new List<string>(targetTmFilesPath)
					};
					availableTms.Add(tm);
				}

				if (sourceMtFilesPath.Any())
				{
					var mt = new StarTranslationMemoryMetadata
					{
						Name = $"MT_{packageName}.{sourceLangCode}-{targetLangCode}",
						TransitTmsSourceFilesPath = new List<string>(sourceMtFilesPath),
						TransitTmsTargeteFilesPath = new List<string>(targetMtFilesPath),
						IsMtFile = true
					};
					availableTms.Add(mt);
				}

				return availableTms;
			}
			return new List<StarTranslationMemoryMetadata>();
		}


		//TODO: Remove this for final implementation

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

	//	private 

		//TODO: Remove this for final implementation
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

		//TODO: Remove this for final implementation

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