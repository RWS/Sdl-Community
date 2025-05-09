using System.Collections.Generic;

namespace GroupshareExcelAddIn.Helper
{
    public static class Constants
    {
        public static readonly List<string> CustomFieldColumns = new List<string>()
        {
            "Name",
            "Description",
            "Resource Type",
            "Default Value",
            "Read Only",
            "Organization"
        };

        public static readonly List<string> FieldTemplateColumns = new List<string>
        {
            "Name",
            "Location",
            "Description",
            "Field template ID",
            "Owner ID",
            "TM Specific",
            "Fields"
        };

        public static readonly List<string> LanguageResourceTemplateColumns = new List<string>
        {
            "Name",
            "Location",
            "Description",
            "Language Resource Template ID",
            "Owner ID",
            "Is TM Specific",
            "Language Resources"
        };

        public static readonly List<string> PhasesList = new List<string>
        {
            "Preparation",
            "Translation",
            "Review",
            "Finalisation"
        };

        public static readonly List<string> ProjectColumns = new List<string>()
        {
            "Organization", "Project Name", "Created At", "Delivery Date", "Client",  "Project Language Pairs", "Project Status", "Project Description", "File Status", "File Name", "File Type", "File Size (bytes)", "File Role", "File Language Code", "File Check In By", "File Check Out To", "File Last Check In Date", "File Last Check Out Date", "File Last Modified Date", "File Approved SignOff Words", "File Approved Translation Words", "File Draft Words", "File Rejected SignOff Words","File Rejected Translation Words", "File Total Words","File Translated Words", "File Unspecified Words","File Type Percent Complete", "Assignees Names", "Assignees Roles", "Assignees Emails","Current Phase"
        };

        public static readonly Dictionary<int, string> ProjectStatusDict = new Dictionary<int, string>()
        {
            {2, "In progress" },
            {4, "Completed"},
            {8, "Archived"}
        };

        public static readonly List<string> ProjectTemplateColumns = new List<string>()
        {
            "Name",
            "ID",
            "Description",
            "Organization"
        };

        public static readonly List<string> TermbaseColumns = new List<string>
        {
            "Name",
            "Id",
            "Description",
            "Parent Organization",
            "Linked Organization",
        };

        public static readonly List<string> TranslationMemoryColumns = new List<string>()
        {
            "Translation Memory Name",
            "Translation Memory Language Direction",
            "Number of Translation Units",
            "Translation Memory Description",
            "Translation Memory Location",
            "Translation Memory Creation Date",
            "Translation Memory Tokenizer Flags",
            "Translation Memory WordCount Flags",
            "Translation Memory Last Recomputed Date",
            "Translation Memory Last Recomputed Size",
            "Translation Memory Fuzzy Indexes",
            "Last Reindex Date",
            "Last Reindex Size",
            "Should Recompute Statistics"
        };

        public static readonly List<string> UsersColumns = new List<string>()
        {
            "User Name",
            "User Email",
            "Description",
            "Phone Number",
            "Role",
            "Project (role related)",
            "Organization"
        };
    }
}