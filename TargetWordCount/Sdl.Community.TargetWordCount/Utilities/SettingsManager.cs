using Sdl.Community.TargetWordCount.Models;

namespace Sdl.Community.TargetWordCount.Utilities
{
	public static class SettingsManager
    {
        public static SerializableSettings ConvertToSerializableSettings(IWordCountBatchTaskSettings settings)
        {
            var s = new SerializableSettings();
            s.CharactersPerLine = settings.CharactersPerLine;
            s.Culture = settings.Culture;
            s.IncludeSpaces = settings.IncludeSpaces;
            s.InvoiceRates = settings.InvoiceRates;
            s.ReportLockedSeperately = settings.ReportLockedSeperately;
            s.UseLineCount = settings.UseLineCount;
            s.UseSource = settings.UseSource;

            return s;
        }

        public static void UpdateSettings(SerializableSettings s1, IWordCountBatchTaskSettings s2)
        {
            s2.CharactersPerLine = s1.CharactersPerLine;
            s2.Culture = s1.Culture;
            s2.IncludeSpaces = s1.IncludeSpaces;
            s2.InvoiceRates = s1.InvoiceRates;
            s2.ReportLockedSeperately = s1.ReportLockedSeperately;
            s2.UseLineCount = s1.UseLineCount;
            s2.UseSource = s1.UseSource;
        }
    }
}