using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public class CommentInfo
    {
        public string Author { get; set; }
        public string Date { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string Severity { get; set; }
        public string Text { get; set; }
    }
}