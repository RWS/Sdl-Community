using System;
using System.Collections.Generic;
using Sdl.Community.InvoiceAndQuotes.OpenXML;

namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    public class User : TokensProvider
    {
        private string _name;
        private string _street;
        private string _city;
        private string _state;
        private string _twitter;
        private string _webAddress;
        private string _skype;
        private string _email;
        private string _mobile;
        private string _phone;
        private string _country;
        private string _zip;

        public User(){}
        public User(string name, string street, string city, string state, string zip, string country,
            string phone, string mobile, string email, string skype, string webAddress, string twitter)
        {
            Name = name;
            Street = street;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
            Phone = phone;
            Mobile = mobile;
            Email = email;
            Skype = skype;
            WebAddress = webAddress;
            Twitter = twitter;
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; UpdateTokenValue(TokenConstants.USERNAME, value); }
        }

        public String Street
        {
            get { return _street; }
            set { _street = value; UpdateTokenValue(TokenConstants.USERSTREET, value); }
        }

        public String City
        {
            get { return _city; }
            set { _city = value; UpdateTokenValue(TokenConstants.USERCITY, value); }
        }

        public String State
        {
            get { return _state; }
            set { _state = value; UpdateTokenValue(TokenConstants.USERSTATE, value); }
        }

        public String Zip
        {
            get { return _zip; }
            set { _zip = value; UpdateTokenValue(TokenConstants.USERZIP, value); }
        }

        public String Country
        {
            get { return _country; }
            set { _country = value; UpdateTokenValue(TokenConstants.USERCOUNTRY, value); }
        }

        public String Phone
        {
            get { return _phone; }
            set { _phone = value; UpdateTokenValue(TokenConstants.USERPHONE, value); }
        }

        public String Mobile
        {
            get { return _mobile; }
            set { _mobile = value; UpdateTokenValue(TokenConstants.USERMOBILE, value); }
        }

        public String Email
        {
            get { return _email; }
            set { _email = value; UpdateTokenValue(TokenConstants.USEREMAIL, value); }
        }

        public String Skype
        {
            get { return _skype; }
            set { _skype = value; UpdateTokenValue(TokenConstants.USERSKYPE, value); }
        }

        public String WebAddress
        {
            get { return _webAddress; }
            set { _webAddress = value; UpdateTokenValue(TokenConstants.USERWEBADDRESS, value); }
        }

        public String Twitter
        {
            get { return _twitter; }
            set { _twitter = value; UpdateTokenValue(TokenConstants.USERTWITTER, value); }
        }

        public override void GenerateTokens()
        {
            Tokens = new List<Token>()
                {
                    new Token(TokenConstants.USERNAME, Name),
                    new Token(TokenConstants.USERSTREET, Street),
                    new Token(TokenConstants.USERCITY, City),
                    new Token(TokenConstants.USERSTATE, State),
                    new Token(TokenConstants.USERZIP, Zip),
                    new Token(TokenConstants.USERCOUNTRY, Country),
                    new Token(TokenConstants.USERPHONE, Phone),
                    new Token(TokenConstants.USERMOBILE, Mobile),
                    new Token(TokenConstants.USEREMAIL, Email),
                    new Token(TokenConstants.USERSKYPE, Skype),
                    new Token(TokenConstants.USERWEBADDRESS, WebAddress),
                    new Token(TokenConstants.USERTWITTER, Twitter),
                };
        }
    }
}