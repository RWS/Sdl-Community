using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QATracker.BatchTasks
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
