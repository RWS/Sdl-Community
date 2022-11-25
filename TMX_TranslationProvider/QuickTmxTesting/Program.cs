using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;
using TMX_Lib.XmlSplit;

namespace QuickTmxTesting
{
	// quick and dirty tests
    internal class Program
    {
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
			var result = await search.Search(new SearchSettings(), segment, new LanguagePair("en-GB","es-MX"));
			Debug.Assert(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.");

			segment = new Segment();
			segment.Add("En el cumplimiento de estas metas, lograr nuestra visión de ser el socio de confianza para todos aquellos con quienes tenemos una relación, accionistas, clientes, empleados, proveedores, miembros de la comunidad en la que estamos trabajando, o de cualquier otro grupo o persona.");
			result = await search.Search(new SearchSettings(), segment, new LanguagePair("es-MX", "en-GB"));
			Debug.Assert(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.");
		}

		private static void SplitLargeXmlFile(string inputXmlFile, string outputPrefix)
	    {
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
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en(GB) - it(IT)_(DGT 2015, 2017).tmx", "C:\\john\\buff\\TMX Examples\\temp\\");
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (EU Bookshop v2_10.8M).tmx", "C:\\john\\buff\\TMX Examples\\temp2\\");
			Console.WriteLine("test started");

			var root = ".";
	        if (args.Length > 0)
		        root = args[0];

	        //Task.Run(() => TestImportSmallFile2(root)).Wait();
	        Task.Run(() => TestImportSample4(root)).Wait();
	        Console.ReadLine();
        }
    }
}
