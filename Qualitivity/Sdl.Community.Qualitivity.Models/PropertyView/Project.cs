using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.Structures.PropertyView
{
    public class ProjectConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Project) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string) || !(value is Project))
                return base.ConvertTo(context, culture, value, destinationType);
            var project = (Project)value;

            return project.ProjectName;
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


    [TypeConverter(typeof(ProjectConverter)),
    Description("Expand to see the Project")]
    public class Project
    {


        [DisplayName("ID")]
        [Description("The ID of the project")]
        [ReadOnly(true)]
        public string ProjectId { get; set; }

        [DisplayName("ID {Studio}")]
        [Description("The ID of the studio project")]
        [ReadOnly(true)]
        public string ProjectIdStudio { get; set; }



        [DisplayName("Name")]
        [Description("The name of the project")]
        [ReadOnly(true)]
        public string ProjectName { get; set; }


        [DisplayName("Description")]
        [Description("The desc of the project")]
        [ReadOnly(true)]
        public string ProjectDescription { get; set; }




        [DisplayName("Status")]
        [Description("The status of the project")]
        [ReadOnly(true)]
        public string ProjectStatus { get; set; }



        [DisplayName("Created")]
        [Description("The created date of the project")]
        [ReadOnly(true)]
        public DateTime? ProjectDateCreated { get; set; }


        [DisplayName("Due")]
        [Description("The due date of the project")]
        [ReadOnly(true)]
        public DateTime? ProjectDateDue { get; set; }


        [DisplayName("Completed")]
        [Description("The completed date of the project")]
        [ReadOnly(true)]
        public DateTime? ProjectDateComplated { get; set; }



        [DisplayName("Activities Count")]
        [Description("The number of activies for the project")]
        [ReadOnly(true)]
        public string ProjectActivitesCount { get; set; }

    }

    
}
