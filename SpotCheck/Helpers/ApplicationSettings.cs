using System.Collections.Generic;

namespace Sdl.Studio.SpotCheck.Helpers
{
    public class ApplicationSettings : ApplicationSettingsBase
    {
        public int Percentage { get; set; }
        public int TotalWords { get; set; }
        public int MinWords { get; set; }
        public int MaxWords { get; set; }
        public bool SkipLocked { get; set; }
        public bool SkipCm { get; set; }
        public bool Skip100 { get; set; }
        public bool SkipRepetition { get; set; }
        public bool LockContext { get; set; }
        public bool LimitByWords { get; set; }

        protected override void TextFileToValues()
        {
            Percentage = _settings.GetIntValue("Percentage", 20);
            TotalWords = _settings.GetIntValue("TotalWords", 0);
            MinWords = _settings.GetIntValue("MinWords", 5);
            MaxWords = _settings.GetIntValue("MaxWords", 25);
            SkipLocked = _settings.GetBoolValue("SkipLocked", true);
            SkipCm = _settings.GetBoolValue("SkipCm", false);
            SkipRepetition = _settings.GetBoolValue("SkipRepetition", false);
            Skip100 = _settings.GetBoolValue("Skip100", false);
            LockContext = _settings.GetBoolValue("LockContext", false);
            LimitByWords = _settings.GetBoolValue("LimitByWords", false);
        }

        protected override void ValuesToTextFile()
        {
            _settings.SetIntValue("Percentage", Percentage);
            _settings.SetIntValue("TotalWords", TotalWords);
            _settings.SetIntValue("MinWords", MinWords);
            _settings.SetIntValue("MaxWords", MaxWords);
            _settings.SetBoolValue("SkipLocked", SkipLocked);
            _settings.SetBoolValue("SkipCm", SkipCm);
            _settings.SetBoolValue("SkipRepetition", SkipRepetition);
            _settings.SetBoolValue("Skip100", Skip100);
            _settings.SetBoolValue("LockContext", LockContext);
            _settings.SetBoolValue("LimitByWords", LimitByWords);
        }

    }
}
