using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
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