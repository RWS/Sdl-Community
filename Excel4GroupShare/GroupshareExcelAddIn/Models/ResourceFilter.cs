using Sdl.Community.GroupShareKit.Models.Response;

namespace GroupshareExcelAddIn.Models
{
    public class ResourceFilter
    {
        public bool IncludeSubOrganizations { get; set; }
        public Organization Organization { get; set; }
        public FilterParameter SecondParameter { get; set; }
    }
}