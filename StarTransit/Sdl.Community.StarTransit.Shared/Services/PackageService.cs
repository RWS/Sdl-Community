using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class PackageService:IPackageService
	{
		private readonly List<KeyValuePair<string, string>> _dictionaryPropetries;
		private readonly Dictionary<string, List<KeyValuePair<string, string>>> _pluginDictionary;
		private static PackageModel _package;
		private const char LanguageTargetSeparator = ' ';
		private readonly IFileService _fileService;
		private readonly IEventAggregatorService _eventAggregatorService;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly List<Language> _languagePlatformLanguages;
		private Dictionary<int, CultureInfo> _transitMappingCulture; 

		public PackageService()
		{
			_languagePlatformLanguages= Language.GetAllLanguages().ToList();
			_fileService = new FileService();
			_pluginDictionary =
				new Dictionary<string, List<KeyValuePair<string, string>>>();
			_dictionaryPropetries =
				new List<KeyValuePair<string, string>>();
			InitializeLcidMappingDictionary();
		}

		public PackageService(IEventAggregatorService eventAggregatorService):this()
		{
			_eventAggregatorService = eventAggregatorService;
		}

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
				_eventAggregatorService.PublishEvent(new Error { ErrorMessage = Resources.NoFilesFromPrj });

				_logger.Error("Could read any files information from [File]");
			}
			return fileNames;
		}

		public List<string> GetFilesPathForLanguage(string pathToExtractedProject, CultureInfo language, List<string> fileNames)
		{
			var filePaths = new List<string>();

			// used for following scenario: for one Windows language (Ex: Nigeria), Star Transit might use different extensions (eg: EDO,EFI)
			var multiLanguageExtensions = _fileService.GetTransitCorrespondingExtension(language);
			foreach (var multiLangExtension in multiLanguageExtensions)
			{
				foreach (var file in fileNames)
				{
					var filePath = Path.Combine(pathToExtractedProject, $"{file}.{multiLangExtension}");
					if (!File.Exists(filePath)) continue;
					var pathExists = filePaths.Any(f => f.Equals(filePath));
					if (!pathExists)
					{
						filePaths.Add(filePath);
					}
				}
			}
			return filePaths;
		}

		public bool PackageContainsTms(PackageModel packageModel)
		{
			return packageModel.LanguagePairs.Any(pair => pair.StarTranslationMemoryMetadatas.Count != 0);
		}

		public CultureInfo GetMappingCultureForLcId(int lcId,string projectName)
		{
			var isLcidSupported = IsLcIdSupportedByStudio(lcId, projectName);
			if (isLcidSupported)
			{
				return new CultureInfo(lcId);
			}
			var mappingExist = _transitMappingCulture.TryGetValue(lcId, out var languageCulture);
			if (mappingExist)
			{
				return languageCulture;
			}

			_logger.Info($"For project{projectName} following LCID: {lcId} does not have Transit Mapping Language added");

			return new CultureInfo(lcId);
		}

		/// <summary>
		/// Creates a package model based on info read from .prj file
		/// </summary>
		private async Task<PackageModel> CreateModel(string pathToTempFolder)
		{
			var model = new PackageModel
			{
				LanguagePairs = new List<LanguagePair>()
			};

			CultureInfo sourceLanguageCultureInfo = null;
			var filesNames = new List<string>();
			var targetLanguages = new List<Language>();
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
						sourceLanguageCultureInfo = GetMappingCultureForLcId(sourceLanguageCode, model.Name);
					}

					if (item.Key != "TargetLanguages") continue;
					var languages = item.Value.Split(LanguageTargetSeparator);
					foreach (var language in languages)
					{
						var targetLanguageCode = int.Parse(language);
						var targetCultureInfo = GetMappingCultureForLcId(targetLanguageCode, model.Name);
						targetLanguages.Add(new Language(targetCultureInfo));

						var pair = new LanguagePair
						{
							LanguagePairId = Guid.NewGuid(),
							SourceLanguage = sourceLanguageCultureInfo,
							TargetLanguage = targetCultureInfo,
							TargetFlag = new Language(targetCultureInfo).GetFlagImage(),
							SourceFlag = new Language(sourceLanguageCultureInfo).GetFlagImage(),
							NoTm = true
						};
						model.LanguagePairs.Add(pair);
					}
				}
			}

			if (_pluginDictionary.ContainsKey("Files"))
			{
				var filesList = _pluginDictionary["Files"];
				filesNames.AddRange(GetFilesNamesFromPrjFile(filesList));
			}

			if (model.LanguagePairs.Count > 0)
			{
				SetTransitFilesOnModel(pathToTempFolder, model, sourceLanguageCultureInfo, filesNames);
			}

			model.SourceLanguage = model.LanguagePairs[0].SourceLanguage;
			model.SourceFlag = new Language(model.SourceLanguage).GetFlagImage();
			model.TargetLanguages = targetLanguages.ToArray();
			model.PackageContainsTms = PackageContainsTms(model);

			return model;
		}

		private void SetTransitFilesOnModel(string pathToTempFolder, PackageModel model, CultureInfo sourceLanguageCultureInfo, List<string> filesNames)
		{
			var sourceFiles = GetFilesPathForLanguage(pathToTempFolder, sourceLanguageCultureInfo, filesNames);
			var transitSourceExtensions =
				_fileService.GetTransitCorrespondingExtension(model.LanguagePairs[0].SourceLanguage);
			var filesFoundLocally = true;
			if (!sourceFiles.Any())
			{
				filesFoundLocally = false;
				_logger.Error($"Could not find Transit source files on local machine. Source language {sourceLanguageCultureInfo.Name}");
			}

			//for target
			foreach (var languagePair in model.LanguagePairs)
			{
				var targetFiles =
					GetFilesPathForLanguage(pathToTempFolder, languagePair.TargetLanguage, filesNames);
				languagePair.SourceFile = sourceFiles;
				languagePair.TargetFile = targetFiles;
				languagePair.SelectedTranslationMemoryMetadatas = new List<StarTranslationMemoryMetadata>();
				var transitTargetExtensions =
					_fileService.GetTransitCorrespondingExtension(languagePair.TargetLanguage);

				if (!targetFiles.Any())
				{
					_logger.Error($"Could not find Transit target files on local machine for target language: {languagePair.TargetLanguage.Name}");
					filesFoundLocally = false;
				}

				var tms = GetTransitTmsForLanguagePair(pathToTempFolder, model.Name, transitSourceExtensions,
					transitTargetExtensions, languagePair);

				languagePair.StarTranslationMemoryMetadatas = new List<StarTranslationMemoryMetadata>(tms);
			}

			if (!filesFoundLocally)
			{
				_eventAggregatorService.PublishEvent(new Error { ErrorMessage = Resources.NoTransitFilesError });
			}
		}

		private List<StarTranslationMemoryMetadata> GetTransitTmsForLanguagePair(string pathToTempFolder,
			string packageName, string[] transitSourceExtensions, string[] transitTargetExtensions,LanguagePair languagePair)
		{
			var availableTms = new List<StarTranslationMemoryMetadata>();
			var sourceTmFilesPath = new List<string>();
			var targetTmFilesPath = new List<string>();
			var sourceMtFilesPath = new List<string>();
			var targetMtFilesPath = new List<string>();

			//_AXTR TMs/MTs
			var tempDirInfo = new DirectoryInfo(pathToTempFolder);
			var allTms = tempDirInfo.GetFiles("_AEXTR_*", SearchOption.TopDirectoryOnly).GroupBy(f => f.Extension);

			foreach (var transitTargetExtension in transitTargetExtensions)
			{
				var targetExtension = $".{transitTargetExtension}";
				GroupTmsIntoLists(allTms, targetExtension, transitSourceExtensions, targetTmFilesPath, targetMtFilesPath,
					sourceTmFilesPath, sourceMtFilesPath);
			}

			//Ref folder tms
			var refFolderPath = Path.Combine(pathToTempFolder, "REF");
			if (Directory.Exists(refFolderPath))
			{
				var refFolderDirectories = Directory.GetDirectories(refFolderPath);

				foreach (var subDirectory in refFolderDirectories)
				{
					var subDirFileInfo = new DirectoryInfo(subDirectory).GetFiles().GroupBy(f => f.Extension);

					foreach (var transitTargetExtension in transitTargetExtensions)
					{
						var targetExtension = $".{transitTargetExtension}";
						var targetLanguageExists = subDirFileInfo.Any(t => t.Key.Equals(targetExtension));
						if (targetLanguageExists)
						{
							GroupTmsIntoLists(subDirFileInfo, targetExtension, transitSourceExtensions, targetTmFilesPath,
								targetMtFilesPath, sourceTmFilesPath, sourceMtFilesPath);
						}
					}
				}
			}

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

			if (availableTms.Any())
			{
				languagePair.HasTm = true;
			}

			return availableTms;
		}

		private void GroupTmsIntoLists(IEnumerable<IGrouping<string, FileInfo>> subDirFileInfo, string targetExtension, string[] transitSourceExtensions, List<string> targetTmFilesPath,
			List<string> targetMtFilesPath, List<string> sourceTmFilesPath, List<string> sourceMtFilesPath)
		{
			foreach (var subGroupInfo in subDirFileInfo)
			{
				var files = subGroupInfo.ToList();
				var tmFiles = files.Where(f => !f.Name.Contains("MT")).Select(f => f.FullName);
				var mtFiles = files
					.Where(f => f.Name.Contains("MT"))
					.Select(f => f.FullName);
				if (subGroupInfo.Key.Equals(targetExtension))
				{
					targetTmFilesPath.AddRange(tmFiles);
					targetMtFilesPath.AddRange(mtFiles);
				}
				else
				{
					//There are folders which contains all source and target files. We want only the one corresponding for current lang pair
					foreach (var transitSourceExtension in transitSourceExtensions)
					{
						var sourceExtension = $".{transitSourceExtension}";
						if (!subGroupInfo.Key.Equals(sourceExtension)) continue;
						sourceTmFilesPath.AddRange(tmFiles);
						sourceMtFilesPath.AddRange(mtFiles);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the LCID used by transit is supported by Language Platform
		/// </summary>
		/// <param name="lcId">LCID from Transit Package</param>
		/// <param name="projectName">Transit Project name</param>
		public bool IsLcIdSupportedByStudio(int lcId,string projectName)
		{
			var languageExist = _languagePlatformLanguages.Any(l => l.CultureInfo.LCID.Equals(lcId));
			if (!languageExist)
			{
				_logger.Info($"For project{projectName} following LCID: {lcId} is not supported by LanguagePlatform");
			}

			return languageExist;
		}

		private void InitializeLcidMappingDictionary()
		{
			_transitMappingCulture = new Dictionary<int, CultureInfo>
			{
				{2074, new CultureInfo("sr-Latn-RS")}, //Serbian Latin
				{3098, new CultureInfo("sr-Cyrl-RS")} //Serbian Cyrillic
			};
		}
	}
}