using System;
using System.ComponentModel;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ClientProjectPropertiesView : ICloneable
    {
        #region  |  client  |

        [Category("Client")]
        [DisplayName(@"Client ID")]
        [Description("The ID of the client")]
        [ReadOnly(true)]
        public string ClientId { get; set; }

        [Category("Client")]
        [DisplayName(@"Client name")]
        [Description("The Name of the client")]
        [ReadOnly(true)]
        public string ClientName { get; set; }




        [Category("Client")]
        [DisplayName(@"Client Address")]
        [Description("The address of the client")]
        [ReadOnly(true)]
        public string ClientAddress { get; set; }


        [Category("Client")]
        [DisplayName(@"Client TAX Nr.")]
        [Description("The TAX number of the client")]
        [ReadOnly(true)]
        public string ClientTax { get; set; }


        [Category("Client")]
        [DisplayName(@"Client VAT Nr.")]
        [Description("The VAT number of the client")]
        [ReadOnly(true)]
        public string ClientVat { get; set; }

        [Category("Client")]
        [DisplayName(@"Client E-Mail")]
        [Description("The e-mail address of the client")]
        [ReadOnly(true)]
        public string ClientEMail { get; set; }


        [Category("Client")]
        [DisplayName(@"Client Web-page")]
        [Description("The web-page address of the client")]
        [ReadOnly(true)]
        public string ClientWebPage { get; set; }

        [Category("Client")]
        [DisplayName(@"Client phone number")]
        [Description("The phone number of the client")]
        [ReadOnly(true)]
        public string ClientPhone { get; set; }

        [Category("Client")]
        [DisplayName(@"Client fax number")]
        [Description("The fax number of the client")]
        [ReadOnly(true)]
        public string ClientFax { get; set; }



        #endregion


        #region  |  project  |

        [Category("Project")]
        [DisplayName(@"Project ID")]
        [Description("The ID of the project")]
        [ReadOnly(true)]
        public string ProjectId { get; set; }


        [Category("Project")]
        [DisplayName(@"Project Name")]
        [Description("The name of the project")]
        [ReadOnly(true)]
        public string ProjectName { get; set; }

        [Category("Project")]
        [DisplayName(@"Project Description")]
        [Description("The description of the project")]
        [ReadOnly(true)]
        public string ProjectDescription { get; set; }



        [Category("Project")]
        [DisplayName(@"Project Status")]
        [Description("The status of the project")]
        [ReadOnly(true)]
        public string ProjectStatus { get; set; }


        [Category("Project")]
        [DisplayName(@"Project Created")]
        [Description("The created date of the project")]
        [ReadOnly(true)]
        public string ProjectDateCreated { get; set; }

        [Category("Project")]
        [DisplayName(@"Project Due")]
        [Description("The due date of the project")]
        [ReadOnly(true)]
        public string ProjectDateDue { get; set; }

        [Category("Project")]
        [DisplayName(@"Project Completed")]
        [Description("The completed date of the project")]
        [ReadOnly(true)]
        public string ProjectDateComplated { get; set; }


        [Category("Project")]
        [DisplayName(@"Project Activities Count")]
        [Description("The number of activies for the project")]
        [ReadOnly(true)]
        public string ProjectActivitesCount { get; set; }
        

        #endregion


        public ClientProjectPropertiesView()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
