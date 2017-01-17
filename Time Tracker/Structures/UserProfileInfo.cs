using System;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class UserProfileInfo: ICloneable
    {


        public string Id { get; set; } 
        public string CompanyName { get; set; }

        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }
        public string AddressCountry { get; set; }

        public string TaxCode { get; set; }
        public string VatCode { get; set; }

        public string Email { get; set; }
        public string WebPage { get; set; }

        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }


        public UserProfileInfo()
        {
            Id = string.Empty;
            CompanyName = string.Empty;

            AddressStreet = string.Empty;
            AddressCity = string.Empty;
            AddressState = string.Empty;
            AddressZip = string.Empty;
            AddressCountry = string.Empty;


            TaxCode = string.Empty;
            VatCode = string.Empty;

            Email = string.Empty;
            WebPage = string.Empty;

            PhoneNumber = string.Empty;
            FaxNumber = string.Empty;
         
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
