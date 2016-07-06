using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Model
{
    public class ErrorReporting
    {
        public ErrorLevel ErrorLevel { get; set; }
        public string ErrorMessage { get; set; }

        public string ExtendedErrorMessage { get; set; }

        public string TargetNumberIssues { get; set; }
        public string SourceNumberIssues { get; set; }

    }
}
