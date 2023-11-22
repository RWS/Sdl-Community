using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using NLog.Targets;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;
using TMX_Lib.Writer;
using TMX_Lib.XmlSplit;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using LogManager = Sdl.LanguagePlatform.TranslationMemory.LogManager;

namespace QuickTmxTesting
{
	// quick and dirty tests
    internal class Program
    {
	    private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

	    private static async Task ImportFileAsync(string file, string dbName, bool quickImport = false, int entryiesPerTextTable = TmxMongoDb.DEFAULT_ENTRIES_PER_TEXT_TABLE, int maxImportTUCount = -1)
	    {
		    var db = new TmxMongoDb(dbName);
		    await db.ImportToDbAsync(file, (r) =>
		    {
				log.Debug($"report: read {r.TUsRead}, ignored {r.TUsWithSyntaxErrors}, success={r.TUsImportedSuccessfully}, invalid={r.TUsWithInvalidChars}, spent={r.ReportTimeSecs} secs");
		    }, quickImport, entryiesPerTextTable, maxImportTUCount);
	    }
		private static async Task ImportFilesAsync(IReadOnlyList< string> files, string dbName)
		{
			var db = new TmxMongoDb(dbName);
			foreach (var file in files)
				await db.ImportToDbAsync(file, (r) =>
				{
					log.Debug($"report: read {r.TUsRead}, ignored {r.TUsWithSyntaxErrors}, success={r.TUsImportedSuccessfully}, invalid={r.TUsWithInvalidChars}, spent={r.ReportTimeSecs} secs");
				});
		}

		// performs the database fuzzy-search, not our Fuzzy-search (our fuzzy search is more constraining)
		private static async Task TestDatabaseFuzzySimple4(string root)
		{
			var db = new TmxMongoDb("sample4");
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
			var db = new TmxMongoDb( "sample4");
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

		public enum SearchType
		{
			Exact, Fuzzy, Concordance,
		}

		private static async Task TestDbSearch(string dbName, string text, SearchType searchType, string sourceLanguage, string targetLanguage)
		{
			var db = new TmxMongoDb( dbName);
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			log.Debug($"search [{text}] - started");
			var watch = Stopwatch.StartNew();
			IReadOnlyList<TmxSegment> result;
			switch (searchType)
			{
				case SearchType.Exact:
					result = await db.ExactSearch(text, sourceLanguage, targetLanguage);
					break;
				case SearchType.Fuzzy:
					result = await db.FuzzySearch(text, sourceLanguage, targetLanguage);
					break;
				case SearchType.Concordance:
					result = await db.ConcordanceSearch(text, sourceLanguage, targetLanguage);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
			}
			log.Debug($"search [{text}] - {result.Count} results - took {watch.ElapsedMilliseconds} ms");
		}

		private static async Task TestSearcherSearch(string dbName, string text, SearchType searchType, string sourceLanguage, string targetLanguage)
		{
			var db = new TmxMongoDb( dbName);
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			log.Debug($"search [{text}] - started");
			var watch = Stopwatch.StartNew();
			var settings = TmxSearchSettings.Default();
			switch (searchType)
			{
				case SearchType.Exact:
					settings.Mode = SearchMode.ExactSearch;
					break;
				case SearchType.Fuzzy:
					settings.Mode = SearchMode.FuzzySearch;
					break;
				case SearchType.Concordance:
					settings.Mode = SearchMode.ConcordanceSearch;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
			}

			var results = await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
			log.Debug($"search [{text}] - {results.Count} results - took {watch.ElapsedMilliseconds} ms");
			foreach (var result in results.Results)
			{
				log.Debug($"    score={result.ScoringResult.BaseScore}");
				log.Debug($"    {result.TranslationProposal.SourceSegment.ToPlain()}");
				log.Debug($"    {result.TranslationProposal.TargetSegment.ToPlain()}");
				log.Debug("");
			}
		}
		private static async Task TestSearcherSearch(string dbName, IReadOnlyList<string> texts, SearchType searchType, string sourceLanguage, string targetLanguage) {
			foreach (var text in texts)
				await TestSearcherSearch(dbName, text, searchType, sourceLanguage, targetLanguage);
		}

		private static string Expand(string text, int size)
		{
			var expandSize = size - text.Length;
			return text + new string(' ', expandSize);
		}
		private static string AtMost(string text, int size) { 
			if (text.Length > size)
				return text.Substring(0,size - 3) + "...";
			else 
				return Expand(text, size);
		}

		private static async Task TestSearcherSearchDumpResults(string dbName, IReadOnlyList<string> texts, string sourceLanguage, string targetLanguage, bool careForLocale = false)
		{
			await TestSearcherSearchDumpResults(new[] { dbName }, texts, sourceLanguage, targetLanguage, careForLocale);
		}
		private static async Task TestSearcherSearchDumpResults(IReadOnlyList< string> dbNames, IReadOnlyList<string> texts, string sourceLanguage, string targetLanguage, bool careForLocale = false) {
			var databases = dbNames.Select(dbName => new TmxMongoDb( dbName) { LogSearches = false}).ToList();
			foreach (var db in databases)
				await db.InitAsync();
			var search = new TmxSearch(databases);
			await search.LoadLanguagesAsync();

			log.Debug($"search started {string.Join(",",dbNames)}");
			var watchAll = Stopwatch.StartNew();
			var settings = TmxSearchSettings.Default();
			foreach (var text in texts) {
				var trimmedText = AtMost(text, 40);
				var watch = Stopwatch.StartNew();

				settings.Mode = SearchMode.ExactSearch;
				var results = await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage), careForLocale);
				var ellapsedExact = watch.ElapsedMilliseconds;
				log.Debug($"search exact [{trimmedText}] - {results.Count} results - took {watch.ElapsedMilliseconds} ms");

				watch = Stopwatch.StartNew();
				settings.Mode = SearchMode.FuzzySearch;
				results = await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage), careForLocale);
				var ellapsedFuzzy = watch.ElapsedMilliseconds;
				log.Debug($"search fuzzy [{trimmedText}] - {results.Count} results - took {watch.ElapsedMilliseconds} ms");

				watch = Stopwatch.StartNew();
				settings.Mode = SearchMode.NormalSearch;
				results = await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage), careForLocale);
				var ellapsedNormal = watch.ElapsedMilliseconds;
				var percent = (int)((1d - (double)ellapsedNormal / ((double)ellapsedExact + (double)ellapsedFuzzy)) * 100);
				log.Debug($"search norm  [{trimmedText}] - {results.Count} results - took {watch.ElapsedMilliseconds} ms, improve={percent}%");
			}
			log.Debug($"search complete- took {watchAll.ElapsedMilliseconds} ms");
		}

		private class TestResults {
			public List<long> Normal = new List<long>();
			public List<long> Exact = new List<long>();
			public List<long> Fuzzy = new List<long>();

			public long NormalAvg => (long)Normal.Average();
			public long NormalMin => (long)Normal.Min();
			public long NormalMax => (long)Normal.Max();

			public long ExactAvg => (long)Exact.Average();
			public long ExactMin => (long)Exact.Min();
			public long ExactMax => (long)Exact.Max();

			public long FuzzyAvg => (long)Fuzzy.Average();
			public long FuzzyMin => (long)Fuzzy.Min();
			public long FuzzyMax => (long)Fuzzy.Max();
		}

		private static async Task TestAvgExactAndFuzzySearcherSearch(string dbName, IReadOnlyList<string> texts, string sourceLanguage, string targetLanguage, int ignoreTimes, int runTimes)
		{
			var db = new TmxMongoDb( dbName) { LogSearches = false };
			await db.InitAsync();
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			log.Debug($"search started {dbName}");
			var settings = TmxSearchSettings.Default();

			for (int i = 0; i < ignoreTimes; ++i)
			{
				for (var idx = 0; idx < texts.Count; idx++)
				{
					Console.Write($"Ignore {i + 1}/{ignoreTimes}, Text {idx+1}/{texts.Count}       \r");
					var text = texts[idx];
					settings.Mode = SearchMode.ExactSearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));

					settings.Mode = SearchMode.FuzzySearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
				}
			}

			var results = new TestResults[texts.Count];
			for (int i = 0; i < results.Length; ++i)
				results[i] = new TestResults();

			var watchAll = Stopwatch.StartNew();

			for (int runIndex = 0; runIndex < runTimes; ++runIndex) 
			{
				for (var idx = 0; idx < texts.Count; idx++)
				{
					Console.Write($"Test {runIndex + 1}/{runTimes}, Text {idx + 1}/{texts.Count}      \r");
					var text = texts[idx];
					var watch = Stopwatch.StartNew();
					settings.Mode = SearchMode.ExactSearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
					results[idx].Exact.Add(watch.ElapsedMilliseconds);

					settings.Mode = SearchMode.FuzzySearch;
					watch = Stopwatch.StartNew();
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
					results[idx].Fuzzy.Add(watch.ElapsedMilliseconds);
				}
			}

			Console.WriteLine($"\r\n\r\n*** Database {dbName}: Results");
			for (int i = 0; i < results.Length; ++i)
			{
				var result = results[i];
				Console.WriteLine($"Text {i + 1:D2}: Exact  Avg={result.ExactAvg:D5}, Min={result.ExactMin:D5} Max={result.ExactMax:D5} | Fuzzy Avg = {result.FuzzyAvg:D5}, Min ={result.FuzzyMin:D5} Max ={result.FuzzyMax:D5}");
			}
			var sum = results.Sum(x => x.ExactAvg) + results.Sum(x => x.FuzzyAvg);
			var avg = results.Average(x => x.ExactAvg + x.FuzzyAvg);
			Console.WriteLine($"search complete- took {watchAll.ElapsedMilliseconds} ms, Avg sum (E+F): {sum}, Avg avg (E+F): {avg}");
		}

		private static async Task TestAvgNormalSearcherSearch(string dbName, IReadOnlyList<string> texts, string sourceLanguage, string targetLanguage, int ignoreTimes, int runTimes)
		{
			var db = new TmxMongoDb( dbName) { LogSearches = false };
			await db.InitAsync();
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			log.Debug($"search started {dbName}");
			var settings = TmxSearchSettings.Default();

			for (int i = 0; i < ignoreTimes; ++i)
			{
				for (var idx = 0; idx < texts.Count; idx++)
				{
					Console.Write($"Ignore {i + 1}/{ignoreTimes}, Text {idx + 1}/{texts.Count}       \r");
					var text = texts[idx];
					settings.Mode = SearchMode.NormalSearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
				}
			}

			var results = new TestResults[texts.Count];
			for (int i = 0; i < results.Length; ++i)
				results[i] = new TestResults();

			var watchAll = Stopwatch.StartNew();

			for (int runIndex = 0; runIndex < runTimes; ++runIndex)
			{
				for (var idx = 0; idx < texts.Count; idx++)
				{
					Console.Write($"Test {runIndex + 1}/{runTimes}, Text {idx + 1}/{texts.Count}      \r");
					var text = texts[idx];
					var watch = Stopwatch.StartNew();
					settings.Mode = SearchMode.NormalSearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
					results[idx].Normal.Add(watch.ElapsedMilliseconds);
				}
			}

			Console.WriteLine($"\r\n\r\n*** Database {dbName}: Results");
			for (int i = 0; i < results.Length; ++i)
			{
				var result = results[i];
				Console.WriteLine($"Text {i + 1:D2}: Normal  Avg={result.NormalAvg:D5}, Min={result.NormalMin:D5} Max={result.NormalMax:D5} ");
			}
			var sum = results.Sum(x => x.NormalAvg);
			var avg = results.Average(x => x.NormalAvg);
			Console.WriteLine($"search complete- took {watchAll.ElapsedMilliseconds} ms, Avg sum (E+F): {sum}, Avg avg (E+F): {avg}");
		}

		// the idea - while warming up (after connecting to the database), tests take a lot longer than once the DB has loaded up all its indexes and so on
		// lets see how much time that is
		private static async Task TestWarmupTimesNormalSearcherSearch(string dbName, IReadOnlyList<string> texts, string sourceLanguage, string targetLanguage, int runTimes)
		{
			var db = new TmxMongoDb( dbName) { LogSearches = false };
			await db.InitAsync();
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			log.Debug($"search started {dbName}");
			var settings = TmxSearchSettings.Default();

			var results = new TestResults[texts.Count];
			for (int i = 0; i < results.Length; ++i)
				results[i] = new TestResults();

			var watchAll = Stopwatch.StartNew();

			for (int runIndex = 0; runIndex < runTimes; ++runIndex)
			{
				for (var idx = 0; idx < texts.Count; idx++)
				{
					Console.Write($"Test {runIndex + 1}/{runTimes}, Text {idx + 1}/{texts.Count}      \r");
					var text = texts[idx];
					var watch = Stopwatch.StartNew();
					settings.Mode = SearchMode.NormalSearch;
					await search.Search(settings, TextToSegment(text), new LanguagePair(sourceLanguage, targetLanguage));
					results[idx].Normal.Add(watch.ElapsedMilliseconds);
				}
			}

			Console.WriteLine($"\r\n\r\n*** Database {dbName}: Results");
			for (int i = 0; i < results.Length; ++i)
			{
				var result = results[i];
				Console.WriteLine($"Text {i + 1:D2}: Normal ={string.Join(" ", result.Normal.Select(n => n.ToString("D5")))}");
			}
			Console.WriteLine($"search complete- took {watchAll.ElapsedMilliseconds} ms");
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

		private static bool ContainsSource(SearchResults sr, string text)
		{
			return sr.Results.Any(r => r.TranslationProposal.SourceSegment.ToPlain() == text);
		}
		private static bool ContainsTarget(SearchResults sr, string text)
		{
			return sr.Results.Any(r => r.TranslationProposal.TargetSegment.ToPlain() == text);
		}

		private static async Task<bool> ExpectInSearch(string source, string target, TmxSearch search, string sourceLanguage, string targetLanguage)
		{
			var result = await search.Search(TmxSearchSettings.Default(), TextToSegment(source), new LanguagePair(sourceLanguage, targetLanguage));
			return (ContainsTarget(result, target));
		}

		private static async Task TestEnRoImport()
		{
			// run it after:
			// Task.Run(() => TestImportFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large2\\en-ro.tmx", "en-ro", quickImport: true)).Wait();
			var db = new TmxMongoDb( "en-ro");
			await db.InitAsync();
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();

			Debug.Assert(await ExpectInSearch(  "We might call them the words of \"unforgiveness.\"", 
												"Le putem numi cuvintele „ne-iertării”.", 
												search, "en-GB", "ro-RO"));
			Debug.Assert(await ExpectInSearch("When in doubt, tell the truth.",
				"„Când ai dubii, spune adevărul.”",
				search, "en-GB", "ro-RO"));
			Debug.Assert(await ExpectInSearch("Yes, even between the land and the ship.”",
				"Da, chiar şi între pământ şi navă\".",
				search, "en-GB", "ro-RO"));
			Debug.Assert(await ExpectInSearch("The Scriptures show that the first stage of our Lord's parousia, presence, will be secret",
				"Scripturile arată că prima etapă a parousiei sau prezenţei Domnului nostru va fi secretă.",
				search, "en-GB", "ro-RO"));
			Debug.Assert(await ExpectInSearch("I know the Three Kings do not exist but I give you this great present",
				"Stiu ca cei trei regi nu exista, dar iti ofer acest mare cadou.”",
				search, "en-GB", "ro-RO"));

			Debug.Assert(await ExpectInSearch("Stiu ca cei trei regi nu exista, dar iti ofer acest mare cadou",
				"I know the Three Kings do not exist but I give you this great present.”",
				search, "ro-ro", "en-US"));
			Debug.Assert(await ExpectInSearch("Ați fost învățați de cunoaștere că fericirea oamenilor depinde de ceea ce au creat cu propriile lor mâini",
				"Were you taught by knowledge that people’s happiness depended on what they created with their own hands?",
				search, "ro-ro", "en-US"));
			Debug.Assert(await ExpectInSearch("În trecut nu prea exista ocazie ca poporul Domnului să vegheze la împlinirea Scripturii; pentru că aceste împliniri erau departe unele de altele",
				"In the past there was little opportunity for the Lord's people to watch the fulfilments of Scripture; for these fulfilments were far apart.",
				search, "ro-ro", "en-US"));
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

		private static async Task TestExportToXml()
		{
			var folder = "C:\\john\\buff\\TMX Examples\\";
			var originalFile = "#4";
			await ImportFileAsync($"{folder}{originalFile}.tmx", "sample4");
			var db = new TmxMongoDb("sample4");
			await db.InitAsync();
			var writer = new TmxWriter($"{folder}{originalFile}-copy.tmx") { Indent = true };
			await writer.WriteAsync(db);
			writer.Dispose();

			var oldContent = File.ReadAllText($"{folder}{originalFile}.tmx");
			var newContent = File.ReadAllText($"{folder}{originalFile}-copy.tmx");
			Debug.Assert(Util.SimpleIsXmlEquivalent(oldContent, newContent));
		}

		static void Main(string[] args)
		{
			LogUtil.Setup( logToConsole: true);
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en(GB) - it(IT)_(DGT 2015, 2017).tmx", "C:\\john\\buff\\TMX Examples\\temp\\");
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (EU Bookshop v2_10.8M).tmx", "C:\\john\\buff\\TMX Examples\\temp2\\");
			log.Debug("test started");
			//var root = "C:\\john\\buff\\TMX Examples";
			//Task.Run(async () => await ImportFilesAsync(new[] { 
			//	$"{root}\\Banking TextBase.tmx" ,
			//	$"{root}\\EAC_FORMS.tmx" ,
			//	$"{root}\\Master TM cy(UK) - en(US)_00000_0000.tmx" ,
			//	$"{root}\\Master TM cy(UK) - en(US)_Trados2007.tmx" ,
			//	$"{root}\\002 - Glossary.tmx" ,
			//	$"{root}\\CAT Fight TM de-DE - en-US.tmx" ,
			//	$"{root}\\clean_Roman Weaponry.tmx" ,
			//}, "multi-files")).Wait();
			//Task.Run(async () => await ImportFilesAsync(new[] {
			//	$"{root}\\clean_Roman Weaponry.tmx" ,
			//}, "multi-files3")).Wait();

			var TEST_TEXTS = new[] {
				"The playing time was also reduced by half comparison to Rugby games",
				"This was the main expectation of the European partners of our mandate",
				"She listened to our fears and concerns with sympathy",
				"Christians often try to serve God and own desires",
				"Somebody said to Hudson You are a man of very great" ,

				"It is there at least on paper because from it has been actually active in a very visible way",
				"Furthermore, as the Swiss points out, account should be taken of the fact that some States bound by the Convention do not apply Directive",
				"He brought many factories to Macedonia as well as investments and new companies to the point that unemployment fell to 12% – this is progress.",
				"Broadcasters argue that the revenue in the EU every year by TV ads for children's products - between euros and 1 billion euros - is essential for the creation of quality children's programming",
				"Simultaneous Visitors – Do you have a site with hits and want to have no errors when 10, 20 or even arrive on the platform you are managing",
				"Also, when we talk about the Eastern, we see the political changes taking place here and we are confident that at level too, EU policy will change in the near future.",
				"It is important to clarify the content of such agreements, in particular for an action that are implemented by the third country under indirect management.",
				"The event will be prepared in close cooperation with the European, as well as organisers of similar events in the previous period (the Luxembourg and Dutch EU Presidency).",
				"Also, Grace has to make difficult decisions that could have life or death consequences for people, and Liam reunite under surprising circumstances.",
				"The first edition of a new online communication tool gives examples of judgments from the Court of Rights and how their implementation has improved people’s lives across Europe.",
				"The conflict proceeded, as suggested when the Bible came into great prominence, nearly all of our great Bible Societies of today having been organized within fifteen years after that date.",
			};

			TEST_TEXTS = new[] {
				//"I know, I know, this is not a car, it’s a horse carriage — but we needed a reference point.",
				"Stanley were twins born in Kingsland, Maine, on June 1",
				"In 1911 Austro-Daimler began producing the Prinz (in English: Prince Henry) model this car  an overhead cam 5,714-cc four-cylinder engine.",
				"In this case, the moon has to be enough to cast shadow",
				"Such locations as this are seldom missed by the inexperienced navigators and are dangerous places for soldiers to occupy",
				"There are few or features to navigate by, making dead reckoning or navigation by stars the only technique for movement",
			};

			//Task.Run(async() => await ImportFileAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large2\\en-ro.tmx", "en-ro-1M-b", entryiesPerTextTable: 10000, maxImportTUCount: 1000000)).Wait();
			//return;

			//Task.Run(async() => await ImportFileAsync("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large2\\en-ro.tmx", "en-ro-10M-f", entryiesPerTextTable: 40000, maxImportTUCount: 10000000)).Wait();
			//return;


			//var TEST_TABLE = "en-ro-10M-f";
			var TEST_TABLE = "en-ro-2-copy";

			//Task.Run(async () => await TestSearcherSearch(TEST_TABLE, TEST_TEXTS, SearchType.Exact, "en", "ro")).Wait();
			//Task.Run(async () => await TestSearcherSearch(TEST_TABLE, TEST_TEXTS, SearchType.Fuzzy, "en", "ro")).Wait();
			//Task.Run(async () => await TestSearcherSearchDumpResults(TEST_TABLE, TEST_TEXTS, "en", "ro")).Wait();

			//Task.Run(async () => await TestSearcherSearchDumpResults(new[] { "ende-1",  }, TEST_TEXTS, "en-us", "de-de", careForLocale: true)).Wait();
			Task.Run(async () => await TestSearcherSearchDumpResults(new[] { "ende-1","ende-2" }, TEST_TEXTS, "en-us", "de-us", careForLocale: true)).Wait();

			//Task.Run(async () => await TestAvgExactAndFuzzySearcherSearch(TEST_TABLE, TEST_TEXTS, "en", "ro", 5, 15)).Wait();
			//Task.Run(async () => await TestAvgNormalSearcherSearch(TEST_TABLE, TEST_TEXTS, "en", "ro", 5, 15)).Wait();
			//Task.Run(async () => await TestWarmupTimesNormalSearcherSearch(TEST_TABLE, TEST_TEXTS, "en", "ro", 15)).Wait();


			//Task.Run(async () => await TestExportToXml()).Wait();
			//SplitLargeXmlFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\fails\\opensubtitlingformat.tmx", "C:\\john\\buff\\TMX Examples\\temp3\\");


			//Task.Run(() => TestImportFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large2\\en-ro.tmx", "en-ro-2", quickImport: true)).Wait();
			//Task.Run(() => TestImportFile("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large2\\ko-zh.tmx", "kozh", quickImport: false)).Wait();

			//TestEnRoImport().Wait();
			//TestDbSearch("sample4", "introduction", SearchType.Exact, "en-gb", "es-mx").Wait();
			//TestDbSearch("en-frEUBookshopv2108M", "The European Social Fund in French Guiana", SearchType.Exact, "en", "fr").Wait();
			//TestDbSearch("en-ro", "We might call them the words of \"unforgiveness", SearchType.Exact, "en", "ro").Wait();

			//TestSearcherSearch("en-ro-2", "we call them", SearchType.Concordance, "en-US", "ro-RO").Wait();
			//TestSearcherSearch("en-ro-2", "taught Happiness knowledge", SearchType.Concordance, "en-US", "ro-RO").Wait();

			log.Debug("test complete");
	        Console.ReadLine();
        }
    }
}
