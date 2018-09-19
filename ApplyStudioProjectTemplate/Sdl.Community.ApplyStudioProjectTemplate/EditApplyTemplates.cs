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
        private bool settingName;

        /// <summary>
        /// The template list
        /// </summary>
        private BindingList<ApplyTemplate> _templateList = new BindingList<ApplyTemplate>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditApplyTemplates"/> class.
        /// </summary>
        public EditApplyTemplates()
        {
            InitializeComponent();
            ProjectTemplates.DataSource = ProjectTemplatesItems;
            ProjectTemplates.DisplayMember = "Name";
        }

	    /// <summary>
	    /// Gets the project templates items.
	    /// </summary>
	    /// <value>
	    /// The project templates items.
	    /// </value>
	    public BindingList<ApplyTemplate> ProjectTemplatesItems => _templateList;
     

        /// <summary>
        /// Handles the TextChanged event of the ProjectTemplateName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ProjectTemplateName_TextChanged(object sender, EventArgs e)
        {
            if (ProjectTemplates.SelectedItem != null)
            {
                settingName = true;
	            var applyTemplate = ProjectTemplates.SelectedItem as ApplyTemplate;
	            if (applyTemplate != null)
	            {
					applyTemplate.Name = ProjectTemplateName.Text;
				}
	            settingName = false;
                ProjectTemplates.DisplayMember = string.Empty;
                ProjectTemplates.DisplayMember = "Name";
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ProjectTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ProjectTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingName)
            {
                if (ProjectTemplates.SelectedIndex < 0)
                {
                    ProjectTemplateName.Text = string.Empty;
                }
                else
                {
	                var applyTemplate = ProjectTemplates.SelectedItem as ApplyTemplate;
	                if (applyTemplate != null)
	                {
		                var templateName = applyTemplate.Name;
		                if (ProjectTemplateName.Text != templateName)
		                {
			                ProjectTemplateName.Text = templateName;
		                }
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
            if (ProjectTemplates.SelectedItem != null)
            {
                ProjectTemplatesItems.Remove(ProjectTemplates.SelectedItem as ApplyTemplate);
            }
        }

        /// <summary>
        /// Handles the Click event of the ClearTemplates control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ClearTemplates_Click(object sender, EventArgs e)
        {
            ProjectTemplatesItems.Clear();
        }

        /// <summary>
        /// Handles the Click event of the AddTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AddTemplate_Click(object sender, EventArgs e)
        {
	        var ofd = new OpenFileDialog
	        {
		        CheckFileExists = true,
		        Filter = @"SDL Trados Studio Templates|*.sdltpl|SDL Trados Studio Projects|*.sdlproj|All Files|*.*",
		        FilterIndex = 1,
		        Multiselect = false,
		        Title = @"Add template"
	        };
	        if (ofd.ShowDialog() == DialogResult.OK)
            {
                var newTemplate = ProjectTemplatesItems.AddNew();
	            if (newTemplate != null)
	            {
		            newTemplate.Name = Path.GetFileNameWithoutExtension(ofd.FileName);
		            newTemplate.FileLocation = ofd.FileName;
		            newTemplate.Uri = null;
		            newTemplate.Id = Guid.NewGuid();
		            ProjectTemplates.SelectedItem = newTemplate;
	            }
	            ProjectTemplates.DisplayMember = string.Empty;
                ProjectTemplates.DisplayMember = "Name";
            }
        }
    }
}
