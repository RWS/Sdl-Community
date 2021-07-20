using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NSubstitute;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class PackageServiceUnitTests
	{
		private readonly StarTransitConfiguration _starTransitConfiguration;
		private readonly string _testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
		private readonly IPackageService _service;

		public PackageServiceUnitTests()
		{
			var packageService = Substitute.For<PackageService>();
			_starTransitConfiguration = new StarTransitConfiguration(packageService);
			_service = new PackageService();
		}

		
		[Fact]
		public void OpenPackage_IsNotNullPackage_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.NotNull(packageModel);
		}

		[Fact]
		public void OpenPackage_IsNullPackage_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND")?.Result;
			Assert.Null(packageModel.PathToPrjFile);
		}

		[Fact]
		public void OpenPackage_CheckPackageName_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.Equal("693203001_Trumpf_ID", packageModel?.Name);
		}

		[Fact]
		public void OpenPackage_CheckWrongPackageName_UnitTest()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF")?.Result;
			Assert.NotEqual("Trumpf_ID", packageModel?.Name);
		}

		[Fact]
		public void ReturnPackage()
		{
			var model = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF").Result;

			var package = _service.GetPackageModel();
			Assert.NotNull(package);
		}

		//TMs Unit tests for AXTR Tms with only one Language Pair
		//We grouping all the tms for a specific language into only one object. All MTs are in a different object.
		[Theory]
		[InlineData("766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf",2)] //TMs and MTs,
		[InlineData("693203001_Trumpf_ID_IND.PPF", 1)] // AXTR tms
		public void OpenPackage_MtsAndMts_CorrectMetadataNumber_OneLanguagePair(string packageName,int metadataNumber)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, packageName).Result;

			Assert.Equal(metadataNumber,packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas.Count);
		}

		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.PPF",4)]
		[InlineData("693203001_Trumpf_ID_IND.PPF", 1)]
		public void OpenPackage_CorrectPairsLanguageNumber(string packageName, int languagePairsNumber)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, packageName).Result;
			Assert.Equal(languagePairsNumber,packageModel.LanguagePairs.Count);
		}

		[Theory]
		[InlineData("de","id")]
		public void OpenPackage_CorrectTmName_ForAxtrTms(string sourceLanguageCode, string targetLanguageCode)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF").Result;
			var tmName = $"{packageModel.Name}.{sourceLanguageCode}-{targetLanguageCode}";
			Assert.Equal(tmName,packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].Name);
			Assert.DoesNotContain(tmName,"MT");
		}

		[Fact]
		public void OpenPackage_CorrectName_ForAxtrMtsAndTms_OneLanguagePair()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf").Result;
			var mtMetadataName = $"MT_{packageModel.Name}.de-en";
			var tmMetadataName = $"{packageModel.Name}.de-en";

			Assert.Contains(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas,
				meta => meta.Name.Contains(mtMetadataName));
			Assert.Contains(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas,
				meta => meta.Name.Contains(tmMetadataName));
		}

		[Fact]
		public void OpenPackage_FilePath_ForAxtrMtsAndTms_OneLanguagePair()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf").Result;
			var tmMetadata = _starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas,true);
			var mtMetadata = _starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas, false);

			//TM files
			Assert.All(tmMetadata.TransitTmsSourceFilesPath, file => Assert.DoesNotContain("MT", file));
			Assert.All(tmMetadata.TransitTmsTargeteFilesPath, file => Assert.DoesNotContain("MT", file));
			//MT files
			Assert.All(mtMetadata.TransitTmsSourceFilesPath, file => Assert.Contains("MT", file));
			Assert.All(mtMetadata.TransitTmsTargeteFilesPath, file => Assert.Contains("MT", file));
		}

		[Fact]
		public void OpenPackage_CorrectFileNumber_ForAxtrMtsAndTms_OneLanguagePair()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath,
				"766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf").Result;
			var tmMetadata =
				_starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas, true);
			var mtMetadata =
				_starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas, false);
			
			//TM Files
			Assert.Equal(2,tmMetadata.TransitTmsSourceFilesPath.Count);
			Assert.Equal(2, tmMetadata.TransitTmsTargeteFilesPath.Count);
			//MT Files
			Assert.Single(mtMetadata.TransitTmsSourceFilesPath);
			Assert.Single(mtMetadata.TransitTmsTargeteFilesPath);
		}

		[Theory]
		[InlineData("766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf",true)]
		[InlineData("693203001_Trumpf_ID_IND.PPF", false)]
		public void OpenPackage_ContainsMtFile_ForAxtrMtsAndTms_OneLanguagePair(string packageName,bool packageContainsMtFiles)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath,
				packageName).Result;

			//MT Files
			Assert.Contains(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas,
				meta =>meta.IsMtFile.Equals(packageContainsMtFiles));
		}

		[Fact]
		public void OpenPackage_CorrectFileExtension_ForAxtrMtsAndTms_OneLanguagePair()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath,
				"766ec607-508b-4c45-92f2-15c58d1eff53_ENU_00_J00265435.ppf").Result;
			var tmMetadata =
				_starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas, true);
			var mtMetadata =
				_starTransitConfiguration.GetTmsOrMtMetadataFromList(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas, false);

			//TM files
			Assert.All(tmMetadata.TransitTmsSourceFilesPath, file => Assert.EndsWith(".DES", file));
			Assert.All(tmMetadata.TransitTmsTargeteFilesPath, file => Assert.EndsWith(".ENU", file));
			//MT files
			Assert.All(mtMetadata.TransitTmsSourceFilesPath, file => Assert.EndsWith(".DES", file));
			Assert.All(mtMetadata.TransitTmsTargeteFilesPath, file => Assert.EndsWith(".ENU", file));
		}

		[Fact]
		public void OpenPackage_CorrectTmsNumber_OneLanguagePair()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF").Result;

			Assert.Equal(9, packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsSourceFilesPath.Count);
			Assert.Equal(9, packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsTargeteFilesPath.Count);
		}

		[Fact]
		public void OpenPackage_CorrectTmsFileExtension_OneLanguage()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF").Result;

			Assert.All(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsSourceFilesPath, file => Assert.EndsWith(".DEU", file));
			Assert.All(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsTargeteFilesPath, file => Assert.EndsWith(".IND", file));
		}

		//TMs File have following name format: _AEXTR_2
		//MT Files _AEXTR_MT_SM
		[Fact]
		public void OpenPackage_CorrectTmsFile_OneLanguage()
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, "693203001_Trumpf_ID_IND.PPF").Result;

			Assert.All(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsSourceFilesPath, file => Assert.DoesNotContain("MT", file));
			Assert.All(packageModel.LanguagePairs[0].StarTranslationMemoryMetadatas[0].TransitTmsTargeteFilesPath, file => Assert.DoesNotContain("MT", file));
		}

		//Multilingual package test, all the TMS are in Ref folder
		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.PPF", 51)]
		public void OpenPackage_Multilingual_CorrectMetadataNumber(string packageName, int tmFilesNumber)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, packageName).Result;

			Assert.All(packageModel.LanguagePairs,
				languagePair => Assert.Equal(tmFilesNumber,
					languagePair.StarTranslationMemoryMetadatas[0].TransitTmsSourceFilesPath.Count));
			Assert.All(packageModel.LanguagePairs,
				languagePair => Assert.Equal(tmFilesNumber,
					languagePair.StarTranslationMemoryMetadatas[0].TransitTmsTargeteFilesPath.Count));
		}

		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.PPF","de-de","en-en",".DEU",".ENG")]
		[InlineData("Test Multilingual Package Trados Plugin.PPF", "de-de", "fr-fr", ".DEU", ".FRA")]
		[InlineData("Test Multilingual Package Trados Plugin.PPF", "de-de", "it-it", ".DEU", ".ITA")]
		[InlineData("Test Multilingual Package Trados Plugin.PPF", "de-de", "es-es", ".DEU", ".ESP")]
		public void OpenPackage_Multilingual_CorrectFileExtenssion(string packageName,string sourceLanguage,string targetLanguage,string sourceExtension,string targetExtension)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, packageName).Result;
			var languagePair = new LanguagePair
			{
				SourceLanguage = new CultureInfo(sourceLanguage), TargetLanguage = new CultureInfo(targetLanguage)
			};
			var tmFilesForLanguagePair = _starTransitConfiguration.GetTmsForLanguagePair(packageModel.LanguagePairs,languagePair);

			Assert.All(tmFilesForLanguagePair.Item1, file => Assert.EndsWith(sourceExtension, file));
			Assert.All(tmFilesForLanguagePair.Item2, file => Assert.EndsWith(targetExtension, file));
		}

		//Packages can contain: Only TMs, only MTs or TMs and MT files.
		[Theory]
		[InlineData("Test Multilingual Package Trados Plugin.PPF", true, false)]
		public void OpenPackage_Multilingual_CorrectTmName(string packageName, bool packageContainsTm,
			bool packageContainsMt)
		{
			var packageModel = _starTransitConfiguration.GetPackageModel(_testingFilesPath, packageName).Result;
			foreach (var languagePair in packageModel.LanguagePairs)
			{
				var sourceLangCode = languagePair.SourceLanguage.TwoLetterISOLanguageName;
				var targetLangCode = languagePair.TargetLanguage.TwoLetterISOLanguageName;
				var tmName = $"{packageModel.Name}.{sourceLangCode}-{targetLangCode}";
				var mtName = $"MT_{packageModel.Name}.{sourceLangCode}-{targetLangCode}";

				if (packageContainsTm)
				{
					Assert.Contains(languagePair.StarTranslationMemoryMetadatas,
						meta => meta.Name.Contains(tmName));
				}

				if (packageContainsMt)
				{
					Assert.Contains(languagePair.StarTranslationMemoryMetadatas,
						meta => meta.Name.Contains(mtName));
				}
				else
				{
					Assert.DoesNotContain(languagePair.StarTranslationMemoryMetadatas,
						meta => meta.Name.Contains(mtName));
				}
			}
		}

		[Fact]
		public void GetFileName_OnlyOneFile()
		{
			var files = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("File1",
					@"File1=1|1|1|0|1|Y:\Trumpf_Maschinen_Austria_GmbH___Co._KG\000102\693203001\_vorb\FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.xml|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|0")
			};
			var name = _service.GetFilesNamesFromPrjFile(files);
			Assert.Equal("FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN",name[0]);
		}

		[Theory]
		[InlineData(@"File1=1|1|1|0|1|C:\Transit NXT\projects\Probeübersetzung\Probeübersetzung.ttc|Probeübersetzung_ttc|Probeübersetzung ttc|0", "Probeübersetzung_ttc")]
		[InlineData(@"File1=1|1|1|0|1|C:\testdateien\Vorlage DLV kommentiert_gek�rzt.docx|Vorlage DLV kommentiert_gek�rzt|Vorlage DLV kommentiert_gek�rzt|0", "Vorlage DLV kommentiert_gek�rzt")]
		public void GetFileName_CorrectValue(string prjFileInfo,string expectedFileName)
		{
			var files = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("File1",prjFileInfo)
			};
			var name = _service.GetFilesNamesFromPrjFile(files);
			Assert.Equal(expectedFileName, name[0]);
		}

		[Fact]
		public void GetFileName_NoDuplication()
		{
			var files = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("File1",
					@"File1=1|1|1|0|1|Y:\Trumpf_Maschinen_Austria_GmbH___Co._KG\000102\693203001\_vorb\FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.xml|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|0"),
				new KeyValuePair<string, string>("File1",
					@"File1=1|1|1|0|1|Y:\Trumpf_Maschinen_Austria_GmbH___Co._KG\000102\693203001\_vorb\FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.xml|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN|0")
			};

			var fileNames = _service.GetFilesNamesFromPrjFile(files);
			Assert.Single(fileNames);
		}

		[Fact]
		public void GetFileName_NullList()
		{
			var fileNames = _service.GetFilesNamesFromPrjFile(null);
			Assert.Empty(fileNames);
		}

		[Fact]
		public void GetFileName_MultipleFiles()
		{
			var files = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("File1",
					@"File1=1|1|1|0|1|\\sedcaclm0120\Jobs\82ea0668-ad82-46aa-80d3-3f0d4a048ae9\sourcefiles\translationFiles\P-90-20-02-02-Parameterliste-BAi_31d2-000_V6_R1_fr_FR.xml|P-90-20-02-02-Parameterliste-BAi_31d2-000_V6_R1_fr_FR|P-90-20-02-02-Parameterliste-BAi_31d2-000_V6_R1_fr_FR|0"),  
				new KeyValuePair<string, string>("File2",
					@"File2=1|1|1|0|1|\\sedcaclm0120\Jobs\82ea0668-ad82-46aa-80d3-3f0d4a048ae9\sourcefiles\translationFiles\P-90-30-02-02-Parameterliste-Gui_9fcc-001_V6_R1_fr_FR.xml|P-90-30-02-02-Parameterliste-Gui_9fcc-001_V6_R1_fr_FR|P-90-30-02-02-Parameterliste-Gui_9fcc-001_V6_R1_fr_FR|0")
			};
			
			var fileNames = _service.GetFilesNamesFromPrjFile(files);

			Assert.Collection(fileNames,
				item => Assert.Equal("P-90-20-02-02-Parameterliste-BAi_31d2-000_V6_R1_fr_FR", item),
				item => Assert.Equal("P-90-30-02-02-Parameterliste-Gui_9fcc-001_V6_R1_fr_FR", item)
			);
		}

	}
}