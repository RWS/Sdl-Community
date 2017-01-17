using System;
using System.ComponentModel;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ActivityPropertiesView : ICloneable
    {
        #region  |  client  |

        [Category("Client")]
        [DisplayName("Client ID")]
        [Description("The ID of the client")]
        [ReadOnly(true)]
        public string clientId { get; set; }

        [Category("Client")]
        [DisplayName("Client name")]
        [Description("The Name of the client")]
        [ReadOnly(true)]
        public string clientName { get; set; }




        [Category("Client")]
        [DisplayName("Client Address")]
        [Description("The address of the client")]
        [ReadOnly(true)]
        public string clientAddress { get; set; }


        [Category("Client")]
        [DisplayName("Client TAX Nr.")]
        [Description("The TAX number of the client")]
        [ReadOnly(true)]
        public string clientTAX { get; set; }


        [Category("Client")]
        [DisplayName("Client VAT Nr.")]
        [Description("The VAT number of the client")]
        [ReadOnly(true)]
        public string clientVAT { get; set; }

        [Category("Client")]
        [DisplayName("Client E-Mail")]
        [Description("The e-mail address of the client")]
        [ReadOnly(true)]
        public string clientEMail { get; set; }


        [Category("Client")]
        [DisplayName("Client Web-page")]
        [Description("The web-page address of the client")]
        [ReadOnly(true)]
        public string clientWebPage { get; set; }

        [Category("Client")]
        [DisplayName("Client phone number")]
        [Description("The phone number of the client")]
        [ReadOnly(true)]
        public string clientPhone { get; set; }

        [Category("Client")]
        [DisplayName("Client fax number")]
        [Description("The fax number of the client")]
        [ReadOnly(true)]
        public string clientFAX { get; set; }



        #endregion


        #region  |  project  |

        [Category("Project")]
        [DisplayName("Project ID")]
        [Description("The ID of the project")]
        [ReadOnly(true)]
        public string projectId { get; set; }


        [Category("Project")]
        [DisplayName("Project Name")]
        [Description("The name of the project")]
        [ReadOnly(true)]
        public string projectName { get; set; }

        [Category("Project")]
        [DisplayName("Project Description")]
        [Description("The description of the project")]
        [ReadOnly(true)]
        public string projectDescription { get; set; }



        [Category("Project")]
        [DisplayName("Project Status")]
        [Description("The status of the project")]
        [ReadOnly(true)]
        public string projectStatus { get; set; }


        [Category("Project")]
        [DisplayName("Project Created")]
        [Description("The created date of the project")]
        [ReadOnly(true)]
        public string projectDateCreated { get; set; }

        [Category("Project")]
        [DisplayName("Project Due")]
        [Description("The due date of the project")]
        [ReadOnly(true)]
        public string projectDateDue { get; set; }

        [Category("Project")]
        [DisplayName("Project Completed")]
        [Description("The completed date of the project")]
        [ReadOnly(true)]
        public string projectDateComplated { get; set; }


        [Category("Project")]
        [DisplayName("Project Activities Count")]
        [Description("The number of activies for the project")]
        [ReadOnly(true)]
        public string projectActivitesCount { get; set; }
        

        #endregion


        #region  |  activity  |

        [Category("Activity")]
        [DisplayName("Activity ID")]
        [Description("The Name of the project activity")]
        [ReadOnly(true)]
        public string activityId { get; set; }

        [Category("Activity")]
        [DisplayName("Activity Name")]
        [Description("The Name of the project activity")]
        [ReadOnly(true)]
        public string activityName { get; set; }


        [Category("Activity")]
        [DisplayName("Activity Description")]
        [Description("The description of the project activity")]
        [ReadOnly(true)]
        public string activityDescription { get; set; }



        [Category("Activity")]
        [DisplayName("Activity Status")]
        [Description("The status of project activity")]
        [ReadOnly(true)]
        public string activityStatus { get; set; }


        [Category("Activity")]
        [DisplayName("Activity Type")]
        [Description("The type of project activity")]
        [ReadOnly(true)]
        public string activityType { get; set; }



        [Category("Activity")]
        [DisplayName("Activity Billable")]
        [Description("The billable status of project activity")]
        [ReadOnly(true)]
        public string activityBillable { get; set; }



        [Category("Activity")]
        [DisplayName("Activity Invoiced")]
        [Description("The invoiced status of project activity")]
        [ReadOnly(true)]
        public string activityInvoiced { get; set; }


        [Category("Activity")]
        [DisplayName("Activity Invoiced Date")]
        [Description("The invoiced date of project activity")]
        [ReadOnly(true)]
        public string activityInvoicedDate { get; set; }





        [Category("Activity")]
        [DisplayName("Activity Start")]
        [Description("The start date/time of project activity")]
        [ReadOnly(true)]
        public string activityDateStart { get; set; }

        [Category("Activity")]
        [DisplayName("Activity End")]
        [Description("The end date/time of project activity")]
        [ReadOnly(true)]
        public string activityDateEnd { get; set; }

        [Category("Activity")]
        [DisplayName("Activity Hours")]
        [Description("The quantity of hours set for the project activity")]
        [ReadOnly(true)]
        public string activityHours { get; set; }


        [Category("Activity")]
        [DisplayName("Activity Rate")]
        [Description("The price rate for the project activity")]
        [ReadOnly(true)]
        public string activityRate { get; set; }

        [Category("Activity")]
        [DisplayName("Activity Rate Adjustment")]
        [Description("The price rate adjustment for the project activity")]
        [ReadOnly(true)]
        public string activityRateAdjustment { get; set; }

        [Category("Activity")]
        [DisplayName("Activity Total")]
        [Description("The total price for the project activity")]
        [ReadOnly(true)]
        public string activityTotal { get; set; }

        [Category("Activity")]
        [DisplayName("Activity Currency")]
        [Description("The currency for the project activity")]
        [ReadOnly(true)]
        public string activityCurrency { get; set; }




        #endregion

        public ActivityPropertiesView()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
