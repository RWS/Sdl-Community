using Sdl.Community.NumberVerifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Processors
{
    public class ModifiedAlphanumericsVerifyProcessor : IVerifyProcessor
    {
        public IErrorMessageProcessor SourceMessageProcessor;
        public IErrorMessageProcessor TargetMessageProcessor;
        public IEnumerable<ErrorReporting> Verify(INumberResults numberResults)
        {
            yield return new ErrorReporting
            {
                ErrorLevel = GetAlphanumericsErrorLevel(numberResults),
                ErrorMessage = PluginResources.Error_AlphanumericsModified,
                SourceNumberIssues = SourceMessageProcessor.GenerateMessage(numberResults),
                TargetNumberIssues = TargetMessageProcessor.GenerateMessage(numberResults)
            };
        }

        private ErrorLevel GetAlphanumericsErrorLevel(INumberResults numberResults)
        {
            switch (numberResults.Settings.ModifiedAlphanumericsErrorType)
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
