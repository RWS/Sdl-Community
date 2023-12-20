using System.IO;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Deletes temp files created during processing.
	/// </summary>
	class DeleteTempFilesStep : ProcessingStep
    {
        public DeleteTempFilesStep() : base("Delete temp files")
        {
        }

        protected override void ExecuteImpl()
        {
            for (int i = 0; i < Context.TempFiles.Count; i++)
            {
                try
                {
                    File.Delete(Context.TempFiles[i]);
                    ReportProgress((int)((i+1)*100.0)/Context.TempFiles.Count);
                }
                catch
                {
                    // ignore
                }
            }

            Context.TempFiles.Clear();

            ReportProgress(100);
        }
    }
}