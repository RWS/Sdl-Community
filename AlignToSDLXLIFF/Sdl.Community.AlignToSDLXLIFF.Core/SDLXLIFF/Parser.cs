using System;
using System.Collections.Generic;
using Sdl.Community.Amgen.Core.EventArgs;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Amgen.Core.SDLXLIFF
{
	public class Parser
	{

		private readonly PocoFilterManager _pocoFilterManager;

		internal event EventHandler<ProgressEventArgs> ProgressEvent;

		internal Parser(PocoFilterManager pocoFilterManager)
		{
			_pocoFilterManager = pocoFilterManager;
		}

		internal List<SegmentInfo> ReadFile(string filePath, ProcessorOptions options)
		{
			var converter = _pocoFilterManager.GetConverterToDefaultBilingual(filePath, filePath + ".out", null);

			var contentProcessor = new ParagraphProcessor
			{
				Segments = new List<SegmentInfo>()
			};

			if (options.SourceToTargetCopier.CopySourceToTaret)
			{
				converter.AddBilingualProcessor(new SourceToTargetCopier(
					options.SourceToTargetCopier.Preserve
						? ExistingContentHandling.Preserve
						: ExistingContentHandling.Replace));
			}

			converter.AddBilingualProcessor(contentProcessor);

			try
			{
				converter.Progress += ConverterProgress;

				converter.Parse();
			}
			finally
			{
				converter.Progress -= ConverterProgress;
			}

			return contentProcessor.Segments;
		}

		private void ConverterProgress(object sender, BatchProgressEventArgs e)
		{
			ProgressEvent?.Invoke(this, new ProgressEventArgs
			{
				Maximum = e.TotalFiles,
				Current = e.FileNumber,
				Percent = e.FilePercentComplete,
				Message = e.FilePath
			});
		}
	}
}