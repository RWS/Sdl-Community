using System.Collections.Generic;

namespace VerifyFilesAuditReport.Components.SegmentMetadata;

public static class SegmentStatuses
{
    /// <summary>Maps sdlxliff confirmation-status ids to the names Studio shows in its UI.</summary>
    public static IReadOnlyDictionary<string, string> UiNames { get; } = new Dictionary<string, string>
    {
        ["Not Translated"] = "Not Translated",
        ["Draft"] = "Draft",
        ["Translated"] = "Translated",
        ["ApprovedTranslation"] = "Translation Approved",
        ["RejectedTranslation"] = "Translation Rejected",
        ["ApprovedSignOff"] = "Signed Off",
        ["RejectedSignOff"] = "Sign-off Rejected",
        ["Locked Segments"] = "Locked Segments"
    };

    public static string ToUiName(string status) =>
        status != null && UiNames.TryGetValue(status, out var uiName) ? uiName : status;
}
