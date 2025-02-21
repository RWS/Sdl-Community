using System;
using System.Windows.Forms;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;

namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    public partial class CustomersDialog : Form
    {
        private readonly string _culture;
        public Customer SelectedCustomer { get; set; }
        public CustomersDialog(String culture)
        {
            _culture = culture;
            InitializeComponent();
            EnsureUiBasedOnCultureInfo(culture);
        }

        private void CustomersDialog_Load(object sender, EventArgs e)
        {
            gridCustomers.DataSource = Customers.GetAllCustomers();
        }

        private void EnsureUiBasedOnCultureInfo(String culture)
        {
            var resources = new UIResources(culture);
            grpCustomers.Text = resources.Customers;
            btnAdd.Text = resources.Add;
            btnSelect.Text = resources.Select;
            btnClose.Text = resources.Close;
            btnDelete.Text = resources.Delete;
            btnEdit.Text = resources.Edit;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (gridCustomers.SelectedRows.Count == 0)
                return;
            SelectedCustomer = Customers.GetCustomer(gridCustomers.SelectedRows[0].Cells[0].Value.ToString());
            DialogResult = DialogResult.OK;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addfrm = new AddOrEditCustomer(_culture, null);
            if (addfrm.ShowDialog() == DialogResult.OK)
                gridCustomers.DataSource = Customers.GetAllCustomers();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gridCustomers.SelectedRows.Count == 0)
                return;
            var addfrm = new AddOrEditCustomer(_culture, Customers.GetCustomer(gridCustomers.SelectedRows[0].Cells[0].Value.ToString()));
            if (addfrm.ShowDialog() == DialogResult.OK)
                gridCustomers.DataSource = Customers.GetAllCustomers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridCustomers.SelectedRows.Count == 0)
                return;
            Customers.DeleteCustomer(Customers.GetCustomer(gridCustomers.SelectedRows[0].Cells[0].Value.ToString()));
            gridCustomers.DataSource = Customers.GetAllCustomers();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
