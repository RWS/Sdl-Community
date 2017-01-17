using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Structures;
using Sdl.Community.Studio.Time.Tracker.Tracking;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class ClientProfile : Form
    {

        public bool Saved { get; set; }
        public bool IsEdit { get; set; }
        public bool IsReadOnly { get; set; }

        public ClientProfileInfo ClientProfileInfo = new ClientProfileInfo();

        public ClientProfile()
        {
            InitializeComponent();
        }

        private void CompanyProfile_Load(object sender, EventArgs e)
        {
            Text += IsEdit ? " (Edit)" : " (New)";


            textBox_companyName.Text = ClientProfileInfo.ClientName.Trim();

            textBox_companyAddress.Text = ClientProfileInfo.AddressStreet + "\r\n"
                    + ClientProfileInfo.AddressZip + " "
                    + ClientProfileInfo.AddressCity
                    + (ClientProfileInfo.AddressState != string.Empty ? " (" + ClientProfileInfo.AddressState + ")" : string.Empty) + "\r\n"
                    + ClientProfileInfo.AddressCountry;

            textBox_companyTaxCode.Text = ClientProfileInfo.TaxCode.Trim();
            textBox_companyVatCode.Text = ClientProfileInfo.VatCode.Trim();

            textBox_internetEmail.Text = ClientProfileInfo.Email.Trim();
            textBox_internetWebPageAddress.Text = ClientProfileInfo.WebPage.Trim();
            textBox_companyPhoneNumber.Text = ClientProfileInfo.PhoneNumber.Trim();
            textBox_companyFaxNumber.Text = ClientProfileInfo.FaxNumber.Trim();

            textBox_companyName.Select();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            var foundName = Tracked.Preferences.Clients.Any(clientProfileInfo => 
                string.Compare(ClientProfileInfo.ClientName.Trim(), clientProfileInfo.ClientName.Trim(), StringComparison.OrdinalIgnoreCase) == 0 
                && ClientProfileInfo.Id != clientProfileInfo.Id);
            if (foundName)
            {
                MessageBox.Show(this, @"Unable to save the company profile; name already exists!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {

                ClientProfileInfo.ClientName = textBox_companyName.Text.Trim();

                ClientProfileInfo.TaxCode = textBox_companyTaxCode.Text;
                ClientProfileInfo.VatCode = textBox_companyVatCode.Text;

                ClientProfileInfo.Email = textBox_internetEmail.Text;
                ClientProfileInfo.WebPage = textBox_internetWebPageAddress.Text;
                ClientProfileInfo.PhoneNumber = textBox_companyPhoneNumber.Text;
                ClientProfileInfo.FaxNumber = textBox_companyFaxNumber.Text;


                Saved = true;
                Close();
            }
            
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void button_companyAddress_Click(object sender, EventArgs e)
        {
            var addressDetails = new AddressDetails
            {
                textBox_addressStreet = {Text = ClientProfileInfo.AddressStreet.Trim()},
                textBox_addressCity = {Text = ClientProfileInfo.AddressCity.Trim()},
                textBox_addressState = {Text = ClientProfileInfo.AddressState.Trim()},
                textBox_addressZip = {Text = ClientProfileInfo.AddressZip.Trim()},
                textBox_addressCountry = {Text = ClientProfileInfo.AddressCountry.Trim()}
            };


            addressDetails.ShowDialog();
            if (!addressDetails.Saved) return;
            ClientProfileInfo.AddressStreet = addressDetails.textBox_addressStreet.Text.Trim();
            ClientProfileInfo.AddressCity = addressDetails.textBox_addressCity.Text.Trim();
            ClientProfileInfo.AddressState = addressDetails.textBox_addressState.Text.Trim();
            ClientProfileInfo.AddressZip = addressDetails.textBox_addressZip.Text.Trim();
            ClientProfileInfo.AddressCountry = addressDetails.textBox_addressCountry.Text.Trim();


            textBox_companyAddress.Text = ClientProfileInfo.AddressStreet + "\r\n"
                                          + ClientProfileInfo.AddressZip + " "
                                          + ClientProfileInfo.AddressCity
                                          + (ClientProfileInfo.AddressState != string.Empty ? " (" + ClientProfileInfo.AddressState + ")" : string.Empty) + "\r\n"
                                          + ClientProfileInfo.AddressCountry;
        }

      
    }
}
