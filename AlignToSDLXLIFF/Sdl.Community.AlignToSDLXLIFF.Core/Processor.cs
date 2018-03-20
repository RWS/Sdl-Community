using System;
using System.Collections.Generic;
using Sdl.Community.Amgen.Core.EventArgs;
using Sdl.Community.Amgen.Core.SDLXLIFF;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Amgen.Core
{
	public class Processor
	{
		private readonly IFileTypeManager _fileTypeManager;

		public event EventHandler<ProgressEventArgs> ProgressEvent;

		public Processor() : this(DefaultFileTypeManager.CreateInstance(true)) { }

		public Processor(IFileTypeManager fileTypeManager)
		{
			_fileTypeManager = fileTypeManager;
		}

		public List<SegmentInfo> ReadFile(string filePath, ProcessorOptions options)
		{
			var parser = new Parser(_fileTypeManager);

			try
			{
				parser.ProgressEvent += ParserProgressEvent;

				return parser.ReadFile(filePath, options);
			}
			finally
			{
				parser.ProgressEvent -= ParserProgressEvent;
			}
		}

		private void ParserProgressEvent(object sender, ProgressEventArgs e)
		{
			ProgressEvent?.Invoke(this, e);
		}
	}
}