using System;

namespace GroupshareExcelAddIn.Models
{
    [Flags]
    public enum ProjectStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 4,
        Archived = 8,
        Detached = 16
    }
}