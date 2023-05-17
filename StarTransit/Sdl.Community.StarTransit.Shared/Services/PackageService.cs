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
using Sdl.Core.Globalization.LanguageRegistry;

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
			_languagePlatformLanguages= LanguageRegistryApi.Instance.GetAllLanguages().ToList();
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
					var splitedValues = file.Value.Split('|');
					var fileName = splitedValues[splitedValues.Length - 3];
					var fileExists = fileNames.Any(f => f.Equals(fileName));
					if (!fileExists)
					{
						fileNames.Add(fileName);
					}
				}
			}
			else
			{
				_eventAggregatorService?.PublishEvent(new Error { ErrorMessage = Resources.NoFilesFromPrj });

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
						targetLanguages.Add(LanguageRegistryApi.Instance.GetLanguage(targetCultureInfo.Name));

						var pair = new LanguagePair
						{
							LanguagePairId = Guid.NewGuid(),
							SourceLanguage = sourceLanguageCultureInfo,
							TargetLanguage = targetCultureInfo,
							TargetFlag = LanguageRegistryApi.Instance.GetLanguage(targetCultureInfo.Name).GetFlagImage(),
							SourceFlag = LanguageRegistryApi.Instance.GetLanguage(sourceLanguageCultureInfo.Name).GetFlagImage(),
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
			model.SourceFlag = LanguageRegistryApi.Instance.GetLanguage(model.SourceLanguage.Name).GetFlagImage();
			model.TargetLanguages = targetLanguages.ToArray();
			model.PackageContainsTms = PackageContainsTms(model);

			return model;
		}

		/// <summary>
		/// Set source,target and tms on language pairs
		/// </summary>
		/// <param name="filesNames">Name of the files read from prj file</param>
		private void SetTransitFilesOnModel(string pathToTempFolder, PackageModel model, CultureInfo sourceLanguageCultureInfo, List<string> filesNames)
		{
			var sourceFiles = GetFilesPathForLanguage(pathToTempFolder, sourceLanguageCultureInfo, filesNames);
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

				if (!targetFiles.Any())
				{
					_logger.Error($"Could not find Transit target files on local machine for target language: {languagePair.TargetLanguage.Name}");
					filesFoundLocally = false;
				}
				var tms = GetTransitTmsForLanguagePair(pathToTempFolder, model.Name, languagePair);

				languagePair.StarTranslationMemoryMetadatas = new List<StarTranslationMemoryMetadata>(tms);
			}

			if (!filesFoundLocally)
			{
				_eventAggregatorService?.PublishEvent(new Error { ErrorMessage = Resources.NoTransitFilesError });
			}
		}

		private List<StarTranslationMemoryMetadata> GetTransitTmsForLanguagePair(string pathToTempFolder,
			string packageName,LanguagePair languagePair)
		{
			var availableTms = new List<StarTranslationMemoryMetadata>();
			var metadataFileInfo = new MetadataFileInfo
			{
				SourceMtFilesPath = new List<string>(),
				SourceTmFilesPath = new List<string>(),
				TargetMtFilesPath = new List<string>(),
				TargetTmFilesPath = new List<string>()
			};

			var sourceFilesExtension = Path.GetExtension(languagePair.SourceFile[0]);
			var targetFilesExtension = Path.GetExtension(languagePair.TargetFile[0]);

			if (!_fileService.AreFilesExtensionsSupported(sourceFilesExtension, targetFilesExtension))
				return availableTms;

			//_AXTR TMs/MTs
			var tempDirInfo = new DirectoryInfo(pathToTempFolder);
			var allTms = tempDirInfo.GetFiles("_AEXTR_*", SearchOption.TopDirectoryOnly).GroupBy(f => f.Extension);

			GroupTmsIntoLists(allTms, targetFilesExtension, sourceFilesExtension, metadataFileInfo);
			SetAvailableTms(availableTms, languagePair, metadataFileInfo, packageName);

			//Ref folder tms
			var refFolderPath = Path.Combine(pathToTempFolder, "REF");
			if (Directory.Exists(refFolderPath))
			{
				var refFolderDirectories = Directory.GetDirectories(refFolderPath);

				foreach (var subDirectory in refFolderDirectories)
				{
					var subDirFileInfo = new DirectoryInfo(subDirectory).GetFiles().GroupBy(f => f.Extension);
					GroupTmsIntoLists(subDirFileInfo, targetFilesExtension, sourceFilesExtension, metadataFileInfo);
				}
				metadataFileInfo.IsRefFolderMetadata = true;
				SetAvailableTms(availableTms, languagePair, metadataFileInfo, packageName);
			}

			if (availableTms.Any())
			{
				languagePair.HasTm = true;
			}
			return availableTms;
		}

		private void SetAvailableTms(List<StarTranslationMemoryMetadata> availableTms,LanguagePair languagePair, MetadataFileInfo metadataFileInfo,string packageName)
		{
			var sourceLangCode = languagePair.SourceLanguage.TwoLetterISOLanguageName;
			var targetLangCode = languagePair.TargetLanguage.TwoLetterISOLanguageName;
			if (metadataFileInfo.SourceTmFilesPath.Any())
			{
				var tm = new StarTranslationMemoryMetadata
				{
					Name = $"{packageName}.{sourceLangCode}-{targetLangCode}",
					TransitTmsSourceFilesPath = new List<string>(metadataFileInfo.SourceTmFilesPath),
					TransitTmsTargeteFilesPath = new List<string>(metadataFileInfo.TargetTmFilesPath),
					IsReferenceMeta = metadataFileInfo.IsRefFolderMetadata
				};
				availableTms.Add(tm);
			}

			if (metadataFileInfo.SourceMtFilesPath.Any())
			{
				var mt = new StarTranslationMemoryMetadata
				{
					Name = $"MT_{packageName}.{sourceLangCode}-{targetLangCode}",
					TransitTmsSourceFilesPath = new List<string>(metadataFileInfo.SourceMtFilesPath),
					TransitTmsTargeteFilesPath = new List<string>(metadataFileInfo.TargetMtFilesPath),
					IsReferenceMeta = metadataFileInfo.IsRefFolderMetadata,
					IsMtFile = true
				};
				availableTms.Add(mt);
			}
		}
		
		private void GroupTmsIntoLists(IEnumerable<IGrouping<string, FileInfo>> subDirFileInfo, string targetExtension, string transitSourceExtension, MetadataFileInfo metadataFileInfo)
		{
			foreach (var subGroupInfo in subDirFileInfo)
			{
				//We create the source files based on the target files because in REF folder case we'll need only the source files for the current language pair not all of them
				if (subGroupInfo.Key.Contains(targetExtension))
				{
					var files = subGroupInfo.ToList();
					var tmFiles = files.Where(f => !f.Name.Contains("MT")).Select(f => f.FullName);
					var mtFiles = files
						.Where(f => f.Name.Contains("MT"))
						.Select(f => f.FullName);
					metadataFileInfo.TargetTmFilesPath.AddRange(tmFiles);
					metadataFileInfo.TargetMtFilesPath.AddRange(mtFiles);

					metadataFileInfo.SourceTmFilesPath.AddRange(GetCorrespondingSourceFiles(tmFiles,transitSourceExtension));
					metadataFileInfo.SourceMtFilesPath.AddRange(GetCorrespondingSourceFiles(mtFiles,transitSourceExtension));
				}
			}
		}

		/// <summary>
		/// Transit source - target files have same name only the extension is different. The extension is the language code.
		/// </summary>
		/// <param name="targetFilesPath">Path to temp folder where the target files are extracted</param>
		/// <param name="sourceExtension">Source files extension (Language code)</param>
		private List<string> GetCorrespondingSourceFiles(IEnumerable<string> targetFilesPath, string sourceExtension)
		{
			var correspondingSourceFiles = new List<string>();
			if (targetFilesPath == null) return correspondingSourceFiles;
			foreach (var tmFile in targetFilesPath)
			{
				var name = Path.GetFileNameWithoutExtension(tmFile);
				var parentDirectory = Path.GetDirectoryName(tmFile);
				if (parentDirectory != null)
				{
					var correspondingSourceFile = Path.Combine(parentDirectory, $"{name}{sourceExtension}");
					if (File.Exists(correspondingSourceFile))
					{
						correspondingSourceFiles.Add(correspondingSourceFile);
					}
				}
			}
			return correspondingSourceFiles;
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