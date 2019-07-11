using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.RecordSourceTU
{
	public partial class SourceTmConfiguration : Form
	{
		private const string SelectCustomField = "Or create one";

		private readonly Uri _providerUri;
		private AddSourceTmConfigurations _addSourceTmConfigurations;
		private bool _isUsed;

		public SourceTmConfiguration(Uri providerUri)
		{
			InitializeComponent();

			_providerUri = providerUri.GetInnerProviderUri();
			_isUsed = false;
		}

		protected override void OnLoad(EventArgs e)
		{
			var persistance = new Persistance();
			_addSourceTmConfigurations = persistance.Load();

			var addSourceTmConfiguration =
				_addSourceTmConfigurations.Configurations.FirstOrDefault(x => x.ProviderUri == _providerUri);
			if (addSourceTmConfiguration == null)
			{
				addSourceTmConfiguration = _addSourceTmConfigurations.Default;
				addSourceTmConfiguration.ProviderUri = _providerUri;
				_addSourceTmConfigurations.Configurations.Add(addSourceTmConfiguration);
			}
			else
			{
				_isUsed = true;
			}

			var dataAccess = TmDataAccess.OpenConnection(addSourceTmConfiguration.ProviderUri);
			var existingCustomFields = dataAccess.GetCustomFields();

			var fileNameFields = new List<string>(existingCustomFields);
			fileNameFields.Insert(0, SelectCustomField);
			cmbFilenameFields.Items.AddRange(fileNameFields.ToArray());

			var fullPathFields = new List<string>(existingCustomFields);
			fullPathFields.Insert(0, SelectCustomField);
			cmbCompletePathField.Items.AddRange(fullPathFields.ToArray());

			var projectNameFields = new List<string>(existingCustomFields);
			projectNameFields.Insert(0, SelectCustomField);
			cmbProjectNameField.Items.AddRange(projectNameFields.ToArray());

			cmbFilenameFields.SelectedIndexChanged += Boxes_SelectedIndexChanged;
			cmbCompletePathField.SelectedIndexChanged += Boxes_SelectedIndexChanged;
			cmbProjectNameField.SelectedIndexChanged += Boxes_SelectedIndexChanged;

			SetConfigurationData(txtFilenameField, cmbFilenameFields, addSourceTmConfiguration.FileNameField);
			SetConfigurationData(txtCompletePathField, cmbCompletePathField, addSourceTmConfiguration.FullPathField);
			SetConfigurationData(txtProjectNameField, cmbProjectNameField, addSourceTmConfiguration.ProjectNameField);

			chkFullPath.Checked = addSourceTmConfiguration.StoreFullPath;
			chkFileName.Checked = addSourceTmConfiguration.StoreFilename;
			chkProjectName.Checked = addSourceTmConfiguration.StoreProjectName;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			var persistance = new Persistance();

			var addSourceTmConfiguration = _addSourceTmConfigurations.Configurations.FirstOrDefault(x => x.ProviderUri == _providerUri) ??
										   _addSourceTmConfigurations.Default;

			var fileNameField = GetFieldNameValue(cmbFilenameFields, txtFilenameField);
			var fullPathField = GetFieldNameValue(cmbCompletePathField, txtCompletePathField);
			var projectNameField = GetFieldNameValue(cmbProjectNameField, txtProjectNameField);
			if (chkFileName.Checked &&
				fileNameField.Equals(SelectCustomField, StringComparison.CurrentCultureIgnoreCase))
			{
				MessageBox.Show(
					@"Please specify a name for file name custom field",
					@"Specify field", MessageBoxButtons.OK);

				e.Cancel = true;
				return;
			}

			if (chkFileName.Checked && string.IsNullOrEmpty(fileNameField))
			{
				MessageBox.Show(
					@"Please specify a name for file name custom field",
					@"Specify field", MessageBoxButtons.OK);
				e.Cancel = true;
				return;
			}

			if (chkFullPath.Checked && fullPathField.Equals(SelectCustomField, StringComparison.CurrentCultureIgnoreCase))
			{
				MessageBox.Show(
				   @"Please specify a name for full path custom field",
				   @"Specify field", MessageBoxButtons.OK);
				e.Cancel = true;
				return;
			}

			if (chkFullPath.Checked && string.IsNullOrEmpty(fullPathField))
			{
				MessageBox.Show(
				  @"Please specify a name for full path custom field",
				  @"Specify field", MessageBoxButtons.OK);
				e.Cancel = true;
				return;
			}

			if (chkProjectName.Checked && string.IsNullOrEmpty(projectNameField))
			{
				MessageBox.Show(
				   @"Please specify a name for project name custom field",
				   @"Specify field", MessageBoxButtons.OK);
				e.Cancel = true;
				return;
			}
			if (chkProjectName.Checked && projectNameField.Equals(SelectCustomField, StringComparison.CurrentCultureIgnoreCase))
			{
				MessageBox.Show(
				   @"Please specify a name for project name custom field",
				   @"Specify field", MessageBoxButtons.OK);
				e.Cancel = true;
				return;
			}

			addSourceTmConfiguration.FileNameField = fileNameField;
			addSourceTmConfiguration.StoreFilename = chkFileName.Checked;
			addSourceTmConfiguration.FullPathField = fullPathField;
			addSourceTmConfiguration.StoreFullPath = chkFullPath.Checked;
			addSourceTmConfiguration.ProjectNameField = projectNameField;
			addSourceTmConfiguration.StoreProjectName = chkProjectName.Checked;

			if (_isUsed &&
				(addSourceTmConfiguration.HasChanges()))
			{
				var result = MessageBox.Show(
					@"You are about to change the source file configuration for this TM. This will result in creating an additional source file field. Are you sure you want to continue?",
					@"Confirm changes", MessageBoxButtons.OKCancel);

				if (result == DialogResult.Cancel)
				{
					return;
				}
			}
			persistance.Save(_addSourceTmConfigurations);
		}

		private void SetConfigurationData(TextBox textBox, ComboBox cmbFields, string fieldName)
		{
			textBox.Text = fieldName;
			cmbFields.SelectedIndex = 0;
			foreach (var item in cmbFields.Items)
			{
				if (item.ToString().Equals(fieldName))
				{
					cmbFields.SelectedItem = item;
					textBox.Text = string.Empty;
				}
			}
		}

		private string GetFieldNameValue(ComboBox comboBox, TextBox textBox)
		{
			if (comboBox.SelectedIndex != 0)
			{
				return comboBox.SelectedItem.ToString();
			}
			return textBox.Text;
		}

		private void ToggleCorrespondingTextField(ComboBox comboBox)
		{
			var onOff = comboBox.SelectedIndex == 0;

			switch (comboBox.Name)
			{
				case "cmbFilenameFields":
					ToggleTextField(txtFilenameField, onOff);
					break;

				case "cmbCompletePathField":
					ToggleTextField(txtCompletePathField, onOff);
					break;

				case "cmbProjectNameField":
					ToggleTextField(txtProjectNameField, onOff);
					break;
			}
		}

		private void ToggleTextField(TextBox textBox, bool onOff)
		{
			if (textBox.Enabled == onOff) return;
			textBox.Enabled = onOff;

			if (!onOff)
			{
				textBox.Text = "";
			}
		}

		private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				ToggleCorrespondingTextField(comboBox);
			}
		}
	}
}