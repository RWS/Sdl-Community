using System;
using System.Collections.Generic;
using Sdl.Community.InvoiceAndQuotes.OpenXML;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    [Serializable]
    public class Customer : TokensProvider
    {
        private string _name;
        private string _street;
        private string _city;
        private string _state;
        private string _zip;
        private string _country;
        private List<TemplateRatesBase> _rates;

        public Customer(){}
        public Customer(string name, string street, string city, string state, string zip, string country)
        {
            Name = name;
            Street = street;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
        }

        public List<TemplateRatesBase> Rates
        {
            get { return _rates; }
            set { _rates = value; }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; UpdateTokenValue(TokenConstants.CUSTOMERNAME, value); }
        }

        public String Street
        {
            get { return _street; }
            set { _street = value; UpdateTokenValue(TokenConstants.CUSTOMERSTREET, value); }
        }

        public String City
        {
            get { return _city; }
            set { _city = value; UpdateTokenValue(TokenConstants.CUSTOMERCITY, value); }
        }

        public String State
        {
            get { return _state; }
            set { _state = value; UpdateTokenValue(TokenConstants.CUSTOMERSTATE, value); }
        }

        public String Zip
        {
            get { return _zip; }
            set { _zip = value; UpdateTokenValue(TokenConstants.CUSTOMERZIP, value); }
        }

        public String Country
        {
            get { return _country; }
            set { _country = value; UpdateTokenValue(TokenConstants.CUSTOMERCOUNTRY, value); }
        }

        public override void GenerateTokens()
        {
            Tokens = new List<Token>
            {
                    new Token(TokenConstants.CUSTOMERNAME, Name),
                    new Token(TokenConstants.CUSTOMERSTREET, Street),
                    new Token(TokenConstants.CUSTOMERCITY, City),
                    new Token(TokenConstants.CUSTOMERSTATE, State),
                    new Token(TokenConstants.CUSTOMERZIP, Zip),
                    new Token(TokenConstants.CUSTOMERCOUNTRY, Country),
                };
        }

    }
}
