using System;
using System.ComponentModel;

namespace Sdl.Community.Structures.PropertyView
{
    [Serializable]
    public class ActivityPropertiesView : ICloneable
    {
        #region  |  client  |

        [Category("Client")]
        [DisplayName("Client Profile")]
        [Description("The client profile")]
        [ReadOnly(true)]
        public Client ClientProfile { get; set; }



        #endregion

        #region  |  project  |

        [Category("Project")]
        [DisplayName("Project Details")]
        [Description("The details for the project")]
        [ReadOnly(true)]
        public Project ProjectDetails { get; set; }



        #endregion


        #region  |  activity  |

        [Category("Activity")]
        [DisplayName("Activity Details")]
        [Description("The details for the activity")]
        [ReadOnly(true)]
        public Activity ActivityDetails { get; set; }



        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
