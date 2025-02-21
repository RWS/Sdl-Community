﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui
{
	public partial class AnonymizerSettingsControl : UserControl, ISettingsAware<AnonymizerSettings>, IUISettingsControl
	{
		private BindingList<RegexPattern> _regexPatterns;

		public AnonymizerSettingsControl()
		{
			InitializeComponent();
		}

		public string EncryptionKey
		{
			get => encryptionBox.Text;
			set => encryptionBox.Text = value;
		}

		public bool SelectAll
		{
			get => selectAll.Checked;
			set => selectAll.Checked = value;
		}

		public BindingList<RegexPattern> RegexPatterns
		{
			get
			{
				expressionsGrid.EndEdit();
				foreach (var pattern in _regexPatterns)
				{
					if (string.IsNullOrEmpty(pattern.Id))
					{
						pattern.Id = Guid.NewGuid().ToString();
					}
				}

				return _regexPatterns;
			}
			set
			{
				_regexPatterns = value;
			}
		}

		public AnonymizerSettings Settings { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SuspendLayout();
			expressionsGrid.SuspendLayout();
			//create tooltips for buttons
			var exportTooltip = new ToolTip();
			exportTooltip.SetToolTip(exportBtn, StringResources.Export_selected_expressions_to_disk);
			var importTooltip = new ToolTip();
			importTooltip.SetToolTip(importBtn, StringResources.Import_regular_expressions_in_to_current_project);

			//labels text
			descriptionLbl.Text = Constants.GetGridDescription();
			encryptionLbl.Text = Constants.GetKeyDescription();

			expressionsGrid.AutoGenerateColumns = false;
			var exportHeaderCell = new CustomColumnHeader
			{
				IsChecked = Settings.EnableAll
			};
			exportHeaderCell.OnCheckBoxHeaderClick += ExportHeaderCell_OnCheckBoxHeaderClick;
			var exportColumn = new DataGridViewCheckBoxColumn
			{
				Width = 100,
				HeaderText = @"Enable?",
				DataPropertyName = "ShouldEnable",
				Name = "Enable",
				HeaderCell = exportHeaderCell,
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
			};

			var encryptHeaderCell = new CustomColumnHeader
			{
				IsChecked = Settings.EncryptAll
			};
			encryptHeaderCell.OnCheckBoxHeaderClick += EncryptHeaderCell_OnCheckBoxHeaderClick;
			var shouldEncryptColumn = new DataGridViewCheckBoxColumn
			{
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
				HeaderText = @"Encrypt?",
				Width = 105,
				DataPropertyName = "ShouldEncrypt",
				HeaderCell = encryptHeaderCell,
				Name = "Encrypt"
			};
			expressionsGrid.Columns.Add(exportColumn);

			var pattern = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Regex Pattern",
				DataPropertyName = "Pattern",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
			};
			expressionsGrid.Columns.Add(pattern);
			var description = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Description",
				DataPropertyName = "Description",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
			};
			expressionsGrid.Columns.Add(description);
			expressionsGrid.Columns.Add(shouldEncryptColumn);

			var moveUpButtonColumn = new DataGridViewImageColumn()
			{
				AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
				Icon = new Icon(PluginResources.Up, 16, 16),
				ValuesAreIcons = true,
				DisplayIndex = 4,
			};

			expressionsGrid.Columns.Add(moveUpButtonColumn);
			expressionsGrid.CellClick += UpDownDeleteButtonClick;

			expressionsGrid.SuspendLayout();
			var moveDownButtonColumn = new DataGridViewImageColumn()
			{
				AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
				Icon = new Icon(PluginResources.Down, 16, 16),
				ValuesAreIcons = true,
				DisplayIndex = 5,
			};

			expressionsGrid.Columns.Add(moveDownButtonColumn);
			expressionsGrid.ResumeLayout();
			var deleteButtonColumn = new DataGridViewImageColumn()
			{
				AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet,
				Icon = new Icon(PluginResources.Delete, 16, 16),
				ValuesAreIcons = true,
				DisplayIndex = 6,
			};

			expressionsGrid.Columns.Add(deleteButtonColumn);

			var defaultRow = expressionsGrid.RowTemplate;
			defaultRow.DefaultCellStyle = null;

			expressionsGrid.AllowUserToAddRows = false;

			expressionsGrid.ResumeLayout();
			ResumeLayout();

			if (Settings.EncryptionState == State.DefaultState)
			{
				Settings.EncryptionState = IsProjectAnonymized() ? State.DataEncrypted : State.Decrypted;
			}

			if ((Settings.EncryptionState & State.DataEncrypted) != 0)
			{
				mainPanel.Visible = false;
				encryptedPanel.Visible = true;
			}

			ReadExistingExpressions();
			SetSettings(Settings);
		}

		private void UpDownDeleteButtonClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 4)
			{
				MovePatternUp(e.RowIndex);
			}

			if (e.ColumnIndex == 5)
			{
				MovePatternDown(e.RowIndex);
			}

			if (e.ColumnIndex == 6)
			{
				DeletePattern(e.RowIndex);
			}
		}

		private void DeletePattern(int index)
		{
			if (index == RegexPatterns.Count - 1)
			{
				RegexPatterns.RemoveAt(index);
			}
			else
			{
				for (int i = index; i < RegexPatterns.Count - 1; i++)
				{
					RegexPatterns[i] = RegexPatterns[i + 1];
				}

				RegexPatterns.RemoveAt(RegexPatterns.Count - 1);
			}
		}

		private void MovePatternUp(int index)
		{
			if (index > 0)
			{
				var previousPattern = RegexPatterns[index - 1];
				RegexPatterns[index - 1] = RegexPatterns[index];
				RegexPatterns[index] = previousPattern;
			}
		}

		private void MovePatternDown(int index)
		{
			if (index < RegexPatterns.Count - 1)
			{
				var previousPattern = RegexPatterns[index + 1];
				RegexPatterns[index + 1] = RegexPatterns[index];
				RegexPatterns[index] = previousPattern;
			}
		}

		private void EncryptHeaderCell_OnCheckBoxHeaderClick(CheckBoxHeaderCellEventArgs e)
		{
			RemoveEmptyRows();
			foreach (var pattern in RegexPatterns)
			{
				pattern.ShouldEncrypt = e.IsChecked;
			}
			Settings.EncryptAll = e.IsChecked;
			Settings.RegexPatterns = RegexPatterns;
			expressionsGrid.Refresh();
		}

		private void ExportHeaderCell_OnCheckBoxHeaderClick(CheckBoxHeaderCellEventArgs e)
		{
			RemoveEmptyRows();
			foreach (var pattern in RegexPatterns)
			{
				pattern.ShouldEnable = e.IsChecked;
			}
			Settings.EnableAll = e.IsChecked;
			Settings.RegexPatterns = RegexPatterns;
			expressionsGrid.Refresh();
		}

		private void ReadExistingExpressions()
		{
			if (Settings.DefaultListAlreadyAdded || Settings.RegexPatterns.Count > 0)
			{
				return;
			}

			Settings.RegexPatterns.Clear();

			var patterns = Constants.GetDefaultRegexPatterns();
			foreach (var pattern in patterns)
			{
				Settings.AddPattern(pattern);
			}
			Settings.DefaultListAlreadyAdded = true;
		}

		private void SetSettings(AnonymizerSettings settings)
		{
			Settings = settings;
			RegexPatterns = Settings.RegexPatterns;
			var key = Settings.GetSetting<string>(nameof(Settings.EncryptionKey)).Value;
			key = key == "<dummy-encryption-key>" ? "" : key;
			if (!string.IsNullOrEmpty(key))
			{
				encryptionBox.Text = AnonymizeData.DecryptData(key, Constants.Key);
			}

			expressionsGrid.DataSource = RegexPatterns;
		}

		private void selectAll_CheckedChanged(object sender, EventArgs e)
		{
			RemoveEmptyRows();
			var shouldSelect = ((CheckBox)sender).Checked;
			foreach (var pattern in RegexPatterns)
			{
				pattern.ShouldEnable = shouldSelect;
				pattern.ShouldEncrypt = shouldSelect;
			}
			Settings.RegexPatterns = RegexPatterns;
			Settings.EnableAll = shouldSelect;
			Settings.EncryptAll = shouldSelect;
			((CustomColumnHeader)expressionsGrid.Columns[0].HeaderCell).IsChecked = shouldSelect;
			((CustomColumnHeader)expressionsGrid.Columns[3].HeaderCell).IsChecked = shouldSelect;
			expressionsGrid.Refresh();
		}

		private void exportBtn_Click(object sender, EventArgs e)
		{
			RemoveEmptyRows();
			if (expressionsGrid.SelectedRows.Count > 0)
			{
				var fileDialog = new SaveFileDialog
				{
					Title = StringResources.Export_selected_expressions,
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					var selectedExpressions = new List<RegexPattern>();
					foreach (DataGridViewRow row in expressionsGrid.SelectedRows)
					{
						var regexPattern = row.DataBoundItem as RegexPattern;
						selectedExpressions.Add(regexPattern);
					}
					Expressions.ExportExpressions(fileDialog.FileName, selectedExpressions);
					MessageBox.Show(StringResources.File_was_exported_successfully_to_selected_location, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(StringResources.Please_select_at_least_one_row_to_export, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void RemoveEmptyRows()
		{
			var regexPatternsCount = RegexPatterns.Count;

			for (int i = 0; i < regexPatternsCount; i++)
			{
				if (string.IsNullOrWhiteSpace(RegexPatterns[i].Pattern))
				{
					RegexPatterns.RemoveAt(i);
					i--;
					regexPatternsCount--;
				}
			}

		}

		private void importBtn_Click(object sender, EventArgs e)
		{
			RemoveEmptyRows();
			var fileDialog = new OpenFileDialog
			{
				Title = StringResources.Please_select_the_files_you_want_to_import,
				Filter = @"Excel |*.xlsx",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "xlsx",
				Multiselect = true
			};
			var result = fileDialog.ShowDialog();
			if (result == DialogResult.OK && fileDialog.FileNames.Length > 0)
			{
				var importedExpressions = Expressions.GetImportedExpressions(fileDialog.FileNames.ToList());
				ImportExpressionsInSettings(importedExpressions);
			}
		}

		private void ImportExpressionsInSettings(IEnumerable<RegexPattern> expressions)
		{
			if (expressions is null) return;
			foreach (var expression in expressions)
			{
				var existScript = RegexPatterns.FirstOrDefault(s => s.Pattern.Equals(expression.Pattern));
				//add script to list
				if (existScript == null)
				{
					expression.ShouldEncrypt = true;
					expression.ShouldEnable = true;
					RegexPatterns.Add(expression);
				}
			}
			Settings.RegexPatterns = RegexPatterns;
		}

		private void expressionsGrid_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode.Equals(Keys.Delete))
			{
				var result = MessageBox.Show(StringResources.Are_you_sure_you_want_to_delete_the_expressions, StringResources.Please_confirm,
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

				if (result == DialogResult.OK)
				{
					RegexPatterns.AllowRemove = true;
					foreach (DataGridViewRow row in expressionsGrid.SelectedRows)
					{
						var regexPattern = row.DataBoundItem as RegexPattern;
						if (regexPattern != null)
						{
							var itemToRemove = RegexPatterns.FirstOrDefault(p => p.Id.Equals(regexPattern.Id));
							RegexPatterns.Remove(itemToRemove);
						}
					}
				}
				Settings.RegexPatterns = RegexPatterns;
			}
		}

		private bool IsProjectAnonymized()
		{
			return Settings.EncryptionKey != "<dummy-encryption-key>";
		}

		private void addNewPatternButton_Click(object sender, EventArgs e)
		{
			RemoveEmptyRows();
			RegexPatterns.AddNew();
			expressionsGrid.Focus();
		}
	}
}