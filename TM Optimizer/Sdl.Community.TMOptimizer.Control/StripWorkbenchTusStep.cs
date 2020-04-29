using System;
using Sdl.Community.TMOptimizerLib;

namespace Sdl.Community.TMOptimizer.Control
{
	class StripWorkbenchTusStep : ProcessingStep
    {
        private TmxFile _inputTmxFile;
        private TmxFile _outputTmxFile;
        private Stripper _stripper;

        public StripWorkbenchTusStep(string name, TmxFile inputTmxFile, TmxFile outputTmxFile) : base(String.Format("Identify Workbench TUs in {0}", name))
        {
            _inputTmxFile = inputTmxFile;
            _outputTmxFile = outputTmxFile;
        }

        protected override void ExecuteImpl()
        {
            _stripper = new Stripper(_inputTmxFile, _outputTmxFile, Context.Settings);
            AttachProcessorEvents(_stripper);
            _stripper.Execute();
        }

        protected override void OnReportProgress()
        {
            StatusMessage = String.Format("TUs: {0}/{1} | Workbench TUs removed: {2}", _stripper.TusRead, _inputTmxFile.GetDetectInfo().TuCount, _stripper.TusStripped);
        }
    }
}