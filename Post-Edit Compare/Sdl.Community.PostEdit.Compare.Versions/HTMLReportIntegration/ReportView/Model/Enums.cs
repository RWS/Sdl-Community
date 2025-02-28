using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    [Flags]
    public enum Statuses
    {
        NotTranslated = 1,
        Draft = 2,
        Translated = 4,
        TranslationRejected = 8,
        TranslationApproved = 16,
        SignOffRejected = 32,
        SignedOff = 64
    }
    
    [Flags]
    public enum MatchTypes
    {
        CM = 1,
        PM = 2,
        AT = 4,
        ExactMatch = 8,
        FuzzyMatch = 16, //when fuzzy match -> another dropdown: fuzzy percentage based on settings
        NoMatch = 32
    }

    public enum Operator
    {
        Or,
        And
    }

    public enum AddReplace
    {
        Add,
        Replace
    }

    public class EnumHelper
    {
        public static bool TryGetStatus(string enumValue, out Statuses status) 
        {
            enumValue = enumValue.Replace(" ", "").Replace("-", "");
            return Enum.TryParse(enumValue, true, out status);
        }
    }
}