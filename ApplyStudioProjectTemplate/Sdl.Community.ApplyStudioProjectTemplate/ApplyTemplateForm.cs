// -----------------------------------------------------------------------
// <copyright file="ApplyTemplateForm.cs" company="SDL plc">
// © 2014 SDL plc
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The main form for applying a template
    /// </summary>
    public partial class ApplyTemplateForm : Form
    {
        /// <summary>
        /// Determines whether to show the warning about terminology
        /// </summary>
        private bool showWarning = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplateForm"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ApplyTemplateForm(ProjectsController controller)
        {
            this.InitializeComponent();
            this.LoadProjectTemplates(controller);
        }

        /// <summary>
        /// Gets the active template.
        /// </summary>
        /// <value>
        /// The active template.
        /// </value>
        public ApplyTemplate ActiveTemplate
        {
            get
            {
                return this.SelectedTemplate.SelectedItem as ApplyTemplate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to apply to selected projects.
        /// </summary>
        /// <value>
        ///   <c>true</c> if apply to selected projects; otherwise, <c>false</c>.
        /// </value>
        public bool ApplyToSelected
        {
            get
            {
                return this.ApplyTo.SelectedIndex == 1;
            }
        }

        /// <summary>
        /// Saves the project templates.
        /// </summary>
        public void SaveProjectTemplates()
        {
            string projectTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL\ASPT.xml");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(projectTemplatesPath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("templates");
                writer.WriteAttributeString("default", (this.SelectedTemplate.SelectedItem as ApplyTemplate).Id.ToString("D"));
                writer.WriteAttributeString("apply", (string)this.ApplyTo.SelectedItem);
                writer.WriteAttributeString("tooltips", this.ShowToolTips.Checked ? "1" : "0");
                writer.WriteAttributeString("warning", this.showWarning ? "1" : "0");
                foreach (object comboObject in this.SelectedTemplate.Items)
                {
                    ApplyTemplate comboTemplate = comboObject as ApplyTemplate;
                    if (comboTemplate.Id != Guid.Empty)
                    {
                        comboTemplate.WriteXml(writer);
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Loads the project templates.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private void LoadProjectTemplates(ProjectsController controller)
        {
            // Add in the project templates defined in Studio
            foreach (ProjectTemplateInfo templateInfo in controller.GetProjectTemplates())
            {
                ApplyTemplate newTemplate = new ApplyTemplate(templateInfo);
                this.SelectedTemplate.Items.Add(newTemplate);
            }

            // Add in any extra templates manually defined
            Guid selectedId = Guid.Empty;
            this.ApplyTo.SelectedIndex = 0;
            string projectTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL\ASPT.xml");
            if (File.Exists(projectTemplatesPath))
            {
                try
                {
                    XmlDocument templatesXml = new XmlDocument();
                    templatesXml.Load(projectTemplatesPath);

                    if (templatesXml.DocumentElement.HasAttribute("default"))
                    {
                        selectedId = new Guid(templatesXml.DocumentElement.Attributes["default"].Value);
                    }

                    if (templatesXml.DocumentElement.HasAttribute("apply"))
                    {
                        this.ApplyTo.SelectedItem = templatesXml.DocumentElement.Attributes["apply"].Value;
                    }

                    if (templatesXml.DocumentElement.HasAttribute("tooltips"))
                    {
                        this.ShowToolTips.Checked = templatesXml.DocumentElement.Attributes["tooltips"].Value == "1";
                    }

                    if (templatesXml.DocumentElement.HasAttribute("warning"))
                    {
                        this.showWarning = templatesXml.DocumentElement.Attributes["warning"].Value == "1";
                    }

                    foreach (XmlNode templateXml in templatesXml.SelectNodes("//template"))
                    {
                        ApplyTemplate newTemplate = new ApplyTemplate(templateXml);
                        if (string.IsNullOrEmpty(newTemplate.FileLocation))
                        {
                            foreach (object o in this.SelectedTemplate.Items)
                            {
                                ApplyTemplate thisTemplate = o as ApplyTemplate;
                                if (thisTemplate.Id == newTemplate.Id)
                                {
                                    thisTemplate.TranslationProvidersAllLanguages = newTemplate.TranslationProvidersAllLanguages;
                                    thisTemplate.TranslationProvidersSpecificLanguages = newTemplate.TranslationProvidersSpecificLanguages;
                                    thisTemplate.TranslationMemoriesAllLanguages = newTemplate.TranslationMemoriesAllLanguages;
                                    thisTemplate.TranslationMemoriesSpecificLanguages = newTemplate.TranslationMemoriesSpecificLanguages;
                                    thisTemplate.TerminologyTermbases = newTemplate.TerminologyTermbases;
                                    thisTemplate.TerminologySearchSettings = newTemplate.TerminologySearchSettings;
                                    thisTemplate.VerificationQaChecker30 = newTemplate.VerificationQaChecker30;
                                    thisTemplate.VerificationTagVerifier = newTemplate.VerificationTagVerifier;
                                    thisTemplate.VerificationTerminologyVerifier = newTemplate.VerificationTerminologyVerifier;
                                    thisTemplate.VerificationNumberVerifier = newTemplate.VerificationNumberVerifier;
                                    thisTemplate.VerificationGrammarChecker = newTemplate.VerificationGrammarChecker;
                                    thisTemplate.BatchTasksAllLanguages = newTemplate.BatchTasksAllLanguages;
                                    thisTemplate.BatchTasksSpecificLanguages = newTemplate.BatchTasksSpecificLanguages;
                                    thisTemplate.FileTypes = newTemplate.FileTypes;
                                }
                            }
                        }
                        else
                        {
                            this.SelectedTemplate.Items.Add(newTemplate);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            // Add in a default template if we don't have any defined
            if (this.SelectedTemplate.Items.Count == 0)
            {
                this.SelectedTemplate.Items.Add(new ApplyTemplate("<none>"));
            }

            // Select the first one in the list if we haven't selected one yet
            this.SelectTemplate(selectedId);
        }

        /// <summary>
        /// Selects the template.
        /// </summary>
        /// <param name="selectedId">The selected identifier.</param>
        private void SelectTemplate(Guid selectedId)
        {
            this.SelectedTemplate.SelectedIndex = 0;
            for (int index = 0; index < this.SelectedTemplate.Items.Count; index++)
            {
                if ((this.SelectedTemplate.Items[index] as ApplyTemplate).Id == selectedId)
                {
                    this.SelectedTemplate.SelectedIndex = index;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the SelectedTemplate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SelectedTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyTemplate selectedTemplate = this.SelectedTemplate.SelectedItem as ApplyTemplate;
            this.TranslationProvidersAllLanguages.SelectedItem = selectedTemplate.TranslationProvidersAllLanguages.ToString();
            this.TranslationProvidersSpecificLanguages.SelectedItem = selectedTemplate.TranslationProvidersSpecificLanguages.ToString();
            this.TranslationMemoriesAllLanguages.SelectedItem = selectedTemplate.TranslationMemoriesAllLanguages.ToString();
            this.TranslationMemoriesSpecificLanguages.SelectedItem = selectedTemplate.TranslationMemoriesSpecificLanguages.ToString();
            this.TerminologyTermbases.SelectedItem = selectedTemplate.TerminologyTermbases.ToString();
            this.TerminologySearchSettings.SelectedItem = selectedTemplate.TerminologySearchSettings.ToString();
            this.BatchTasksAllLanguages.SelectedItem = selectedTemplate.BatchTasksAllLanguages.ToString();
            this.BatchTasksSpecificLanguages.SelectedItem = selectedTemplate.BatchTasksSpecificLanguages.ToString();
            this.VerificationQaChecker30.SelectedItem = selectedTemplate.VerificationQaChecker30.ToString();
            this.VerificationTagVerifier.SelectedItem = selectedTemplate.VerificationTagVerifier.ToString();
            this.VerificationTerminologyVerifier.SelectedItem = selectedTemplate.VerificationTerminologyVerifier.ToString();
            this.VerificationNumberVerifier.SelectedItem = selectedTemplate.VerificationNumberVerifier.ToString();
            this.VerificationGrammarChecker.SelectedItem = selectedTemplate.VerificationGrammarChecker.ToString();
            this.FileTypes.SelectedItem = selectedTemplate.FileTypes.ToString();
            this.CheckChanged();
        }

        /// <summary>
        /// Checks whether to enable the OK button.
        /// </summary>
        private void CheckChanged()
        {
            if ((this.SelectedTemplate.SelectedItem as ApplyTemplate).Id == Guid.Empty)
            {
                this.OkButton.Enabled = false;
            }
            else
            {
                int sumOfSelected = this.TranslationProvidersAllLanguages.SelectedIndex +
                                    this.TranslationProvidersSpecificLanguages.SelectedIndex +
                                    this.TranslationMemoriesAllLanguages.SelectedIndex +
                                    this.TranslationMemoriesSpecificLanguages.SelectedIndex +
                                    this.TerminologyTermbases.SelectedIndex +
                                    this.TerminologySearchSettings.SelectedIndex +
                                    this.BatchTasksAllLanguages.SelectedIndex +
                                    this.BatchTasksSpecificLanguages.SelectedIndex +
                                    this.VerificationQaChecker30.SelectedIndex +
                                    this.VerificationTagVerifier.SelectedIndex +
                                    this.VerificationTerminologyVerifier.SelectedIndex +
                                    this.VerificationNumberVerifier.SelectedIndex +
                                    this.FileTypes.SelectedIndex;
                this.OkButton.Enabled = sumOfSelected > 0;
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TranslationProvidersAllLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TranslationProvidersAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TranslationProvidersAllLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TranslationProvidersAllLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TranslationProvidersSpecificLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TranslationProvidersSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TranslationProvidersSpecificLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TranslationProvidersSpecificLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TranslationMemoriesAllLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TranslationMemoriesAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TranslationMemoriesAllLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TranslationMemoriesAllLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TranslationMemoriesSpecificLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TranslationMemoriesSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TranslationMemoriesSpecificLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TranslationMemoriesSpecificLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TerminologyTermbases control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TerminologyTermbases_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TerminologyTermbases = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TerminologyTermbases.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the TerminologySearchSettings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TerminologySearchSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).TerminologySearchSettings = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.TerminologySearchSettings.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the BatchTasksAllLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BatchTasksAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).BatchTasksAllLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.BatchTasksAllLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the BatchTasksSpecificLanguages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void BatchTasksSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).BatchTasksSpecificLanguages = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.BatchTasksSpecificLanguages.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the Verification Quality Assurance Checker 3.0 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VerificationQaChecker30_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).VerificationQaChecker30 = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.VerificationQaChecker30.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the VerificationTagVerifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VerificationTagVerifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).VerificationTagVerifier = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.VerificationTagVerifier.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the VerificationTerminologyVerifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VerificationTerminologyVerifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).VerificationTerminologyVerifier = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.VerificationTerminologyVerifier.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the VerificationNumberVerifier control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VerificationNumberVerifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).VerificationNumberVerifier = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.VerificationNumberVerifier.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the VerificationGrammarChecker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void VerificationGrammarChecker_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).VerificationGrammarChecker = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.VerificationGrammarChecker.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the FileTypes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FileTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            (this.SelectedTemplate.SelectedItem as ApplyTemplate).FileTypes = (ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), this.FileTypes.SelectedItem.ToString());
            this.CheckChanged();
        }

        /// <summary>
        /// Handles the Click event of the EditTemplatesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EditTemplatesButton_Click(object sender, EventArgs e)
        {
            // Prepare the edit templates dialog
            EditApplyTemplates editTemplates = new EditApplyTemplates();
            ApplyTemplate comboTemplate = null;
            foreach (object comboObject in this.SelectedTemplate.Items)
            {
                comboTemplate = comboObject as ApplyTemplate;
                if (!string.IsNullOrEmpty(comboTemplate.FileLocation))
                {
                    editTemplates.ProjectTemplatesItems.Add(comboTemplate);
                }
            }

            // Show the dialog and check the result
            if (editTemplates.ShowDialog() == DialogResult.OK)
            {
                // Remember which item is selected
                Guid selectedId = (this.SelectedTemplate.SelectedItem as ApplyTemplate).Id;

                // Remove any templates which are not part of the Studio default list
                int itemCount = 0;
                while (itemCount < this.SelectedTemplate.Items.Count)
                {
                    comboTemplate = this.SelectedTemplate.Items[itemCount] as ApplyTemplate;
                    if (comboTemplate.Uri == null)
                    {
                        this.SelectedTemplate.Items.RemoveAt(itemCount);
                    }
                    else
                    {
                        itemCount++;
                    }
                }

                // Add in each template from the dialog
                foreach (object o in editTemplates.ProjectTemplatesItems)
                {
                    this.SelectedTemplate.Items.Add(o);
                }

                // Add a default template if necessary
                if (this.SelectedTemplate.Items.Count == 0)
                {
                    this.SelectedTemplate.Items.Add(new ApplyTemplate("<none>"));
                }

                // Select the previously selected template
                this.SelectTemplate(selectedId);
            }
        }

        /// <summary>
        /// Handles the mouse enter event for multiple controls.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ControlMouseEnter(object sender, EventArgs e)
        {
            if (sender is Control)
            {
                this.FormToolTip.ToolTipTitle = (string)(sender as Control).Tag;
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the ShowToolTips control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ShowToolTips_CheckedChanged(object sender, EventArgs e)
        {
            this.FormToolTip.Active = this.ShowToolTips.Checked;
        }

        /// <summary>
        /// Handles the Click event of the OkButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OkButton_Click(object sender, EventArgs e)
        {
            if (this.showWarning)
            {
                if (this.TerminologyTermbases.SelectedIndex > 0 || this.TerminologySearchSettings.SelectedIndex > 0)
                {
                    TermbaseWarningForm warningForm = new TermbaseWarningForm();
                    if (warningForm.ShowDialog(this) == DialogResult.Cancel)
                    {
                        this.TerminologyTermbases.SelectedIndex = 0;
                        this.TerminologySearchSettings.SelectedIndex = 0;
                    }

                    this.showWarning = warningForm.ShowAgain;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the AboutButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AboutButton_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }
    }
}
