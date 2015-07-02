// -----------------------------------------------------------------------
// <copyright file="EditApplyTemplates.cs" company="SDL plc">
// © 2014 SDL plc
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The edit project templates form
    /// </summary>
    public partial class EditApplyTemplates : Form
    {
        /// <summary>
        /// Indicates whether we are setting the name of the template
        /// </summary>
        private bool settingName = false;

        /// <summary>
        /// The template list
        /// </summary>
        private BindingList<ApplyTemplate> templateList = new BindingList<ApplyTemplate>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditApplyTemplates"/> class.
        /// </summary>
        public EditApplyTemplates()
        {
            this.InitializeComponent();
            this.ProjectTemplates.DataSource = this.ProjectTemplatesItems;
            this.ProjectTemplates.DisplayMember = "Name";
        }

        /// <summary>
        /// Gets the project templates items.
        /// </summary>
        /// <value>
        /// The project templates items.
        /// </value>
        public BindingList<ApplyTemplate> ProjectTemplatesItems
        {
            get
            {
                return this.templateList;
            }
        }

        /// <summary>
        /// Handles the TextChanged event of the ProjectTemplateName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ProjectTemplateName_TextChanged(object sender, EventArgs e)
        {
            if (this.ProjectTemplates.SelectedItem != null)
            {
                this.settingName = true;
                (this.ProjectTemplates.SelectedItem as ApplyTemplate).Name = this.ProjectTemplateName.Text;
                this.settingName = false;
                this.ProjectTemplates.DisplayMember = string.Empty;
                this.ProjectTemplates.DisplayMember = "Name";
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ProjectTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ProjectTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.settingName)
            {
                if (this.ProjectTemplates.SelectedIndex < 0)
                {
                    this.ProjectTemplateName.Text = string.Empty;
                }
                else
                {
                    string templateName = (this.ProjectTemplates.SelectedItem as ApplyTemplate).Name;
                    if (this.ProjectTemplateName.Text != templateName)
                    {
                        this.ProjectTemplateName.Text = templateName;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RemoveTemplate_Click(object sender, EventArgs e)
        {
            if (this.ProjectTemplates.SelectedItem != null)
            {
                this.ProjectTemplatesItems.Remove(this.ProjectTemplates.SelectedItem as ApplyTemplate);
            }
        }

        /// <summary>
        /// Handles the Click event of the ClearTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ClearTemplates_Click(object sender, EventArgs e)
        {
            this.ProjectTemplatesItems.Clear();
        }

        /// <summary>
        /// Handles the Click event of the AddTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AddTemplate_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "SDL Trados Studio Templates|*.sdltpl|SDL Trados Studio Projects|*.sdlproj|All Files|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;
            ofd.Title = "Add template";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ApplyTemplate newTemplate = this.ProjectTemplatesItems.AddNew();
                newTemplate.Name = Path.GetFileNameWithoutExtension(ofd.FileName);
                newTemplate.FileLocation = ofd.FileName;
                newTemplate.Uri = null;
                newTemplate.Id = Guid.NewGuid();
                this.ProjectTemplates.SelectedItem = newTemplate;
                this.ProjectTemplates.DisplayMember = string.Empty;
                this.ProjectTemplates.DisplayMember = "Name";
            }
        }
    }
}
