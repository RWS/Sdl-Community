namespace GroupshareExcelAddIn.Models
{
    public class UserDetails : Sdl.Community.GroupShareKit.Models.Response.UserDetails
    {
        //TODO: see if it can be removed
        public string Description { get; set; }

        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string IsProtected { get; set; }
        public string Name { get; set; }
        public string OrganizationId { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string UniqueId { get; set; }
        public string UserType { get; set; }
    }
}