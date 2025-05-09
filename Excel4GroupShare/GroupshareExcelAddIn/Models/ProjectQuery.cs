namespace GroupshareExcelAddIn.Models
{
    public class ProjectQuery
    {
        public ProjectFilter Filter { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int StartItem { get; set; }
    }
}