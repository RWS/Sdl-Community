using System;

namespace Sdl.Community.Structures.Profile
{
    
    [Serializable]
    public class UserProfile: Base.Profile, ICloneable
    {
        public string UserName { get; set; }

        public UserProfile()
        {
            UserName = string.Empty;
           
            Id = -1;            
            Name = string.Empty;
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Zip = string.Empty;
            Country = string.Empty;
            TaxCode = string.Empty;
            VatCode = string.Empty;
            Email = string.Empty;
            Web = string.Empty;
            Phone = string.Empty;
            Mobile = string.Empty;
            Fax = string.Empty;
            Note = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
