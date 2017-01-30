using System;
using System.Collections.Generic;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ClientProfileInfo: ICloneable
    {


        public string Id { get; set; } 
        public string ClientName { get; set; }

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

        //public string currency { get; set; }

        public List<ClientActivityType> ClientActivities { get; set; }


        public ClientProfileInfo()
        {
            Id = string.Empty;
            ClientName = string.Empty;

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


            ClientActivities = new List<ClientActivityType>();

        }

        public object Clone()
        {
            var cpi = new ClientProfileInfo();

            cpi.Id = Id;
            cpi.ClientName = ClientName;
            cpi.AddressStreet = AddressStreet;
            cpi.AddressCity = AddressCity;
            cpi.AddressState = AddressState;
            cpi.AddressZip = AddressZip;
            cpi.AddressCountry = AddressCountry;

            cpi.TaxCode = TaxCode;
            cpi.VatCode = VatCode;

            cpi.Email = Email;
            cpi.WebPage = WebPage;

            cpi.PhoneNumber = PhoneNumber;
            cpi.FaxNumber = FaxNumber;


            cpi.ClientActivities = new List<ClientActivityType>();
            foreach (var ca in ClientActivities)
                cpi.ClientActivities.Add((ClientActivityType)ca.Clone());

         

            return cpi;
        }
    }
}
