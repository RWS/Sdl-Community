using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class AddressConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Address) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is Address))
                return base.ConvertTo(context, culture, value, destinationType);
            var address = (Address)value;

            return address.AddressStreet + ",  "
                   + address.AddressZip + " "
                   + address.AddressCity
                   + (address.AddressState.Trim() != string.Empty ? " (" + address.AddressState + ")" : string.Empty)
                   + (address.AddressCountry.Trim() != string.Empty ? ", " + address.AddressCountry : string.Empty);
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


    [TypeConverter(typeof(AddressConverter)),
    Description("Expand to see the Address")]
    public class Address
    {
        [DisplayName("Street")]
        [Description("The street address")]
        [ReadOnly(true)]
        public string AddressStreet { get; set; }
        [DisplayName("City")]
        [Description("The city")]
        [ReadOnly(true)]
        public string AddressCity { get; set; }
        [DisplayName("State")]
        [Description("The state")]
        [ReadOnly(true)]
        public string AddressState { get; set; }
        [DisplayName("ZIP")]
        [Description("The zip code")]
        [ReadOnly(true)]
        public string AddressZip { get; set; }
        [DisplayName("Country")]
        [Description("The country")]
        [ReadOnly(true)]
        public string AddressCountry { get; set; }     
    }

    
}
