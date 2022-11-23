using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;
using TMX_Lib.XmlSplit;

namespace QuickTmxTesting
{
	// quick and dirty tests
    internal class Program
    {
	    private static async Task Test(string root)
	    {
//		    var parser = new TmxParser($"{root}\\SampleTestFiles\\Banking TextBase.tmx");
		    var parser = new TmxParser("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en-fr (EU Bookshop v2_10.8M).tmx");
		    //var parser = new TmxParser("C:\\john\\buff\\TMX Examples\\TMX Test Files\\large\\en(GB) - it(IT)_(DGT 2015, 2017).tmx");
		    var db = new TmxMongoDb("localhost:27017", "mydb");
		    await MongoDbUtil.ImportToDbAsync(parser, db);
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

	        Task.Run(() => Test(root)).Wait();
	        Console.ReadLine();
        }
    }
}
