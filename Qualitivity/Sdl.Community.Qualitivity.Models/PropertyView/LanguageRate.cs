using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class LanguageRateConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(LanguageRateGroup) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is LanguageRateGroup))
                return base.ConvertTo(context, culture, value, destinationType);
            var languageRateGroup = (LanguageRateGroup)value;

            var str = "Total: " + Math.Round(languageRateGroup.PemCkd ? languageRateGroup.PemTotal : 0, 2);

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


    [TypeConverter(typeof(LanguageRateConverter)),
    Description("Expand to see the Default Rates")]
    public class LanguageRateGroup
    {
        [DisplayName("Use")]
        [Description("use the language rate")]
        [ReadOnly(true)]
        public bool PemCkd { get; set; }

        [DisplayName("Language Rate")]
        [Description("The selected language rate")]
        [ReadOnly(true)]
        public string PemName { get; set; }

        
        [DisplayName("Currency")]
        [Description("The language rate curr")]
        [ReadOnly(true)]
        public string PemCurrency { get; set; }


        [DisplayName("Total")]
        [Description("The language rate total")]
        [ReadOnly(true)]
        public decimal PemTotal { get; set; }
      

        
          
    }

    
}
