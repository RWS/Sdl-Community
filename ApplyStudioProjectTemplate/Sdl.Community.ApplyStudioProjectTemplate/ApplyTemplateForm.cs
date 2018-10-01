// -----------------------------------------------------------------------
// <copyright file="ApplyTemplateForm.cs" company="SDL plc">
// © 2014 SDL plc
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
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
		private bool _showWarning = true;
		private bool _languageMatches = true;

		private readonly ProjectsController _projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplyTemplateForm"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public ApplyTemplateForm(ProjectsController controller)
		{
			InitializeComponent();
			LoadProjectTemplates(controller);
		}

		/// <summary>
		/// Gets the active template.
		/// </summary>
		/// <value>
		/// The active template.
		/// </value>
		public ApplyTemplate ActiveTemplate => SelectedTemplate.SelectedItem as ApplyTemplate;

		/// <summary>
		/// Gets a value indicating whether to apply to selected projects.
		/// </summary>
		/// <value>
		///   <c>true</c> if apply to selected projects; otherwise, <c>false</c>.
		/// </value>
		public bool ApplyToSelected => ApplyTo.SelectedIndex == 1;

		/// <summary>
		/// Saves the project templates.
		/// </summary>
		public void SaveProjectTemplates()
		{
			var projectTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL\ASPT.xml");
			var settings = new XmlWriterSettings
			{
				Indent = true
			};
			using (var writer = XmlWriter.Create(projectTemplatesPath, settings))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("templates");
				var applyTemplate = SelectedTemplate?.SelectedItem as ApplyTemplate;
				if (applyTemplate != null)
					writer.WriteAttributeString("default",
						applyTemplate.Id.ToString("D"));
				writer.WriteAttributeString("apply", (string)ApplyTo.SelectedItem);
				writer.WriteAttributeString("tooltips", ShowToolTips.Checked ? "1" : "0");
				writer.WriteAttributeString("warning", _showWarning ? "1" : "0");

				if (SelectedTemplate?.Items != null)
				{
					foreach (var comboObject in SelectedTemplate?.Items)
					{
						var comboTemplate = comboObject as ApplyTemplate;
						if (comboTemplate != null && comboTemplate.Id != Guid.Empty)
						{
							comboTemplate.WriteXml(writer);
						}
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
			foreach (var templateInfo in controller.GetProjectTemplates())
			{
				if (File.Exists(templateInfo.Uri.LocalPath))
				{
					var newTemplate = new ApplyTemplate(templateInfo);
					SelectedTemplate.Items.Add(newTemplate);
				}
			}

			// Add in any extra templates manually defined
			var selectedId = Guid.Empty;
			ApplyTo.SelectedIndex = 0;
			var projectTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL\ASPT.xml");
			if (File.Exists(projectTemplatesPath))
			{
				try
				{
					var templatesXml = new XmlDocument();
					templatesXml.Load(projectTemplatesPath);

					if (templatesXml.DocumentElement != null && templatesXml.DocumentElement.HasAttribute("default"))
					{
						selectedId = new Guid(templatesXml.DocumentElement.Attributes["default"].Value);
					}

					if (templatesXml.DocumentElement != null && templatesXml.DocumentElement.HasAttribute("apply"))
					{
						ApplyTo.SelectedItem = templatesXml.DocumentElement.Attributes["apply"].Value;
					}

					if (templatesXml.DocumentElement != null && templatesXml.DocumentElement.HasAttribute("tooltips"))
					{
						ShowToolTips.Checked = templatesXml.DocumentElement.Attributes["tooltips"].Value == "1";
					}

					if (templatesXml.DocumentElement != null && templatesXml.DocumentElement.HasAttribute("warning"))
					{
						_showWarning = templatesXml.DocumentElement.Attributes["warning"].Value == "1";
					}

					var xmlNodeList = templatesXml.SelectNodes("//template");
					if (xmlNodeList != null)
					{
						foreach (XmlNode templateXml in xmlNodeList)
						{
							var newTemplate = new ApplyTemplate(templateXml);
							if (string.IsNullOrEmpty(newTemplate.FileLocation))
							{
								foreach (var o in SelectedTemplate.Items)
								{
									var thisTemplate = o as ApplyTemplate;
									if (thisTemplate == null || thisTemplate.Id != newTemplate.Id) continue;

									thisTemplate.TranslationProvidersAllLanguages = newTemplate.TranslationProvidersAllLanguages;
									thisTemplate.TranslationProvidersSpecificLanguages =
										newTemplate.TranslationProvidersSpecificLanguages;
									thisTemplate.TranslationMemoriesAllLanguages = newTemplate.TranslationMemoriesAllLanguages;
									thisTemplate.TranslationMemoriesSpecificLanguages =
										newTemplate.TranslationMemoriesSpecificLanguages;
									thisTemplate.TerminologyTermbases = newTemplate.TerminologyTermbases;
									thisTemplate.TerminologySearchSettings = newTemplate.TerminologySearchSettings;
									thisTemplate.TranslationQualityAssessment = newTemplate.TranslationQualityAssessment;
									thisTemplate.VerificationQaChecker30 = newTemplate.VerificationQaChecker30;
									thisTemplate.VerificationTagVerifier = newTemplate.VerificationTagVerifier;
									thisTemplate.VerificationTerminologyVerifier = newTemplate.VerificationTerminologyVerifier;
									thisTemplate.VerificationNumberVerifier = newTemplate.VerificationNumberVerifier;
									thisTemplate.VerificationGrammarChecker = newTemplate.VerificationGrammarChecker;
									thisTemplate.BatchTasksAllLanguages = newTemplate.BatchTasksAllLanguages;
									thisTemplate.BatchTasksSpecificLanguages = newTemplate.BatchTasksSpecificLanguages;
									thisTemplate.FileTypes = newTemplate.FileTypes;
									thisTemplate.MatchRepairSettings = newTemplate.MatchRepairSettings;
									thisTemplate.VerificationSpecificLanguages = newTemplate.VerificationSpecificLanguages;
								}
							}
							else
							{
								if (File.Exists(newTemplate.FileLocation))
								{
									SelectedTemplate.Items.Add(newTemplate);
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}
			}

			// Add in a default template if we don't have any defined
			if (SelectedTemplate.Items.Count == 0)
			{
				SelectedTemplate.Items.Add(new ApplyTemplate("<none>"));
			}
			// Select the first one in the list if we haven't selected one yet
			SelectTemplate(selectedId);
		}

		/// <summary>
		/// Selects the template.
		/// </summary>
		/// <param name="selectedId">The selected identifier.</param>
		private void SelectTemplate(Guid selectedId)
		{
			SelectedTemplate.SelectedIndex = 0;
			for (var index = 0; index < SelectedTemplate.Items.Count; index++)
			{
				var applyTemplate = SelectedTemplate.Items[index] as ApplyTemplate;
				if (applyTemplate == null || applyTemplate.Id != selectedId) continue;
				SelectedTemplate.SelectedIndex = index;
				break;
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the SelectedTemplate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void SelectedTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedTemplate = SelectedTemplate.SelectedItem as ApplyTemplate;
			
			if (selectedTemplate != null)
			{
				TranslationProvidersAllLanguages.SelectedItem = selectedTemplate.TranslationProvidersAllLanguages.ToString();
				TranslationProvidersSpecificLanguages.SelectedItem = selectedTemplate.TranslationProvidersSpecificLanguages.ToString();
				TranslationMemoriesAllLanguages.SelectedItem = selectedTemplate.TranslationMemoriesAllLanguages.ToString();
				TranslationMemoriesSpecificLanguages.SelectedItem = selectedTemplate.TranslationMemoriesSpecificLanguages.ToString();
				TerminologyTermbases.SelectedItem = selectedTemplate.TerminologyTermbases.ToString();
				TerminologySearchSettings.SelectedItem = selectedTemplate.TerminologySearchSettings.ToString();
				TranslationQualityAssessment.SelectedItem = selectedTemplate.TranslationQualityAssessment.ToString();
				BatchTasksAllLanguages.SelectedItem = selectedTemplate.BatchTasksAllLanguages.ToString();
				BatchTasksSpecificLanguages.SelectedItem = selectedTemplate.BatchTasksSpecificLanguages.ToString();
				VerificationQaChecker30.SelectedItem = selectedTemplate.VerificationQaChecker30.ToString();
				VerificationTagVerifier.SelectedItem = selectedTemplate.VerificationTagVerifier.ToString();
				VerificationTerminologyVerifier.SelectedItem = selectedTemplate.VerificationTerminologyVerifier.ToString();
				VerificationNumberVerifier.SelectedItem = selectedTemplate.VerificationNumberVerifier.ToString();
				VerificationGrammarChecker.SelectedItem = selectedTemplate.VerificationGrammarChecker.ToString();
				FileTypes.SelectedItem = selectedTemplate.FileTypes.ToString();
				matchRepairBox.SelectedItem = selectedTemplate.MatchRepairSettings.ToString();
				VerificationSpecificLanguages.SelectedItem = selectedTemplate.VerificationSpecificLanguages.ToString();
			}
			CheckChanged();
		}

		/// <summary>
		/// Checks whether to enable the OK button.
		/// </summary>
		private void CheckChanged()
		{
			var applyTemplate = SelectedTemplate.SelectedItem as ApplyTemplate;
			if (applyTemplate != null && applyTemplate.Id == Guid.Empty)
			{
				OkButton.Enabled = false;
			}
			else
			{
				var sumOfSelected = TranslationProvidersAllLanguages.SelectedIndex +
									TranslationProvidersSpecificLanguages.SelectedIndex +
									TranslationMemoriesAllLanguages.SelectedIndex +
									TranslationMemoriesSpecificLanguages.SelectedIndex +
									TerminologyTermbases.SelectedIndex +
									TerminologySearchSettings.SelectedIndex +
									TranslationQualityAssessment.SelectedIndex +
									BatchTasksAllLanguages.SelectedIndex +
									BatchTasksSpecificLanguages.SelectedIndex +
									VerificationQaChecker30.SelectedIndex +
									VerificationTagVerifier.SelectedIndex +
									VerificationTerminologyVerifier.SelectedIndex +
									VerificationNumberVerifier.SelectedIndex +
									matchRepairBox.SelectedIndex +
									FileTypes.SelectedIndex +
									VerificationSpecificLanguages.SelectedIndex;
				OkButton.Enabled = sumOfSelected > 0;
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TranslationProvidersAllLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TranslationProvidersAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TranslationProvidersAllLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TranslationProvidersAllLanguages.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TranslationProvidersSpecificLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TranslationProvidersSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TranslationProvidersSpecificLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TranslationProvidersSpecificLanguages.SelectedItem.ToString());
			}

			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TranslationMemoriesAllLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TranslationMemoriesAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TranslationMemoriesAllLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TranslationMemoriesAllLanguages.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TranslationMemoriesSpecificLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TranslationMemoriesSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TranslationMemoriesSpecificLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TranslationMemoriesSpecificLanguages.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TerminologyTermbases control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TerminologyTermbases_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TerminologyTermbases =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TerminologyTermbases.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TerminologySearchSettings control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TerminologySearchSettings_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TerminologySearchSettings =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TerminologySearchSettings.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the BatchTasksAllLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void BatchTasksAllLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.BatchTasksAllLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						BatchTasksAllLanguages.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the BatchTasksSpecificLanguages control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void BatchTasksSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.BatchTasksSpecificLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						BatchTasksSpecificLanguages.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the Verification Quality Assurance Checker 3.0 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void VerificationQaChecker30_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.VerificationQaChecker30 =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						VerificationQaChecker30.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the VerificationTagVerifier control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void VerificationTagVerifier_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.VerificationTagVerifier =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						VerificationTagVerifier.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the VerificationTerminologyVerifier control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void VerificationTerminologyVerifier_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.VerificationTerminologyVerifier =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						VerificationTerminologyVerifier.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the VerificationNumberVerifier control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void VerificationNumberVerifier_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.VerificationNumberVerifier =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						VerificationNumberVerifier.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the VerificationGrammarChecker control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void VerificationGrammarChecker_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.VerificationGrammarChecker =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						VerificationGrammarChecker.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the FileTypes control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void FileTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.FileTypes =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), FileTypes.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the TranslationQualityAssessment control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void TranslationQualityAssessment_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
			{
				applyTemplate.TranslationQualityAssessment =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions),
						TranslationQualityAssessment.SelectedItem.ToString());
			}
			CheckChanged();
		}

		/// <summary>
		/// Handles the Click event of the EditTemplatesButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void EditTemplatesButton_Click(object sender, EventArgs e)
		{
			// Prepare the edit templates dialog
			var editTemplates = new EditApplyTemplates();
			ApplyTemplate comboTemplate;
			foreach (var comboObject in SelectedTemplate.Items)
			{
				comboTemplate = comboObject as ApplyTemplate;
				if (!string.IsNullOrEmpty(comboTemplate?.FileLocation))
				{
					editTemplates.ProjectTemplatesItems.Add(comboTemplate);
				}
			}
			// Show the dialog and check the result
			if (editTemplates.ShowDialog() == DialogResult.OK)
			{
				// Remember which item is selected
				if (SelectedTemplate.SelectedItem is ApplyTemplate applyTemplate)
				{
					var selectedId = applyTemplate.Id;

					// Remove any templates which are not part of the Studio default list
					var itemCount = 0;
					while (itemCount < SelectedTemplate.Items.Count)
					{
						comboTemplate = SelectedTemplate.Items[itemCount] as ApplyTemplate;
						if (comboTemplate != null && comboTemplate.Uri == null)
						{
							SelectedTemplate.Items.RemoveAt(itemCount);
						}
						else
						{
							itemCount++;
						}
					}
					// Add in each template from the dialog
					foreach (var o in editTemplates.ProjectTemplatesItems)
					{
						SelectedTemplate.Items.Add(o);
					}
					// Add a default template if necessary
					if (SelectedTemplate.Items.Count == 0)
					{
						SelectedTemplate.Items.Add(new ApplyTemplate("<none>"));
					}
					// Select the previously selected template
					SelectTemplate(selectedId);
				}
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
				FormToolTip.ToolTipTitle = (string)(sender as Control).Tag;
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the ShowToolTips control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void ShowToolTips_CheckedChanged(object sender, EventArgs e)
		{
			FormToolTip.Active = ShowToolTips.Checked;
		}

		/// <summary>
		/// Handles the Click event of the OkButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void OkButton_Click(object sender, EventArgs e)
		{
			if (_showWarning)
			{
				if (TerminologyTermbases.SelectedIndex > 0 || TerminologySearchSettings.SelectedIndex > 0)
				{
					var warningForm = new TermbaseWarningForm();
					if (warningForm.ShowDialog(this) == DialogResult.Cancel)
					{
						TerminologyTermbases.SelectedIndex = 0;
						TerminologySearchSettings.SelectedIndex = 0;
					}
					_showWarning = warningForm.ShowAgain;
				}
			}

			if (SelectedTemplate.SelectedIndex > 0)
			{
				_languageMatches = Helpers.Matches(_projectController.SelectedProjects.ToList(), ActiveTemplate);
			}

			if (!_languageMatches)
			{
				var result = MessageBox.Show(@"Selected template has different language directions, or language pairs, from the selected project.  Do you still wish to apply the settings from this template?", @"Warning",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				DialogResult = result == DialogResult.Yes ? DialogResult.OK : DialogResult.None;
			}
			else
			{
				DialogResult = DialogResult.OK;
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

		private void matchRepairBox_MouseEnter(object sender, EventArgs e)
		{
			var control = sender as Control;
			if (control != null)
			{
				FormToolTip.ToolTipTitle = (string)control.Tag;
			}
		}

		private void matchRepairBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var applyTemplate = SelectedTemplate.SelectedItem as ApplyTemplate;
			if (applyTemplate != null)
			{
				applyTemplate.MatchRepairSettings =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), matchRepairBox.SelectedItem.ToString());
				CheckChanged();
			}
		}

		private void VerificationSpecificLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			var applyTemplate = SelectedTemplate.SelectedItem as ApplyTemplate;
			if (applyTemplate != null)
			{
				applyTemplate.VerificationSpecificLanguages =
					(ApplyTemplateOptions)Enum.Parse(typeof(ApplyTemplateOptions), VerificationSpecificLanguages.SelectedItem.ToString());
				CheckChanged();
			}
		}
	}
}
