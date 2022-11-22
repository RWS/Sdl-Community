using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;

namespace QuickTmxTesting
{
	// quick and dirty tests
    internal class Program
    {
	    private static async Task Test(string root)
	    {
		    var parser = new TmxParser($"{root}\\SampleTestFiles\\Banking TextBase.tmx");
		    await parser.LoadAsync();
		    var db = new TmxMongoDb("localhost:27017", "mydb");
		    await Util.ImportToDbAsync(parser, db);
	    }
		static void Main(string[] args)
        {
	        var root = ".";
	        if (args.Length > 0)
		        root = args[0];

	        Task.Run(() => Test(root)).Wait();
        }
    }
}
