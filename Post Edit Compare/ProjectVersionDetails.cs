using System;
using System.ComponentModel;
using System.Globalization;

namespace Sdl.Community.PostEdit.Versions
{
    [Serializable]
    public class ProjectVersionDetails : ICloneable
    {

        
        [Category("Project")]
        [DisplayName("ID")]
        [Description("The ID of the project")]
        [ReadOnly(true)]
        public string ProjectId { get; set; }

        [Category("Project")]
        [DisplayName("Name")]
        [Description("The Name of the project")]
        [ReadOnly(true)]
        public string ProjectName { get; set; }

        [Category("Project")]
        [DisplayName("Description")]
        [Description("The Description of the project")]
        [ReadOnly(true)]
        public string ProjectDescription { get; set; }

        [Category("Project")]
        [DisplayName("Location")]
        [Description("The Location of the project")]
        [ReadOnly(true)]
        public string ProjectLocation { get; set; }

        [Category("Project")]
        [DisplayName("Created At")]
        [Description("The creation date of the project")]
        [ReadOnly(true)]
        public string ProjectCreatedAt { get; set; }


        [Category("Project")]
        [DisplayName("Created By")]
        [Description("The user that created the project")]
        [ReadOnly(true)]
        public string ProjectCreatedBy { get; set; }



        [Category("Project Version")]
        [DisplayName("ID")]
        [Description("The ID the project version")]
        [ReadOnly(true)]
        public string ProjectVersionId { get; set; }

        [Category("Project Version")]
        [DisplayName("Name")]
        [Description("The Name of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionName { get; set; }


        [Category("Project Version")]
        [DisplayName("Description")]
        [Description("The Description of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionDescription { get; set; }


        [Category("Project Version")]
        [DisplayName("Location")]
        [Description("The Location of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionLocation { get; set; }



        [Category("Project Version")]
        [DisplayName("Created At")]
        [Description("The creation date of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionCreatedAt { get; set; }


        [Category("Project Version")]
        [DisplayName("Created By")]
        [Description("The user that created the project version")]
        [ReadOnly(true)]
        public string ProjectVersionCreatedBy { get; set; }



        [Category("Project Version")]
        [DisplayName("Source Language")]
        [Description("Source language of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionSourceLanguage { get; set; }


        [Category("Project Version")]
        [DisplayName("Target Languages")]
        [Description("Target languages of the project version")]
        [ReadOnly(true)]
        public string ProjectVersionTargetLanguages { get; set; }


        [Category("Project Version")]
        [DisplayName("Total Files")]
        [Description("Total files included in the project version")]
        [ReadOnly(true)]
        public string ProjectVersionTotalFiles { get; set; }


        [Category("Project Version")]
        [DisplayName("Shallow Copy")]
        [Description("Shallow Copy means that only the *.sdlxliff && *.sdlproj files are included in the project version\r\n"
            + "Note: if this option is 'false' then all local 'project' files are included during project version creation.")]
        [ReadOnly(true)]
        public string ProjectVersionShallowCopy { get; set; }


        [Category("Project Version")]
        [DisplayName("Translatable Files")]
        [Description("Total translatable source files")]
        [ReadOnly(true)]
        public string ProjectVersionTranslatableCount { get; set; }

        [Category("Project Version")]
        [DisplayName("Reference Files")]
        [Description("Total reference source files")]
        [ReadOnly(true)]
        public string ProjectVersionReferenceCount { get; set; }

        [Category("Project Version")]
        [DisplayName("Localizable Files")]
        [Description("Total localizable source files")]
        [ReadOnly(true)]
        public string ProjectVersionLocalizableCount { get; set; }

         
        [Category("Project Version")]
        [DisplayName("Unknown Files")]
        [Description("Total unknown source files")]
        [ReadOnly(true)]
        public string ProjectVersionUnknownCount { get; set; }


        public ProjectVersionDetails()
        {
            ProjectId = string.Empty;
            ProjectName = string.Empty;
            ProjectDescription = string.Empty;
            ProjectLocation = string.Empty;
            ProjectCreatedAt = string.Empty;
            ProjectCreatedBy = string.Empty;
      
            ProjectVersionId = string.Empty;
            ProjectVersionName = string.Empty;
            ProjectVersionDescription = string.Empty;
            ProjectVersionLocation = string.Empty;
            ProjectVersionCreatedAt = string.Empty;
            ProjectVersionCreatedBy = string.Empty;
          
            ProjectVersionSourceLanguage = string.Empty;
            ProjectVersionTargetLanguages = string.Empty;
            ProjectVersionTotalFiles = "0";
            ProjectVersionShallowCopy = "True";

            ProjectVersionTranslatableCount = "0";
            ProjectVersionReferenceCount = "0";
            ProjectVersionLocalizableCount = "0";
            ProjectVersionUnknownCount = "0";

        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    internal class MySourceFilesCounterConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                             CultureInfo culture,
                             object value, Type destType)
        {

            return "...";
            //return base.ConvertTo(context, culture, value, destType);
        }
    }

    internal class MyTargetLanguageConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                             CultureInfo culture,
                             object value, Type destType)
        {

            return "...";
            //return base.ConvertTo(context, culture, value, destType);
        }
    }

    [Serializable]
    public class SourceFilesCounter : ICloneable
    {
        //[Category("Project")]
        [DisplayName("Translatable")]
        [Description("Total translatable source files")]
        [ReadOnly(true)]
        public string TranslatableCount { get; set; }

        [DisplayName("Reference")]
        [Description("Total reference source files")]
        [ReadOnly(true)]
        public string ReferenceCount { get; set; }

        [DisplayName("Localizable")]
        [Description("Total localizable source files")]
        [ReadOnly(true)]
        public string LocalizableCount { get; set; }

        [DisplayName("Unknown")]
        [Description("Total unknown source files")]
        [ReadOnly(true)]
        public string UnknownCount { get; set; }

        public SourceFilesCounter()
        {
            TranslatableCount = "0";
            ReferenceCount = "0";
            LocalizableCount = "0";
            UnknownCount = "0";
        }
        public object Clone()
        {
            return MemberwiseClone();
        }

    }

    [Serializable]
    public class LanguageProperty : ICloneable
    {
        [DisplayName("Name")]
        [Description("Language Name")]
        [ReadOnly(true)]
        public string Name { get; set; }
        public LanguageProperty(string name)
        {
            Name = name;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}



