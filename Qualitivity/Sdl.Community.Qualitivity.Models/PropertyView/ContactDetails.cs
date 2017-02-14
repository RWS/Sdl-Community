using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class ContactDetailsConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ContactDetails) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is ContactDetails))
                return base.ConvertTo(context, culture, value, destinationType);
            var contactDetails = (ContactDetails)value;

            var str = string.Empty;

            if (contactDetails.EMail.Trim() != string.Empty)
                str = "e-mail: " + contactDetails.EMail;
            else if (contactDetails.Phone.Trim() != string.Empty)
                str = "phone: " + contactDetails.Phone;
            return str;
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


    [TypeConverter(typeof(ContactDetailsConverter)),
    Description("Expand to see the contact details")]
    public class ContactDetails
    {


       
        [DisplayName("E-Mail")]
        [Description("The e-mail address")]
        [ReadOnly(true)]
        public string EMail { get; set; }


     
        [DisplayName("Web-page")]
        [Description("The web-page address")]
        [ReadOnly(true)]
        public string WebPage { get; set; }

     
        [DisplayName("Phone")]
        [Description("The phone number")]
        [ReadOnly(true)]
        public string Phone { get; set; }

       
        [DisplayName("FAX")]
        [Description("The fax number")]
        [ReadOnly(true)]
        public string FAX { get; set; }
    }

    
}
