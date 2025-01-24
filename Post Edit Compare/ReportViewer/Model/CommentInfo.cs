using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;

namespace Sdl.Community.PostEdit.Versions.ReportViewer.Model
{
    public class CommentInfo
    {
        public string Author { get; set; }
        public string Date { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Severity Severity { get; set; }
        public string Text { get; set; }
    }
}