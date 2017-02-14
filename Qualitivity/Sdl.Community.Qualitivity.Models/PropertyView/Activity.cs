using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class ActivityConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Activity) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is Activity))
                return base.ConvertTo(context, culture, value, destinationType);
            var activity = (Activity)value;

            return activity.ActivityName;
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


    [TypeConverter(typeof(ActivityConverter)),
    Description("Expand to see the Activity")]
    public class Activity
    {
       
        [DisplayName("ID")]
        [Description("The id of the project activity")]
        [ReadOnly(true)]
        public string ActivityId { get; set; }

        [DisplayName("Name")]
        [Description("The Name of the project activity")]
        [ReadOnly(true)]
        public string ActivityName { get; set; }

        [DisplayName("Description")]
        [Description("The desc of the project activity")]
        [ReadOnly(true)]
        public string ActivityDescription { get; set; }

        [DisplayName("Documents")]
        [Description("The activity documents")]
        [ReadOnly(true)]
        public int ActivityDocuments { get; set; }

        [DisplayName("Status")]
        [Description("The status of project activity")]
        [ReadOnly(true)]
        public string ActivityStatus { get; set; }


        [DisplayName("Billable")]
        [Description("The billable status of project activity")]
        [ReadOnly(true)]
        public bool ActivityBillable { get; set; }



        [DisplayName("Start")]
        [Description("The started date/time of project activity")]
        [ReadOnly(true)]
        public DateTime? ActivityDateStart { get; set; }

       
        [DisplayName("End")]
        [Description("The end date/time of project activity")]
        [ReadOnly(true)]
        public DateTime? ActivityDateEnd { get; set; }

       
        [DisplayName("Hourly Rate")]
        [Description("The quantity of hours set for the project activity")]
        [ReadOnly(true)]
        public HourlyRate HourlyRate { get; set; }


       
        [DisplayName("Language Rate")]
        [Description("The language rate for the project activity")]
        [ReadOnly(true)]
        public LanguageRateGroup PemRate { get; set; }

       

        [DisplayName("Total")]
        [Description("The total cost for the activity")]
        [ReadOnly(true)]
        public string ActivityTotal { get; set; }


       

      
    }

    
}
