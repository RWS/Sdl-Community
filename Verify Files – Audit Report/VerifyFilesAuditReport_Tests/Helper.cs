using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureQARuleState_Tests
{
    public static class Helper
    {
        public static string GetUriOfResourceFile(string resourceFile)
        {
            var temp = Path.Combine(Path.GetTempPath(), $"{DateTime.Now.Millisecond}temp.file");

            File.WriteAllText(temp, resourceFile);
            return new Uri(temp, UriKind.Absolute).ToString();
        }
    }
}
