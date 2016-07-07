using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Utils
{
    public class ErrorReportingUtils
    {
        public  static ErrorLevel GetAlphanumericsErrorLevel(List<string> sourceList, List<string> targetList,
            INumberVerifierSettings settings)
        {
            if ((targetList.Count <= 0 && sourceList.Count <= 0) || !settings.ReportModifiedAlphanumerics)
                return ErrorLevel.Unspecified;
            switch (settings.ModifiedAlphanumericsErrorType)
            {
                case "Error":
                    return ErrorLevel.Error;
                case "Warning":
                    return ErrorLevel.Warning;
                default:
                    return ErrorLevel.Note;
            }
        }

        public static string GetAlphanumericsIssues(List<string> alphanumericsList, INumberVerifierSettings settings)
        {
            var result = string.Empty;
            if (alphanumericsList.Count > 0 && settings.ReportModifiedAlphanumerics)
            {
                 alphanumericsList.Aggregate(result,
                    (current, t) => current + (t + " \r\n"));
                result = alphanumericsList[0];
            }
            return result;
        }
    }
}
