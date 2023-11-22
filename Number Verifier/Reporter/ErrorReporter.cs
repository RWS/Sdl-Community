﻿using System;
using System.Linq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.MessageUI;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Validator;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Reporter
{
	public class ErrorReporter
	{
		public ErrorReporter(IBilingualContentMessageReporter messageReporter, INumberVerifierSettings settings, IMessageFilter messageFilter) =>
			(MessageReporter, Settings, MessageFilter) = (messageReporter, settings, messageFilter);

		[Flags]
		public enum Range
		{
			Default = 0,
			Source = 1,
			Target = 2,
			Both = 3
		}

		private IMessageFilter MessageFilter { get; }

		private IBilingualContentMessageReporter MessageReporter { get; }
		private INumberVerifierSettings Settings { get; }

		public void ReportErrors(NumberTexts sourceNumberTexts, NumberTexts targetNumberTexts, ISegmentPair segmentPair)
		{
			var sourceNumbersTotal = sourceNumberTexts.Texts.Count;
			var targetNumbersTotal = targetNumberTexts.Texts.Count;
			var errorPairsTotal = sourceNumbersTotal > targetNumbersTotal ? sourceNumbersTotal : targetNumbersTotal;

			for (var i = 0; i < errorPairsTotal; i++)
			{
				var extendedReportInfo = GetExtendedReport(i, sourceNumberTexts, targetNumberTexts);

				if (extendedReportInfo?.Message is null) continue;
				if (Settings.ReportExtendedMessages &&
					MessageReporter is IBilingualContentMessageReporterWithExtendedData extendedMessageReporter)
				{
					extendedMessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
						extendedReportInfo.ErrorLevel,
						extendedReportInfo.Message,
						new TextLocation(new Location(segmentPair.Target, true), extendedReportInfo.Report.TargetRange.StartIndex),
						new TextLocation(new Location(segmentPair.Target, false), extendedReportInfo.Report.TargetRange.Length),
						extendedReportInfo.Report);
				}
				else
				{
					MessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
						extendedReportInfo.ErrorLevel,
						extendedReportInfo.Message,
						new TextLocation(new Location(segmentPair.Target, true), extendedReportInfo.Report.TargetRange.StartIndex),
						new TextLocation(new Location(segmentPair.Target, false), extendedReportInfo.Report.TargetRange.Length));
				}
			}
		}

		private static string GetFormattedError(NumberText numberText)
		{
			if (numberText is null) return null;
			var explanation = numberText.IsValidNumber
				? numberText.Normalized
				: numberText.Errors[NumberText.ErrorLevel.TextAreaLevel].FirstOrDefault()?.Message;

			return $"{numberText.Text}	[{explanation}]";
		}

		private ExtendedErrorReportInfo GetExtendedReport(int i, NumberTexts sourceNumberTexts, NumberTexts targetNumberTexts)
		{
			var isInSourceRange = sourceNumberTexts.Texts.Count > i;
			var isInTargetRange = targetNumberTexts.Texts.Count > i;

			var range = Range.Default;
			if (isInSourceRange) range = Range.Source;
			if (isInTargetRange) range |= Range.Target;

			var errorReportInfo = new ExtendedErrorReportInfo();
			switch (range)
			{
				case Range.Source:
					errorReportInfo.Message = sourceNumberTexts[i].Errors[NumberText.ErrorLevel.SegmentPairLevel].FirstOrDefault()?.Message;

					if (!ShouldReport(errorReportInfo.Message)) return null;

					errorReportInfo.Report = new AlignmentErrorExtendedData
					{
						SourceIssues = GetFormattedError(sourceNumberTexts[i]),
						TargetIssues = "-",
						MessageType = "Segment-pair level errors"
					};
					errorReportInfo.ErrorLevel = GetNumbersErrorLevel(Settings.AddedNumbersErrorType);

					errorReportInfo.Report.SourceRange.StartIndex = sourceNumberTexts[i].StartIndex;
					errorReportInfo.Report.SourceRange.Length = sourceNumberTexts[i].Length;
					break;

				case Range.Both:
					errorReportInfo.Message = targetNumberTexts[i].Errors[NumberText.ErrorLevel.SegmentPairLevel].FirstOrDefault()?.Message;

					errorReportInfo.Report = new AlignmentErrorExtendedData
					{
						SourceIssues = GetFormattedError(sourceNumberTexts[i]),
						TargetIssues = GetFormattedError(targetNumberTexts[i]),
						MessageType = "Segment-pair level errors"
					};

					errorReportInfo.ErrorLevel = GetNumbersErrorLevel(Settings.ModifiedNumbersErrorType);

					errorReportInfo.Report.SourceRange.StartIndex = sourceNumberTexts[i].StartIndex;
					errorReportInfo.Report.SourceRange.Length = sourceNumberTexts[i].Length;
					errorReportInfo.Report.TargetRange.StartIndex = targetNumberTexts[i].StartIndex;
					errorReportInfo.Report.TargetRange.Length = targetNumberTexts[i].Length;

					break;

				case Range.Target:
					errorReportInfo.Message = targetNumberTexts[i].Errors[NumberText.ErrorLevel.SegmentPairLevel].FirstOrDefault()?.Message;

					errorReportInfo.Report = new AlignmentErrorExtendedData
					{
						SourceIssues = GetFormattedError(sourceNumberTexts[i]),
						TargetIssues = GetFormattedError(targetNumberTexts[i]),
						MessageType = "Segment-pair level errors",
					};
					errorReportInfo.ErrorLevel = GetNumbersErrorLevel(Settings.AddedNumbersErrorType);

					errorReportInfo.Report.TargetRange.StartIndex = targetNumberTexts[i].StartIndex;
					errorReportInfo.Report.TargetRange.Length = targetNumberTexts[i].Length;
					break;
			}

			errorReportInfo.Report.MessageType = "Alignment";
			return errorReportInfo;
		}

		private ErrorLevel GetNumbersErrorLevel(string setting)
		{
			switch (setting)
			{
				case "Error":
					return ErrorLevel.Error;

				case "Warning":
					return ErrorLevel.Warning;

				case "Note":
					return ErrorLevel.Note;
			}

			return ErrorLevel.Unspecified;
		}

		private bool ShouldReport(string errorMessage) => MessageFilter.IsAllowed(errorMessage);
	}
}