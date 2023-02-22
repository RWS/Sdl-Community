using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Fluent;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;
using TMX_Lib.Utils;
using TMX_Lib.Writer;

namespace TMXTests
{
	public class TestMongoDb
	{
		[Fact]
		public async Task TestSample4()
		{
			var root = "..\\..\\..\\..";
			var db = new TmxMongoDb( "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			var segment = new Segment();
			segment.Add("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.");
			var result = await search.Search(TmxSearchSettings.Default(), segment, new LanguagePair("en-GB", "es-MX"));
			Assert.True(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.");

			segment = new Segment();
			segment.Add("En el cumplimiento de estas metas, lograr nuestra visión de ser el socio de confianza para todos aquellos con quienes tenemos una relación, accionistas, clientes, empleados, proveedores, miembros de la comunidad en la que estamos trabajando, o de cualquier otro grupo o persona.");
			result = await search.Search(TmxSearchSettings.Default(), segment, new LanguagePair("es-MX", "en-GB"));
			Assert.True(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.");
		}

		// performs the database fuzzy-search, not our Fuzzy-search (our fuzzy search is more constraining)
		[Fact]
		public async Task TestDatabaseFuzzySimple4()
		{
			var root = "..\\..\\..\\..";
			var db = new TmxMongoDb( "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			var result = await db.FuzzySearch("abc def", "en-GB", "es-MX");
			Assert.True(result.Count == 0);

			result = await db.FuzzySearch("This document Interserve Health Safety", "en-GB", "es-MX");
			Assert.True(result.Count >= 3 && result.Any(r => r.TargetText == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas."));

			result = await db.FuzzySearch("construction subcontractors", "en-GB", "es-MX");
			Assert.True(result.Count >= 3
			&& result.Any(r => r.SourceText == "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
			&& result.Any(r => r.SourceText == "These codes have been prepared to ensure that Interserve Construction and all subcontractors on Interserve Construction contracts operate to clear and consistent standards and in doing so assist us in meeting our goals of being accident free and reducing our environmental impact.")
			&& result.Any(r => r.SourceText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
			&& result.All(r => r.SourceText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);

			result = await db.FuzzySearch("construction health", "en-GB", "es-MX");
			Assert.True(result.Count >= 2
						 && result.Any(r => r.SourceText == "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
						 && result.Any(r => r.SourceText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
						 && result.All(r => r.SourceText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);

			result = await db.FuzzySearch("construcción salud", "es-MX", "en-GB");
			Assert.True(result.Count >= 2
						 && result.Any(r => r.TargetText == "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
						 && result.Any(r => r.TargetText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
						 && result.All(r => r.TargetText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);
		}


		private static async Task DbImportFileAsync(string file, string dbName, bool quickImport = false)
		{
			var db = new TmxMongoDb( dbName);
			await db.ImportToDbAsync(file, (r) => { }, quickImport);
		}
		private static (TranslationUnit tu, LanguagePair lp) SimpleTU(string sourceText, string targetText, string sourceLanguage, string targetLanguage, ulong id = 0)
		{
			LanguagePair lp = new LanguagePair(sourceLanguage, targetLanguage);

			var source = new Segment(lp.SourceCulture);
			source.Add(sourceText);
			var target = new Segment(lp.TargetCulture);
			target.Add(targetText);
			var tu = new TranslationUnit
			{
				SourceSegment = source,
				TargetSegment = target,
			};

			tu.ResourceId = new PersistentObjectToken((int)id, Guid.Empty);
			tu.Origin = TranslationUnitOrigin.TM;
			return (tu, lp);
		}
		[Fact]
		public async Task TestAddAndUpdateTUs()
		{
			// the idea - #4 is almost empty (very very small) - thus, easy to test adding/updating
			var root = "..\\..\\..\\..";
			await DbImportFileAsync($"{root}\\SampleTestFiles\\#4.tmx", "add-update");
			var db = new TmxMongoDb( "add-update");
			await db.InitAsync();
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			var (tu, lp) = SimpleTU("this is an interesting recipe", "asta e o reteta interesanta", "en-US", "ro-RO");
			var tuID = await search.AddAsync(tu, lp);
			(tu, lp) = SimpleTU("this is an amazingly interesting recipe", "asta e o reteta super interesanta", "en-US", "ro-RO", tuID);
			await search.UpdateAsync(tu, lp);
			// here, I'm actually adding a new language (spanish)
			(tu, lp) = SimpleTU("this is an amazingly interesting recipe", "esta muchacha con tigo", "en-US", "es-ES", tuID);
			await search.UpdateAsync(tu, lp);

			var dbTU = await db.FindTranslationUnitAsync(tuID);
			var dbTexts = await db.FindTextsAsync(tuID);
			Assert.True(dbTU.NormalizedLanguages.Count == 3 && dbTexts.Count == 3);
			Assert.True(dbTU.NormalizedLanguages.Contains("en-us"));
			Assert.True(dbTU.NormalizedLanguages.Contains("ro-ro"));
			Assert.True(dbTU.NormalizedLanguages.Contains("es-es"));
			Assert.True(dbTexts.Any(t => t.LocaseText == "this is an amazingly interesting recipe"));
			Assert.True(dbTexts.Any(t => t.LocaseText == "asta e o reteta super interesanta"));
			Assert.True(dbTexts.Any(t => t.LocaseText == "esta muchacha con tigo"));
		}

		[Fact]
		public async Task TestExportToXml()
		{
			var folder = "..\\..\\..\\..\\SampleTestFiles\\";
			var originalFile = "#4";
			await DbImportFileAsync($"{folder}{originalFile}.tmx", "sample4");
			var db = new TmxMongoDb("sample4");
			await db.InitAsync();
			var writer = new TmxWriter($"{folder}{originalFile}-copy.tmx") { Indent = true };
			await writer.WriteAsync(db);
			writer.Dispose();

			var oldContent = File.ReadAllText($"{folder}{originalFile}.tmx");
			var newContent = File.ReadAllText($"{folder}{originalFile}-copy.tmx");
			Assert.True(Util.SimpleIsXmlEquivalent(oldContent, newContent));
		}

	}
}
