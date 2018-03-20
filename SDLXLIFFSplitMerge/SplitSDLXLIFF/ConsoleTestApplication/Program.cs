namespace ConsoleTestApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sdl.Utilities.SplitSDLXLIFF.Lib;

    class Program
    {
        // arg[0] - input file path
        // arg[1] - output path
        // arg[2] - words count
        // arg[3] - percent limit
        static void Main(string[] args)
        {
            string fPath = @"C:\MYDOCs\SplitTaskDocumentation\test_docs\large.doc.sdlxliff";
            string outPath = @"C:\MYDOCs\SplitTaskDocumentation\test_docs\out";
            string splitInfoPath = "";
            int wordsMax = -1;
            int percMax = -1;

            List<SegStatus> _statuses = new List<SegStatus>();
            _statuses.Add(SegStatus.ApprovedSignOff);
            _statuses.Add(SegStatus.ApprovedTranslation);
            _statuses.Add(SegStatus.Translated);

            # region // test SPLIT + MERGE (for debug)
            if (args.Length == 0)
            {
                FileParser _testS = new FileParser(fPath, outPath);
                SplitOptions _options = new SplitOptions(SplitOptions.SplitType.WordsCount);
                _options.SplitNonCountStatus = _statuses;
                _options.WordsCount = wordsMax;
                if (percMax > 0)
                {
                    _options.IsPercent = true;
                    _options.PercMax = percMax;
                }

                _testS.Split(_options);

                if (_testS.FilesCount.HasValue)
                {
                    Console.WriteLine("Split Succeeded!");
                    Console.WriteLine(string.Format("Files Created = {0}", _testS.FilesCount.Value));

                    FileMerger _testM = new FileMerger(_testS);
                    _testM.Merge();
                    Console.WriteLine();

                    if (_testM.FilesCount.HasValue)
                    {
                        Console.WriteLine(string.Format("Files Merged = {0}", _testM.FilesCount.Value));
                        Console.WriteLine("Merge Succeeded!");
                    }
                    else Console.WriteLine("No merge file created...");
                }
                else Console.WriteLine("No split files created...");
            }
            #endregion

            #region // test SPLIT
            if (args.Length > 0 && args[0] == "s")
            {
                if (args.Length > 1)
                    fPath = args[1].Replace("/", @"\");
                if (args.Length > 2)
                    outPath = args[2].Replace("/", @"\");
                if (args.Length > 3)
                    int.TryParse(args[3], out wordsMax);
                if (args.Length > 4)
                    int.TryParse(args[4], out percMax);


                FileParser _test = new FileParser(fPath, outPath);
                SplitOptions _options = new SplitOptions(SplitOptions.SplitType.WordsCount);
                _options.SplitNonCountStatus = _statuses;
                _options.WordsCount = wordsMax;
                if (percMax > 0)
                {
                    _options.IsPercent = true;
                    _options.PercMax = percMax;
                }

                _test.Split(_options);

                if (_test.FilesCount.HasValue)
                {
                    Console.WriteLine("Succeeded!");
                    Console.WriteLine(string.Format("Files Created = {0}", _test.FilesCount.Value));
                }
                else Console.WriteLine("No files created...");
            }
            #endregion

            #region // test MERGE
            else if (args.Length > 0 && args[0] == "m")
            {
                if (args.Length > 1)
                    fPath = args[1].Replace("/", @"\");
                if (args.Length > 2)
                    outPath = args[2].Replace("/", @"\");
                if (args.Length > 3)
                    splitInfoPath = args[3].Replace("/", @"\");

                FileMerger _testM = new FileMerger(fPath, outPath, splitInfoPath);
                _testM.Merge();
                Console.WriteLine();

                if (_testM.FilesCount.HasValue)
                {
                    Console.WriteLine(string.Format("Files Merged = {0}", _testM.FilesCount.Value));
                    Console.WriteLine("Merge Succeeded!");
                }
                else Console.WriteLine("No merge file created...");
            }
            #endregion

            Console.ReadKey();
        }
    }
}
