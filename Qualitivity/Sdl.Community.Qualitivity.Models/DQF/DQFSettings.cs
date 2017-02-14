using System;

namespace Sdl.Community.Structures.DQF
{
    [Serializable]
    public class DqfSettings : ICloneable
    {
        public int Id { get; set; }
        public bool EnableReports { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserKey { get; set; }
        public string TranslatorName { get; set; }
        public string TranslatorEmail { get; set; }
        public string TranslatorKey { get; set; }

        public DqfSettings()
        {
            Id = -1;
            EnableReports = false;
            UserName = string.Empty;
            UserEmail = string.Empty;
            UserKey = string.Empty;

            TranslatorName = string.Empty;
            TranslatorEmail = string.Empty;
            TranslatorKey = string.Empty;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
