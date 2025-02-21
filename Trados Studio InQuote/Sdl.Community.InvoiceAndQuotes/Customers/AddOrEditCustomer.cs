using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    public partial class AddOrEditCustomer : Form
    {
        private NoLabelTextBox _txtCustomerName;
        private NoLabelTextBox _txtCustomerStreet;
        private NoLabelTextBox _txtCustomerCountry;
        private NoLabelTextBox _txtCustomerZip;
        private NoLabelTextBox _txtCustomerState;
        private NoLabelTextBox _txtCustomerCity;
        private Rates _rates;
        private readonly Customer _customer;

        public AddOrEditCustomer(String culture, Customer customer)
        {
            _customer = customer;
            InitializeComponent();
            InitializeCustomControls();
            EnsureUiBasedOnCultureInfo(culture, customer);
        }
        private void InitializeCustomControls()
        {
            // 
            // txtCustomerCountry
            // 
            _txtCustomerCountry = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "Country:",
                Location = new Point(5, 255),
                Name = "txtCustomerCountry",
                Size = new Size(301, 20),
                TabIndex = 6
            };
            // 
            // txtCustomerZip
            // 
            _txtCustomerZip = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "Zip/Postal Code:",
                Location = new Point(5, 229),
                Name = "txtCustomerZip",
                Size = new Size(301, 20),
                TabIndex = 5
            };
            // 
            // txtCustomerState
            // 
            _txtCustomerState = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "State/Province:",
                Location = new Point(5, 203),
                Name = "txtCustomerState",
                Size = new Size(301, 20),
                TabIndex = 4
            };
            // 
            // txtCustomerCity
            // 
            _txtCustomerCity = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "City:",
                Location = new Point(5, 177),
                Name = "txtCustomerCity",
                Size = new Size(301, 20),
                TabIndex = 3
            };
            // 
            // txtCustomerStreet
            // 
            _txtCustomerStreet = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "Street:",
                Location = new Point(5, 74),
                Multiline = true,
                Name = "txtCustomerStreet",
                Size = new Size(301, 97),
                TabIndex = 2
            };

            // 
            // txtCustomerName
            // 
            _txtCustomerName = new NoLabelTextBox
            {
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                ForeColor = SystemColors.InactiveCaption,
                LabelText = "Customer name:",
                Location = new Point(6, 48),
                Name = "txtCustomerName",
                Size = new Size(301, 20),
                TabIndex = 0
            };
            
            groupCustomer.Controls.Add(_txtCustomerCountry);
            groupCustomer.Controls.Add(_txtCustomerZip);
            groupCustomer.Controls.Add(_txtCustomerState);
            groupCustomer.Controls.Add(_txtCustomerCity);
            groupCustomer.Controls.Add(_txtCustomerStreet);
            groupCustomer.Controls.Add(_txtCustomerName);
        }

        private void EnsureUiBasedOnCultureInfo(String culture, Customer customer)
        {
            var resources = new UIResources(culture);

            groupCustomer.Text = resources.Customer;
            _txtCustomerName.LabelText = resources.CustomerName;
            _txtCustomerStreet.LabelText = resources.Street;
            _txtCustomerCountry.LabelText = resources.Country;
            _txtCustomerZip.LabelText = resources.Zip;
            _txtCustomerState.LabelText = resources.State;
            _txtCustomerCity.LabelText = resources.City;

            _txtCustomerName.SDLText = customer != null ? customer.Name : String.Empty;
            _txtCustomerStreet.SDLText = customer != null ? customer.Street : String.Empty;
            _txtCustomerCountry.SDLText = customer != null ? customer.Country : String.Empty;
            _txtCustomerZip.SDLText = customer != null ? customer.Zip : String.Empty;
            _txtCustomerState.SDLText = customer != null ? customer.State : String.Empty;
            _txtCustomerCity.SDLText = customer != null ? customer.City : String.Empty;

            groupStudioAnalysisBands.Text = resources.StudioAnalysisBands;
            groupTemplateTemplates.Text = resources.Templates;

            rbTemplateGroupedAnalysis.Text = resources.GroupedAnalysis;
            rbTemplateSimpleWordAnalysis.Text = resources.SimpleWordAnalysis;
            rbTemplateStandardLines.Text = resources.StandardLines;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var customer = new Customer
            {
                    Name = _txtCustomerName.SDLText,
                    Street = _txtCustomerStreet.SDLText,
                    City = _txtCustomerCity.SDLText,
                    Zip = _txtCustomerZip.SDLText,
                    State = _txtCustomerState.SDLText,
                    Country = _txtCustomerCountry.SDLText
                };
            customer.Rates = _rates.State;
            Customers.SaveCustomer(customer.Name, customer);
           
            DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void AddOrEditCustomer_Load(object sender, EventArgs e)
        {
            _rates = new Rates { Name = "rates", Dock = DockStyle.Fill, Visible = true };
            groupStudioAnalysisBands.Controls.Add(_rates);
            if (_customer != null)
                _rates.State = _customer.Rates;
            rbTemplateSimpleWordAnalysis.Checked = true;
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.Equals(rbTemplateStandardLines))
                _rates.BindRates(new StandardLinesTemplate());

            if (sender.Equals(rbTemplateGroupedAnalysis))
                _rates.BindRates(new GroupedAnalysisTemplate());

            if (sender.Equals(rbTemplateSimpleWordAnalysis))
                _rates.BindRates(new SimpleWordTemplate());
        }
    }
}
