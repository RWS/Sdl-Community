using System.Collections.Generic;

namespace CaptureQARuleState.BatchTasks
{
    public class Constants
    {
        public static Dictionary<string, string> Statuses = new()
        {
            { "Not Translated", "Not Translated" },
            { "Draft", "Draft" },
            { "Translated", "Translated" },
            { "ApprovedTranslation", "Translation Approved" },
            { "RejectedTranslation", "Translation Rejected" },
            { "ApprovedSignOff", "Signed Off" },
            { "RejectedSignOff", "Sign-off Rejected" },
            { "Locked Segments", "Locked Segments" }
        };
    }
}
