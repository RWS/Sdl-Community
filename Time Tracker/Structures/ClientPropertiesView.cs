using System;
using System.ComponentModel;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ClientPropertiesView : ICloneable
    {
        #region  |  client  |

        [Category("Client")]
        [DisplayName("Client ID")]
        [Description("The ID of the client")]
        [ReadOnly(true)]
        public string ClientId { get; set; }

        [Category("Client")]
        [DisplayName("Client name")]
        [Description("The Name of the client")]
        [ReadOnly(true)]
        public string ClientName { get; set; }




        [Category("Client")]
        [DisplayName("Client Address")]
        [Description("The address of the client")]
        [ReadOnly(true)]
        public string ClientAddress { get; set; }


        [Category("Client")]
        [DisplayName("Client TAX Nr.")]
        [Description("The TAX number of the client")]
        [ReadOnly(true)]
        public string ClientTax { get; set; }


        [Category("Client")]
        [DisplayName("Client VAT Nr.")]
        [Description("The VAT number of the client")]
        [ReadOnly(true)]
        public string ClientVat { get; set; }

        [Category("Client")]
        [DisplayName("Client E-Mail")]
        [Description("The e-mail address of the client")]
        [ReadOnly(true)]
        public string ClientEMail { get; set; }


        [Category("Client")]
        [DisplayName("Client Web-page")]
        [Description("The web-page address of the client")]
        [ReadOnly(true)]
        public string ClientWebPage { get; set; }

        [Category("Client")]
        [DisplayName("Client phone number")]
        [Description("The phone number of the client")]
        [ReadOnly(true)]
        public string ClientPhone { get; set; }

        [Category("Client")]
        [DisplayName("Client fax number")]
        [Description("The fax number of the client")]
        [ReadOnly(true)]
        public string ClientFax { get; set; }



        #endregion
     

        public ClientPropertiesView()
        {

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
