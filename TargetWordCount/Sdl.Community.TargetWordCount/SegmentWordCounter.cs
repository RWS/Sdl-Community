using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace Sdl.Community.TargetWordCount
{
	public class SegmentWordCounter : AbstractBilingualContentProcessor, ISegmentWordCounter
	{
		// Source and Target language info. First index is source and second is target.
		private readonly Language[] language = new Language[2];

		private readonly List<SegmentCountInfo> segmentCountInfo = new List<SegmentCountInfo>();
		private readonly WordCounter wordCounter = null;
		private FileCountInfo fileCountInfo = null;
		private IRepetitionsTable repTable = null;
		private readonly IWordCountBatchTaskSettings settings;

		public SegmentWordCounter(string name, IWordCountBatchTaskSettings settings, WordCounter wordCounter)
		{
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name));
			Contract.Requires<ArgumentNullException>(settings != null);
			Contract.Requires<ArgumentNullException>(wordCounter != null);

			FileName = name;
			this.settings = settings;
			this.wordCounter = wordCounter;
		}

		public FileCountInfo FileCountInfo
		{
			get
			{
				if (fileCountInfo == null)
				{
					fileCountInfo = new FileCountInfo(segmentCountInfo, language, repTable);
				}

				return fileCountInfo;
			}
		}

		public string FileName { get; }

		public override void Initialize(IDocumentProperties documentInfo)
		{
			// This MUST be called
			base.Initialize(documentInfo);

			repTable = documentInfo.Repetitions;
			language[0] = documentInfo.SourceLanguage;
			language[1] = documentInfo.TargetLanguage;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure) { return; }

			foreach (var pair in paragraphUnit.SegmentPairs)
			{
				if (settings.UseSource)
				{
					var source = pair.Source;

					var plainText = new PlainTextExtractor().GetPlainText(source);

					segmentCountInfo.Add(new SegmentCountInfo(source.Properties.TranslationOrigin, wordCounter.Count(source), source.Properties.IsLocked, plainText.Count(char.IsWhiteSpace)));
				}
				else
				{
					var target = pair.Target;

					var plainText = new PlainTextExtractor().GetPlainText(target);

					segmentCountInfo.Add(new SegmentCountInfo(target.Properties.TranslationOrigin, wordCounter.Count(target), target.Properties.IsLocked, plainText.Count(char.IsWhiteSpace)));
				}
			}
		}
	}
}