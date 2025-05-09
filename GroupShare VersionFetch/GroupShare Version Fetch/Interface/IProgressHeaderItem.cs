namespace Sdl.Community.GSVersionFetch.Interface
{
    public interface IProgressHeaderItem
    {
        string DisplayName { get; set; }

        bool IsCurrentPage { get; set; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        bool IsValid { get; set; }
        bool IsVisited { get; set; }
        double ItemLineWidth { get; set; }
        double ItemTextWidth { get; set; }
        bool NextIsVisited { get; set; }
        int PageIndex { get; set; }

        bool PreviousIsVisited { get; set; }
        int TotalPages { get; set; }

        bool OnChangePage(int position, out string message);
    }
}