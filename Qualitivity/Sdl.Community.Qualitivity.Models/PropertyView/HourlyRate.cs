using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class HourlyRateConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(HourlyRate) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is HourlyRate))
                return base.ConvertTo(context, culture, value, destinationType);
            var hourlyRate = (HourlyRate)value;

            var str = "Total: " + Math.Round(hourlyRate.HrCkd ? hourlyRate.hr_total : 0, 2);

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


    [TypeConverter(typeof(HourlyRateConverter)),
    Description("Expand to see the Default Rates")]
    public class HourlyRate
    {
        [DisplayName("Use")]
        [Description("use the hourly rate")]
        [ReadOnly(true)]
        public bool HrCkd { get; set; }
        [DisplayName("Hours")]
        [Description("The number of hours worked")]
        [ReadOnly(true)]
        public decimal hr_quantity { get; set; }

        [DisplayName("Hourly Rate")]
        [Description("The hourly rate")]
        [ReadOnly(true)]
        public decimal hr_rate { get; set; }

        [DisplayName("Currency")]
        [Description("The rate curr")]
        [ReadOnly(true)]
        public string hr_currency { get; set; }

        [DisplayName("Total")]
        [Description("The total (rate * hours)")]
        [ReadOnly(true)]
        public decimal hr_total { get; set; }
      

        
          
    }

    
}
