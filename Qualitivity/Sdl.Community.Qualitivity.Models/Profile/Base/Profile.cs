namespace Sdl.Community.Structures.Profile.Base
{
    
    
    public class Profile
    {        
        public int Id { get; set; }
        public string Name { get; set; }       
        public string Street { get; set; }        
        public string City { get; set; }        
        public string State { get; set; }       
        public string Zip { get; set; }       
        public string Country { get; set; }
        public string TaxCode { get; set; }
        public string VatCode { get; set; }       
        public string Email { get; set; }
        public string Web { get; set; }     
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Note { get; set; }

        public Profile()
        {
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

      
    }
}
