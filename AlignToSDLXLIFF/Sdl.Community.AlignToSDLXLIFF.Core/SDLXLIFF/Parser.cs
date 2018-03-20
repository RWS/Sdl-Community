using System;
using System.Collections.Generic;
using Sdl.Community.Amgen.Core.EventArgs;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Amgen.Core.SDLXLIFF
{
	public class Parser
	{

		private readonly IFileTypeManager _fileTypeManager;

		internal event EventHandler<ProgressEventArgs> ProgressEvent;

		internal Parser(IFileTypeManager fileTypeManager)
		{
			_fileTypeManager = fileTypeManager;
		}

		internal List<SegmentInfo> ReadFile(string filePath, ProcessorOptions options)
		{
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePath, filePath + ".out", null);

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