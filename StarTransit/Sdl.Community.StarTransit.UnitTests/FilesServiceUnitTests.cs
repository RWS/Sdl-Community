using System;
using System.IO;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class FilesServiceUnitTests
	{
		private readonly string _testingFilesPath;
		private readonly string _tmFilePath;
		private readonly string _transitFilePath;
		private readonly IFileService _fileService;

		public FilesServiceUnitTests()
		{
			_fileService = new FileService();
			_testingFilesPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles");
			_tmFilePath = Path.Combine(_testingFilesPath, "_AEXTR_1.DEU");
			_transitFilePath = Path.Combine(_testingFilesPath, "FnrTranslationTat_Amplexor_FB_TRANSLAT_IDN.DEU");
		}

		[Fact]
		public void IsTmFile_ReturnsTrue()
		{
			var isTm = _fileService.IsTransitTm(_tmFilePath);
			Assert.True(isTm);
		}

		[Fact]
		public void IsTmFile_ReturnsFalse()
		{
			var isTm = _fileService.IsTransitTm(_transitFilePath);
			Assert.False(isTm);
		}

		[Fact]
		public void IsTmFile_FileDoesNotExist_ReturnsFalse()
		{
			var isTm = _fileService.IsTransitTm(Path.Combine(_testingFilesPath, "_AEXTR_2.DEU"));
			Assert.False(isTm);
		}

		[Fact]
		public void IsTransitFile_RetunsTrue()
		{
			var isTransitFile = _fileService.IsTransitFile(_transitFilePath);
			Assert.True(isTransitFile);
		}

		[Fact]
		public void IsTransitFile_ReturnsTrue()
		{
			var isTransitFile = _fileService.IsTransitFile(_tmFilePath);
			Assert.True(isTransitFile);
		}

		[Theory]
		[InlineData("<Seg SegID=\"7\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\P-50-80-09-Technische-Daten-TD-F_3698d3fa_V1_R1_en_GB\uEF030\uEF0420210222\uEF05121026\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD348\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"UnknownStruct\">&lt;/title&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;tgroup cols=&quot;2&quot; y.validity.allowed=&quot;true&quot; y.id=&quot;ID_9f64268130c3e20e354ae3657f9fd0f6&quot; y.validity.mode=&quot;positive&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;colspec colnum=&quot;1&quot; colname=&quot;col1&quot; colwidth=&quot;2*&quot;/&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;colspec colnum=&quot;2&quot; colname=&quot;col2&quot; colwidth=&quot;1*&quot; align=&quot;center&quot;/&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;thead&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableRow\">&lt;row&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col1&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;32&quot; y.id=&quot;ID_e59a6757878c11e49047f5eca7845092&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"1\" Data=\"\uE8C0\uEE02\uEF02R:\\Transit_NXT\\Trumpf\\id\\TruTops und Co\\346855001_id\uEF030\uEF0420160307\uEF05100413\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u4013\u4000\u4000\uEFD364\"><Tag pos=\"Begin\" type=\"Paragraph\">Paragraph</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"1\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\P-50-80-09-Technische-Daten-TD-F_3698d3fa_V1_R1_en_GB\uEF030\uEF0420210222\uEF05121026\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD342\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"Paragraph\">&lt;/p&gt;</Tag> <Tag pos=\"End\" type=\"ListItem\">&lt;/li&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"ListItem\">&lt;li _TR=&quot;22&quot; y.id=&quot;ID_f5e8fbc03007d533354ae36554f03acd&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;23&quot; y.id=&quot;ID_2dddd96863037e37354ae365475fd0e0&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"192\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00195009_P-50-80-09-Technische-Daten-TD-F_3698d3fa_V9_R1_en_GB\uEF030\uEF0420210222\uEF05121026\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD38\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"Paragraph\">&lt;/p&gt;</Tag> <Tag pos=\"End\" type=\"ListItem\">&lt;/li&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"ListItem\">&lt;li _TR=&quot;21&quot; y.id=&quot;ID_f5e8fbc03007d533354ae36554f03acd&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;22&quot; y.id=&quot;ID_2dddd96863037e37354ae365475fd0e0&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"347\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00206500_P-50-80-07-TD-Gewichte-2130_4a95ffe2_V6_R1_en_GB\uEF030\uEF0420210222\uEF05121026\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD332\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"Paragraph\">&lt;/p&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col2&quot;&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"End\" type=\"TableRow\">&lt;/row&gt;</Tag> <Tag pos=\"End\" type=\"UnknownStruct\">&lt;/tbody&gt;</Tag> <Tag pos=\"End\" type=\"UnknownStruct\">&lt;/tgroup&gt;</Tag> <Tag pos=\"End\" type=\"Table\">&lt;/table&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!----&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!--JP--&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!----&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Table\">&lt;table y.validity.allowed=&quot;true&quot; y.id=&quot;ID_c0ea531c181911e6a71df084dcb411d0&quot; width=&quot;auto&quot; y.validity.ids=&quot;5a8dc377c19ed205354ae35e0176f832&quot; y.validity.mode=&quot;positive&quot; y.validity.text=&quot;MA-JP&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;title _TR=&quot;36&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"4565\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00429261_P-50-80-10-TD-AHV-Stuetzlast-247_7445-01B_V4_R1_ja_JP\uEF030\uEF0420210222\uEF05121030\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD324\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"Paragraph\">&lt;/p&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col3&quot;&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"End\" type=\"TableRow\">&lt;/row&gt;</Tag> <Tag pos=\"End\" type=\"UnknownStruct\">&lt;/tbody&gt;</Tag> <Tag pos=\"End\" type=\"UnknownStruct\">&lt;/tgroup&gt;</Tag> <Tag pos=\"End\" type=\"Table\">&lt;/table&gt;</Tag> <Tag pos=\"End\" type=\"UnknownStruct\">&lt;/section&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!----&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!----&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!--Hybrid////////////////////////////////////////////////////////////////////////--&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;section _TR=&quot;49&quot; y.id=&quot;ID_885b19ee0df111e6bc5785cb25a1aa04&quot; y.validity.text=&quot;ZY_PlugInHybrid_L&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;50&quot; y.id=&quot;ID_7d075b5f16a0ba81ac190d2b53a2539a&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"10570\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00466582_P-50-80-10-TD-AHV-Stuetzlast-MFA_3c57-007_V1_R1_en_GB\uEF030\uEF0420210222\uEF05121035\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD314\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"Paragraph\">&lt;/p&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col3&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;!--\u00C4J20_2 Modell fehlt in CarSpecs--&gt;</Tag> <Tag pos=\"End\" type=\"TableCell\">&lt;/entry&gt;</Tag> <Tag pos=\"End\" type=\"TableRow\">&lt;/row&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableRow\">&lt;row&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col1&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;32&quot; y.id=&quot;ID_f55d91ce9ba111e89aa9c29d192dd23b&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		[InlineData("<Seg SegID=\"10571\" Data=\"\uE8C0\uE90F\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00466582_P-50-80-10-TD-AHV-Stuetzlast-AMG_f571-005_V1_R1_en_GB\uEF030\uEF0420210222\uEF05121035\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD38\"><Tag pos=\"Point\" type=\"Unknown\">&lt;/ContentTranslate&gt;</Tag><Tag pos=\"End\" type=\"UnknownStruct\">&lt;/title&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;tgroup cols=&quot;2&quot; y.validity.allowed=&quot;true&quot; y.id=&quot;ID_033ce005e35011e8932198d6bd5610c8&quot; y.validity.mode=&quot;positive&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;colspec colnum=&quot;1&quot; colname=&quot;col1&quot; colwidth=&quot;1.00*&quot; /&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;colspec colname=&quot;col3&quot; colnum=&quot;2&quot; colwidth=&quot;1.00*&quot; align=&quot;center&quot; /&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"UnknownStruct\">&lt;thead&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableRow\">&lt;row&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"TableCell\">&lt;entry colname=&quot;col1&quot;&gt;</Tag> <Tag pos=\"Begin\" i=\"1\" type=\"Paragraph\">&lt;p _TR=&quot;29&quot; y.id=&quot;ID_033ce006e35011e8932198d6bd5610c8&quot;&gt;</Tag><Tag pos=\"Point\" type=\"Unknown\">&lt;ContentTranslate&gt;</Tag></Seg>\r\n")]
		public void IsValidNode_ReturnsFalse(string nodeContent)
		{
			var doc = GetXmlDocument(nodeContent);
			var node = doc.SelectNodes("//Seg")?[0];
			var isValid = _fileService.IsValidNode(node);
			Assert.False(isValid);
		}

		[Theory]
		[InlineData("<Seg SegID=\"16\" Data=\"\uE90A\uEE02\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\Refextr_ohne_Redundanzen_NXT_Urladung_P_ENG2\uEF030\uEF0420150929\uEF05144121\uEF06?0000000000\uEF0720150921\uEF08125407\uEF09?000000000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD3316\uEFD6\u4002\"><Tag pos=\"Begin\" i=\"1\" x=\"1\" type=\"Inline\">&lt;target&gt;</Tag>Vehicles without a controller: <Tag pos=\"End\" i=\"1\" x=\"2\" type=\"Inline\">&lt;/target&gt;</Tag>swipe up or down on the touchpad. </Seg>\r\n")]
		[InlineData("<Seg SegID=\"9\" Data=\"\uE90E\uEA0B\uEB64\uEC01\uED14\uEE01\uF508\uEF02\\\\53.31.16.107\\e$\\PRD\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI\\P\\61fee74b-63e0-4726-b72d-cd83a8a9eb2d_P-31-Fahrzeugdaten-G-500-Guard-G-Guard-463_7fe99413_2_en_GB\uEF030\uEF0420150323\uEF05143655\uEF06?000\uEF0720080415\uEF08110512\uEF09?000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD2\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\P-50-80-09-Technische-Daten-TD-F_3698d3fa_V1_R1_en_GB\uEFD363\uEFD6\u4004\">2065 mm </Seg>\r\n")]
		[InlineData("<Seg SegID=\"930\" Data=\"\uE90E\uEA0D\uEB64\uED10\uEE01\uF602\uF50A\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00295611_P-50-80-07-TD-Gewichte-222_c715-002_V6_R1_en_GB\uEF030\uEF0420190121\uEF05160208\uEF06?00000000000000\uEF0720171106\uEF08162642\uEF09?0000000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD2\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00376420_P-50-80-09-TD-Masse-1907_ec33-004_V3_R1_en_GB\uEFD33\uEFD6\u401C\">The values have been measured and checked in accordance with GB<Tag pos=\"Point\" x=\"1\" type=\"InlineDel\">&lt;nbsp /&gt;</Tag>1589<Tag pos=\"Point\" x=\"2\" type=\"InlineDel\">&lt;nbhy /&gt;</Tag>2016 &quot;Limits of dimensions, axle load and masses for motor vehicles, trailers and vehicles combination&quot;. </Seg>\r\n")]
		[InlineData("<Seg SegID=\"1525\" Data=\"\uE90E\uEA0D\uEB64\uED10\uEE01\uF603\uF50A\uEF02\\\\53.31.16.107\\e$\\PRD\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00185989_P-50-80-02-00-TD-AHV-Anhaengelas_5054e608_V7_R1_en_GB\uEF030\uEF0420190918\uEF05122147\uEF06?00000000000000\uEF0720160201\uEF08181258\uEF09?00000000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD2\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00421552_P-50-80-02-00-TD-Anh-last-2539_650d-01B_V3_R1_en_GB\uEFD3121\uEFD6\u401C\"><Tag pos=\"Begin\" i=\"1\" x=\"1\" type=\"Inline\">&lt;v&gt;</Tag><Tag pos=\"Begin\" i=\"2\" x=\"2\" type=\"Inline\">&lt;m&gt;</Tag>2000 kg<Tag pos=\"End\" i=\"2\" type=\"Inline\">&lt;/m&gt;</Tag><Tag pos=\"Begin\" i=\"3\" x=\"3\" type=\"UnknownStruct\">&lt;nm /&gt;</Tag><Tag pos=\"End\" i=\"1\" type=\"Inline\">&lt;/v&gt;</Tag> </Seg>\r\n")]
		[InlineData("<Seg SegID=\"1959\" Data=\"\uE90F\uEA0D\uEB64\uED10\uEE01\uF603\uF50A\uEF02\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00269668_P-39-15-01-TD-Kaeltemittel-2132_9910-001_V3_R1_en_GB\uEF030\uEF0420191009\uEF05100942\uEF06?00000\uEF0720150929\uEF08154049\uEF09?0000000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD2\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00424351_P-39-15-01-TD-Kaeltemittel-2539_ff49-00E_V4_R1_en_GB\uEFD345\uEFD6\u401C\"><Tag pos=\"Begin\" i=\"1\" x=\"1\" type=\"Inline\">&lt;v&gt;</Tag><Tag pos=\"Begin\" i=\"2\" x=\"2\" type=\"Inline\">&lt;m&gt;</Tag>80 \u00B1 10 g<Tag pos=\"End\" i=\"2\" type=\"Inline\">&lt;/m&gt;</Tag> <Tag pos=\"Begin\" i=\"3\" x=\"3\" type=\"Inline\">&lt;nm&gt;</Tag>2.8 \u00B1 0.4 oz<Tag pos=\"End\" i=\"3\" type=\"Inline\">&lt;/nm&gt;</Tag><Tag pos=\"End\" i=\"1\" type=\"Inline\">&lt;/v&gt;</Tag> </Seg>\r\n")]
		[InlineData("<Seg SegID=\"358\" Data=\"\uE90E\uEA0D\uEB64\uED10\uEE01\uF50A\uEF02\\\\53.31.16.107\\e$\\PRD\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\P-50-80-01-TD-AHV-Achslast_7a1251f8_V1_R1_en_GB\uEF030\uEF0420160602\uEF05140908\uEF06?00000000000000\uEF0720110930\uEF08103923\uEF09?0000000000000\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD2\\\\sedcaclm0110\\RefMat\\equi\\DEU_XXX_ENG_XXX\\KI\\ARKI2\\P\\U00206511_P-50-80-01-TD-AHV-Achslast-238-3_62a4f678_V1_R1_en_GB\uEFD39\uEFD6\u401C\">Axle load </Seg>\r\n")]
		public void IsValidNode_ReturnsTrue(string nodeContent)
		{
			var doc = GetXmlDocument(nodeContent);
			var node = doc.SelectNodes("//Seg")?[0];
			var isValid = _fileService.IsValidNode(node);
			Assert.True(isValid);
		}

		private XmlDocument GetXmlDocument(string content)
		{
			var doc = new XmlDocument();
			doc.LoadXml(content);
			return doc;
		}
	}
}
