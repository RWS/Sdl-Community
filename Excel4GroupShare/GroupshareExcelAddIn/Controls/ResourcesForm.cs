using System;
using System.Linq;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls
{
    public partial class ResourcesForm : Form
    {
        public ResourcesForm(UserControl[] controls)
        {
            if (controls == null) throw new ArgumentNullException(nameof(controls));

            InitializeComponent();
            controls.ToList().ForEach(ctrl => ctrl.Visible = false);
            _dataTypeComboBox.Items.AddRange(controls);
            _dataTypeComboBox.DisplayMember = "Name";

            _tableLayoutPanel.Controls.AddRange(controls);
            DockControls();

            AttachEventHandlers(controls);
        }

        private void _dataTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((UserControl)_dataTypeComboBox.SelectedItem).Visible = true;
            Refresh();
        }

        private void AttachEventHandlers(UserControl[] controls)
        {
            foreach (var userControl in controls)
            {
                //((ResourcesControl)userControl).GetDataTriggered += (sender, args) => Hide();
            }
        }

        private void DockControls()
        {
            foreach (Control control in _tableLayoutPanel.Controls)
            {
                control.Dock = DockStyle.Fill;
            }
        }

        private void ResourcesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}