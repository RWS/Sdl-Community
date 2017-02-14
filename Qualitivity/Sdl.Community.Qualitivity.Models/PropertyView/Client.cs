using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class ClientConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Client) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is Client))
                return base.ConvertTo(context, culture, value, destinationType);
            var client = (Client)value;

            return client.ClientName;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
        
            return base.ConvertFrom(context, culture, value);
        }


    }


    [TypeConverter(typeof(ClientConverter)),
    Description("Expand to see the Client")]
    public class Client
    {
       
        [DisplayName("ID")]
        [Description("The ID of the client")]
        [ReadOnly(true)]
        public string ClientId { get; set; }

       
        [DisplayName("Name")]
        [Description("The Name of the client")]
        [ReadOnly(true)]
        public string ClientName { get; set; }


        [DisplayName("Address")]
        [Description("The Address of the client")]
        [ReadOnly(true)]
        public Address ClientAddress { get; set; }


        [DisplayName("TAX Nr.")]
        [Description("The TAX number of the client")]
        [ReadOnly(true)]
        public string ClientTax { get; set; }


        [DisplayName("VAT Nr.")]
        [Description("The VAT number of the client")]
        [ReadOnly(true)]
        public string ClientVat { get; set; }

        
        [DisplayName("Contact Info")]
        [Description("The contact info for the client")]
        [ReadOnly(true)]
        public ContactDetails ContactDetails { get; set; }


        [DisplayName("Default Rates")]
        [Description("The default rates for the client")]
        [ReadOnly(true)]
        public DefaultRates DefaultRates { get; set; }  
    }

    
}
