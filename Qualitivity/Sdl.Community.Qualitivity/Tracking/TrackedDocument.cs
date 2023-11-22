﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Trados.Community.Toolkit.LanguagePlatform;

namespace Sdl.Community.Qualitivity.Tracking
{
	/// <summary>
	/// information tracked from the individual document.
	/// 
	/// this becomes neccessary when the user open multple document in the
	/// editor that causes a virtual merge.
	/// </summary>
	public class TrackedDocument
	{
		public TrackedDocument()
		{
			Id = string.Empty;
			Name = string.Empty;
			Path = string.Empty;

			SourceLanguage = string.Empty;
			TargetLanguage = string.Empty;

			TranslationMatchTypesOriginal = new List<StateCountItem>();
			ConfirmationStatusOriginal = new List<StateCountItem>();
			TotalSegments = 0;

			TrackedRecords = new List<Record>();

			DocumentTimer = new Stopwatch();
			DatetimeOpened = null;
			DatetimeClosed = null;
		}

		public string Id { get; set; } //document id from studio
		public string Name { get; set; }
		public string Path { get; set; }

		public string ProductName { get; set; }

		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }

		//the statistical counters registered when the document was opened; we will use this 
		//as a base for comparison later on when generating the reports...
		public List<StateCountItem> TranslationMatchTypesOriginal { get; set; }
		public List<StateCountItem> ConfirmationStatusOriginal { get; set; }
		public int TotalSegments { get; set; }

		//the record changes that have been registered
		public List<Record> TrackedRecords { get; set; }


		//the total time tracked for the document from the time that it was opened to the time it was closed        
		public Stopwatch DocumentTimer { get; set; }
		public DateTime? DatetimeOpened { get; set; }
		public DateTime? DatetimeClosed { get; set; }

		private SegmentPairProcessor _segmentPairProcessor;

		public SegmentPairProcessor SegmentPairProcessor
		{
			get
			{
				if (_segmentPairProcessor != null)
				{
					return _segmentPairProcessor;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(string.Format("Unable to parse the file; {0} language cannot be null!", SourceLanguage == null ? "Source" : "Target"));
				}

				var pathInfo = new Trados.Community.Toolkit.LanguagePlatform.Models.PathInfo(ProductName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Trados.Community.Toolkit.LanguagePlatform.Models.Settings(new CultureInfo(SourceLanguage), new CultureInfo(TargetLanguage)),
					pathInfo);

				return _segmentPairProcessor;
			}
		}
	}
}
