using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace EditScript
{
    public class EditScriptService
    {
        public TranslationUnit CreateTu(string src, string trg)
        {
            var tu = new TranslationUnit();

            var srcSegment = new Segment(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            var trgSegment = new Segment(System.Globalization.CultureInfo.GetCultureInfo("de-DE"));

            srcSegment.Add(src);
            trgSegment.Add(trg);

            tu.SourceSegment = srcSegment;
            tu.TargetSegment = trgSegment;

            return tu;
        }
    }
}