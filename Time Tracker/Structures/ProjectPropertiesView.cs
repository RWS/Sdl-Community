using System;
using System.ComponentModel;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ProjectPropertiesView : ICloneable
    {
        

        #region  |  project  |

        [Category("Project")]
        [DisplayName("Project ID")]
        [Description("The ID of the project")]
        [ReadOnly(true)]
        public string ProjectId { get; set; }


        [Category("Project")]
        [DisplayName("Project Name")]
        [Description("The name of the project")]
        [ReadOnly(true)]
        public string ProjectName { get; set; }

        [Category("Project")]
        [DisplayName("Project Description")]
        [Description("The description of the project")]
        [ReadOnly(true)]
        public string ProjectDescription { get; set; }



        [Category("Project")]
        [DisplayName("Project Status")]
        [Description("The status of the project")]
        [ReadOnly(true)]
        public string ProjectStatus { get; set; }


        [Category("Project")]
        [DisplayName("Project Created")]
        [Description("The created date of the project")]
        [ReadOnly(true)]
        public string ProjectDateCreated { get; set; }

        [Category("Project")]
        [DisplayName("Project Due")]
        [Description("The due date of the project")]
        [ReadOnly(true)]
        public string ProjectDateDue { get; set; }

        [Category("Project")]
        [DisplayName("Project Completed")]
        [Description("The completed date of the project")]
        [ReadOnly(true)]
        public string ProjectDateComplated { get; set; }


        [Category("Project")]
        [DisplayName("Project Activities Count")]
        [Description("The number of activies for the project")]
        [ReadOnly(true)]
        public string ProjectActivitesCount { get; set; }
        

        #endregion


        public ProjectPropertiesView()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
