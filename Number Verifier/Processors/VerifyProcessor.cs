using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Composers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Processors
{
	public class VerifyProcessor : IVerifyProcessor
	{
		public IErrorMessageProcessor SourceMessageProcessor;
		public IErrorMessageProcessor TargetMessageProcessor;
		public IErrorMessageProcessor ExtendedErrorMessageProcessor;
		public string ErrorMessage;

		public VerifyProcessor()
		{
			SourceMessageProcessor = new SourceErrorMessageComposer().Compose();
			TargetMessageProcessor = new TargetErrorMessageComposer().Compose();
			ExtendedErrorMessageProcessor = new ExtendedErrorMessageComposer().Compose();
		}

		public IEnumerable<ErrorReporting> Verify(INumberResults numberResults)
		{
			var initialSourceNumbers = string.Empty;
			var initialTargetNumbers = string.Empty;
			foreach (var item in numberResults.InitialSourceNumbers)
			{
				initialSourceNumbers += $"{item} ";
			}
			foreach (var item in numberResults.InitialTargetNumbers)
			{
				initialTargetNumbers += $"{item} ";
			}
			yield return new ErrorReporting
			{
				ErrorLevel = GetNumbersErrorLevel(numberResults),
				ErrorMessage = ErrorMessage,
				ExtendedErrorMessage = ExtendedErrorMessageProcessor.GenerateMessage(numberResults),
				SourceNumberIssues = SourceMessageProcessor.GenerateMessage(numberResults),
				TargetNumberIssues = TargetMessageProcessor.GenerateMessage(numberResults),
				InitialSourceNumber = initialSourceNumbers,
				InitialTargetNumber = initialTargetNumbers
			};
		}

		private ErrorLevel GetNumbersErrorLevel(INumberResults numberResults)
		{
			switch (numberResults.Settings.RemovedNumbersErrorType)
			{
				case "Error":
					return ErrorLevel.Error;
				case "Warning":
					return ErrorLevel.Warning;
				default:
					return ErrorLevel.Note;
			}
		}
	}
}