using System;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls
{
    public partial class ProjectForm
    {
        private void ProjectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// Makes form close when pressing Esc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }

        /// <summary>
        /// Makes checkboxes enable/disable their corresponding controls
        /// </summary>
        private void SetBindings()
        {
            _startPublishingDate.DataBindings.Add("Enabled", _projectPubDate_checkBox, "Checked");
            _endPublishingDate.DataBindings.Add("Enabled", _projectPubDate_checkBox, "Checked");
            _startDeliveryDate.DataBindings.Add("Enabled", _projectDelDate_checkBox, "Checked");
            _endDeliveryDate.DataBindings.Add("Enabled", _projectDelDate_checkBox, "Checked");

            foreach (Control control in _projectStatus_groupBox.Controls)
            {
                if (control.Name == "_projectStatus_checkBox") continue;
                control.DataBindings.Add("Enabled", _projectStatus_checkBox, "Checked");
            }
        }

        /// <summary>
        /// Default time interval that reflects GS website's time interval
        /// </summary>
        private void SetDefaultTimeInterval()
        {
            _startPublishingDate.Value = DateTime.Today.AddDays(-7);
            _startDeliveryDate.Value = DateTime.Today.AddDays(-7);
            _endDeliveryDate.Value = DateTime.Now.AddMonths(6);
        }
    }
}