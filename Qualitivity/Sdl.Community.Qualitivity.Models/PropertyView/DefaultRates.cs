using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class DefaultRatesConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(DefaultRates) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is DefaultRates))
                return base.ConvertTo(context, culture, value, destinationType);
            var defaultRates = (DefaultRates)value;

            var str = "Hourly Rate: " +  defaultRates.HrRate + " (" + defaultRates.HrRateCurrency + ")";

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


    [TypeConverter(typeof(DefaultRatesConverter)),
    Description("Expand to see the Default Rates")]
    public class DefaultRates
    {
    
        [DisplayName("Hourly Rate")]
        [Description("The hourly rate")]
        [ReadOnly(true)]
        public decimal HrRate { get; set; }
        [DisplayName("Currency")]
        [Description("The rate curr")]
        [ReadOnly(true)]
        public string HrRateCurrency { get; set; }
        [DisplayName("Language Rate")]
        [Description("The language rate")]
        [ReadOnly(true)]
        public string PemRate { get; set; }

        
          
    }

    
}
