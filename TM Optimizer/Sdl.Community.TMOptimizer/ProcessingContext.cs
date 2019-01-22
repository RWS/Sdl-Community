using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.TMOptimizerLib;

namespace Sdl.Community.TMOptimizer
{
	public class ProcessingContext
    {
        public ProcessingContext()
        {
            TempFiles = new List<string>();
        }

        public Settings Settings
        {
            get; set;
        }

        public string GetTempTmxFile()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tmx");
            TempFiles.Add(path);
            return path;
        }

        public List<string> TempFiles
        {
            get;
            private set;
        }
    }
}