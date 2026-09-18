using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class PackageServiceUnitTests
	{
		private readonly IPackageService _service = new PackageService();

		[Theory]
		[InlineData(@"File1=1|1|1|0|1|Y:\projects\000102\_vorb\Translation_Tat_IDN.xml|Translation_Tat_IDN|Translation_Tat_IDN|0", "Translation_Tat_IDN")]
		[InlineData(@"File1=1|1|1|0|1|C:\Transit NXT\projects\Probeübersetzung\Probeübersetzung.ttc|Probeübersetzung_ttc|Probeübersetzung ttc|0", "Probeübersetzung_ttc")]
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