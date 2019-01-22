using Sdl.Core.Globalization;

namespace Sdl.Community.TMOptimizerLib
{
    public class DetectInfo
    {
        public enum Versions
        {
            Workbench,
            Studio,
            Unknown
        };

        public Versions DetectedVersion { get; set; }
        public Language SourceLanguage { get; set; }
        public Language TargetLanguage { get; set; }

        public string OriginalSourceLanguage { get; set; }
        public string OriginalTargetLanguage { get; set; }

        public int TuCount { get; set; }

        internal DetectInfo Clone()
        {
            return new DetectInfo
            {
                DetectedVersion = DetectedVersion,
                SourceLanguage = SourceLanguage,
                TargetLanguage = TargetLanguage,
                OriginalSourceLanguage = OriginalSourceLanguage,
                OriginalTargetLanguage = OriginalTargetLanguage,
                TuCount = TuCount
            };
        }
    }
}
