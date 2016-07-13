using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Composers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Processors
{
   public class VerifyProcessor:IVerifyProcessor
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
            yield return new ErrorReporting
            {
                ErrorLevel = GetNumbersErrorLevel(numberResults),
                ErrorMessage = ErrorMessage,
                ExtendedErrorMessage = ExtendedErrorMessageProcessor.GenerateMessage(numberResults),
                SourceNumberIssues = SourceMessageProcessor.GenerateMessage(numberResults),
                TargetNumberIssues = TargetMessageProcessor.GenerateMessage(numberResults)
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
