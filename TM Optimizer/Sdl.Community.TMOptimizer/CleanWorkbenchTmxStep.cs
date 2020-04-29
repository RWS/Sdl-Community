using System;
using System.IO;
using Sdl.Community.TMOptimizerLib;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Cleans a Workbench TMX file, removing unnecessary formatting tags
	/// </summary>
	class CleanWorkbenchTmxStep : ProcessingStep
    {
        private TmxFile _inputTmxFile;
        private TmxFile _outputTmxFile;
        private Cleaner _cleaner;
        private int? _maxTusProcessed;

        public CleanWorkbenchTmxStep(TmxFile inputTmxFile, TmxFile outputTmxFile, int? maxTusProcessed)
            : base(String.Format("Clean {0}", Path.GetFileName(inputTmxFile.FilePath)))
        {
            _inputTmxFile = inputTmxFile;
            _outputTmxFile = outputTmxFile;
            _maxTusProcessed = maxTusProcessed;
        }

        protected override void ExecuteImpl()
        {
            _cleaner = new Cleaner(_inputTmxFile, _outputTmxFile, Context.Settings, null);
            AttachProcessorEvents(_cleaner);
            _cleaner.Execute();
        }

        protected override void OnReportProgress()
        {
            StatusMessage = String.Format("TUs: {0}/{1} | tags removed: {2} | tags updated {3}", _cleaner.TusRead, _inputTmxFile.GetDetectInfo().TuCount, _cleaner.TagsRemoved, _cleaner.TagsUpdated);
        }
    }
}