using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    internal class Parser
    {

        internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        internal event ChangedEventHandler Progress;


        internal Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnitsOriginal { get; set; }
        internal Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnitsUpdated { get; set; }

       

        internal Parser()
        {
 
            //initialize the paragraph units
            FileParagraphUnitsOriginal = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
            FileParagraphUnitsUpdated = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
        }

        internal int GetSegmentCount(Dictionary<string,Dictionary<string, ParagraphUnit>> fileParagraphUnits)
        {
            return fileParagraphUnits.SelectMany(fileParagraphUnit => fileParagraphUnit.Value).Sum(paragraphUnit => paragraphUnit.Value.SegmentPairs.Count);
        }


        internal void GetParagraphUnits(string filePathOriginal, string filePathUpdated)
        {
            FileParagraphUnitsOriginal = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
            FileParagraphUnitsUpdated = new Dictionary<string, Dictionary<string, ParagraphUnit>>();


            FileParagraphUnitsOriginal = GetFileParagraphUnits(filePathOriginal);
            FileParagraphUnitsUpdated = GetFileParagraphUnits(filePathUpdated);
        }

        private Dictionary<string, Dictionary<string, ParagraphUnit>> GetFileParagraphUnits(string filePath)
        {

            var manager = DefaultFileTypeManager.CreateInstance(true);
            var converter = manager.GetConverterToDefaultBilingual(filePath, filePath + "_.sdlxliff", null);

            var contentProcessor = new ContentProcessor
            {
                FileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>(),
                IncludeTagText = Processor.Settings.ComparisonIncludeTags
            };



            converter.AddBilingualProcessor(contentProcessor);
            try
            {
                converter.Progress += converter_Progress;
                converter.Parse();
            }
            finally
            {
                converter.Progress -= converter_Progress;
            }

            return contentProcessor.FileParagraphUnits;



        }


        private void converter_Progress(object sender, BatchProgressEventArgs e)
        {
            if (Progress != null)
            {
                Progress(100, e.FilePercentComplete, e.FilePercentComplete, Path.GetFileName(e.FilePath));
            }
        }
    }
}
