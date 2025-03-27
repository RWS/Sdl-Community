using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Core.Globalization;
using System;
using System.Text.RegularExpressions;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model
{
    public enum AddReplace
    {
        Add,
        Replace
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

    public class EnumHelper
    {
        private static string SplitPascalCase(string input) => string.Join(" ", Regex.Split(input, @"(?<!^)(?=[A-Z])"));

        public static string GetFriendlyStatusString(string statusString)
        {
            var friendlyStatus = ReportUtils.GetVisualSegmentStatus(statusString);
            if (friendlyStatus != "Unknown") return friendlyStatus;

            if (Enum.TryParse<Statuses>(statusString, out var statuses)) return SplitPascalCase(statuses.ToString());
            throw new ArgumentException($"Unknown status string: {statusString}");
        }

        public static bool TryGetConfirmationLevel(string friendlyStatus, out ConfirmationLevel confirmationLevel)
        {
            if (Enum.TryParse(friendlyStatus, out confirmationLevel)) return true;
            if (!TryGetStatus(friendlyStatus, out var status)) return false;

            confirmationLevel = (ConfirmationLevel)Math.Log((double)status, 2);
            return true;
        }

        public static bool TryGetStatus(string enumValue, out Statuses status)
        {
            enumValue = enumValue.Replace(" ", "").Replace("-", "");
            return Enum.TryParse(enumValue, true, out status);
        }
    }
}