using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;
using TMX_Lib.XmlSplit;
using LogManager = Sdl.LanguagePlatform.TranslationMemory.LogManager;

namespace QuickTmxTesting
{
	// quick and dirty tests
    internal class Program
    {
	    private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

	    private static async Task TestImportLargeFile(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "large_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (EU Bookshop v2_10.8M).tmx");
	    }
		private static async Task TestImportLargeFile2(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "large2_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (DGT 2019_5.0M).tmx");
	    }

		private static async Task TestImportSmallFile(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "small_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\cy-GB to en-US.tmx");
	    }
		private static async Task TestImportSmallFile2(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "small2_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\#2 - TUs with a different single field.tmx");
	    }
		private static async Task TestImportMultilingual(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "multilingual_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\multilingual\\4 - multilingual_TMX.tmx");
	    }
		private static async Task TestImportMultilingual2(string root)
	    {
		    var db = new TmxMongoDb("localhost:27017", "multilingual_big_db");
		    await db.ImportToDbAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\multilingual\\ecdc.tmx");
	    }
		private static async Task TestImportSample4(string root)
		{
			var db = new TmxMongoDb("localhost:27017", "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			var segment = new Segment();
			segment.Add("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.");
			var result = await search.Search(TmxSearchSettings.Default(), segment, new LanguagePair("en-GB","es-MX"));
			Debug.Assert(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.");

			segment = new Segment();
			segment.Add("En el cumplimiento de estas metas, lograr nuestra visión de ser el socio de confianza para todos aquellos con quienes tenemos una relación, accionistas, clientes, empleados, proveedores, miembros de la comunidad en la que estamos trabajando, o de cualquier otro grupo o persona.");
			result = await search.Search(TmxSearchSettings.Default(), segment, new LanguagePair("es-MX", "en-GB"));
			Debug.Assert(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.");
		}

		// performs the database fuzzy-search, not our Fuzzy-search (our fuzzy search is more constraining)
		private static async Task TestDatabaseFuzzySimple4(string root)
		{
			var db = new TmxMongoDb("localhost:27017", "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			var result = await db.FuzzySearch("abc def", "en-GB", "es-MX");
			Debug.Assert(result.Count == 0);

			result = await db.FuzzySearch("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.", "en-GB", "es-MX");
			Debug.WriteLine($"best score:{result[0].Score}");
			result = await db.FuzzySearch("En el cumplimiento de estas metas, lograr nuestra visión de ser el socio de confianza para todos aquellos con quienes tenemos una relación, accionistas, clientes, empleados, proveedores, miembros de la comunidad en la que estamos trabajando, o de cualquier otro grupo o persona.", "es-MX", "en-GB");
			Debug.WriteLine($"best score:{result[0].Score}");

			result = await db.FuzzySearch("This document Interserve Health Safety", "en-GB", "es-MX");
			Debug.Assert(result.Count >= 3 && result.Any(r => r.TargetText == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.") );

			result = await db.FuzzySearch("construction subcontractors", "en-GB", "es-MX");
			Debug.Assert(result.Count >= 3 
			&& result.Any(r => r.SourceText == "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
			&& result.Any(r => r.SourceText == "These codes have been prepared to ensure that Interserve Construction and all subcontractors on Interserve Construction contracts operate to clear and consistent standards and in doing so assist us in meeting our goals of being accident free and reducing our environmental impact.")
			&& result.Any(r => r.SourceText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
			&& result.All(r => r.SourceText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);

			result = await db.FuzzySearch("construction health", "en-GB", "es-MX");
			Debug.Assert(result.Count >= 2
			             && result.Any(r => r.SourceText == "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
			             && result.Any(r => r.SourceText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
			             && result.All(r => r.SourceText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);

			result = await db.FuzzySearch("construcción salud", "es-MX", "en-GB");
			Debug.Assert(result.Count >= 2
			             && result.Any(r => r.TargetText== "This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.")
			             && result.Any(r => r.TargetText == "Subcontractors are required to assist and co-operate with Interserve Construction with health, safety and environmental related issues, including initiatives that may be operated from time to time.")
			             && result.All(r => r.TargetText != "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.")
			);
		}

		private static Segment TextToSegment(string text)
		{
			var s = new Segment();
			s.Add(text);
			return s;
		}

		private static async Task TestFuzzySimple4(string root)
		{
			var db = new TmxMongoDb("localhost:27017", "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			var fuzzy = TmxSearchSettings.Default();
			fuzzy.Mode = SearchMode.FuzzySearch;
			var en_sp = new LanguagePair("en-GB", "es-MX");
			var sp_en = new LanguagePair("es-MX", "en-GB");
			var results = await search.Search(fuzzy, TextToSegment("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code"), en_sp);
			results = await search.Search(fuzzy, TextToSegment("This document contains both Interserve Construction Health Safety Code for Subcontractors and Sustainability Code"), en_sp);
			results = await search.Search(fuzzy, TextToSegment("This document contains both Interserve Construction Safety Code for and Code"), en_sp);
		}


		private static void SplitLargeXmlFile(string inputXmlFile, string outputPrefix)
		{
			Directory.CreateDirectory(outputPrefix);
		    var splitter = new XmlSplitter(inputXmlFile);
		    var idx = 0;
		    while (true)
		    {
			    var str = splitter.TryGetNextString();
			    if (str == null)
				    return;
			    var outFile = $"{outputPrefix}{++idx:D3}.xml";
				File.WriteAllText(outFile, str);
		    }
	    }

		static void Main(string[] args)
		{
			LogUtil.Setup();
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en(GB) - it(IT)_(DGT 2015, 2017).tmx", "C:\\john\\buff\\TMX Examples\\temp\\");
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (EU Bookshop v2_10.8M).tmx", "C:\\john\\buff\\TMX Examples\\temp2\\");
			log.Debug("test started");
			SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\fails\\opensubtitlingformat.tmx", "C:\\john\\buff\\TMX Examples\\temp3\\");


			var root = ".";
	        if (args.Length > 0)
		        root = args[0];

			//Task.Run(() => TestImportSmallFile2(root)).Wait();
			//Task.Run(() => TestImportSample4(root)).Wait();
			//Task.Run(() => TestDatabaseFuzzySimple4(root)).Wait();

			//Task.Run(() => TestFuzzySimple4(root)).Wait();

			log.Debug("test complete");
	        Console.ReadLine();
        }
    }
}
