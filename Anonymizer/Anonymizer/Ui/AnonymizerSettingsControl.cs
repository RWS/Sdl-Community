using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.Anonymizer.Batch_Task;
using Sdl.Community.Anonymizer.Helpers;
using Sdl.Community.Anonymizer.Interfaces;
using Sdl.Community.Anonymizer.Models;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Ui
{
	public partial class AnonymizerSettingsControl : UserControl, ISettingsAware<AnonymizerSettings>
	{
		public AnonymizerSettingsControl()
		{
			InitializeComponent();

			expressionsGrid.AutoGenerateColumns = false;
			descriptionLbl.Text = Constants.GetGridDescription();

			var exportColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Enable?",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				DataPropertyName = "ShouldEnable",
				Name = "Enable"
			};
			var shouldEncryptColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Encrypt?",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				DataPropertyName = "ShouldEncrypt",
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
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
			};
			expressionsGrid.Columns.Add(description);
			expressionsGrid.Columns.Add(shouldEncryptColumn);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			ReadExistingExpressions();
			SetSettings(Settings);
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

		public BindingList<RegexPattern> RegexPatterns { get; set; }
		public AnonymizerSettings Settings { get; set; }

		private void ReadExistingExpressions()
		{
			if (!Settings.DefaultListAlreadyAdded)
			{
				RegexPatterns = Constants.GetDefaultRegexPatterns();
				foreach (var pattern in RegexPatterns)
				{
					Settings.AddPattern(pattern);
				}
				Settings.DefaultListAlreadyAdded = true;
			}
		}
	
		private void SetSettings(AnonymizerSettings settings)
		{
			Settings = settings;
			RegexPatterns= Settings.RegexPatterns;
			SettingsBinder.DataBindSetting<string>(encryptionBox, "Text", Settings, nameof(Settings.EncryptionKey));
			SettingsBinder.DataBindSetting<bool>(selectAll, "Checked", Settings, nameof(Settings.SelectAll));
			SettingsBinder.DataBindSetting<BindingList<RegexPattern>>(expressionsGrid, "DataSource", Settings,
				nameof(Settings.RegexPatterns));
			expressionsGrid.DataSource = RegexPatterns;
		}

		private void UpdateUi(AnonymizerSettings settings)
		{
			Settings = settings;
		}

		private void expressionsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			var selectedPattern = RegexPatterns[e.RowIndex];
			var currentCellValue = expressionsGrid.CurrentCell.Value;

			//Enable column
			if (e.ColumnIndex.Equals(0))
			{
				selectedPattern.ShouldEnable = (bool) currentCellValue;
			}
			//Regex pattern column
			if (e.ColumnIndex.Equals(1))
			{
				if (!string.IsNullOrEmpty(currentCellValue.ToString()))
				{
					selectedPattern.Pattern = currentCellValue.ToString();
				}
			}
			//Description column
			if (e.ColumnIndex.Equals(2))
			{
				if (!string.IsNullOrEmpty(currentCellValue.ToString()))
				{
					selectedPattern.Description = currentCellValue.ToString();
				}
			}
			//Encrypt column
			if (e.ColumnIndex.Equals(3))
			{
				selectedPattern.ShouldEncrypt = (bool)currentCellValue;
			}
			if (!string.IsNullOrEmpty(selectedPattern.Description) && !string.IsNullOrEmpty(selectedPattern.Pattern))
			{
				//that means is a new added expression and the id is empty
				if (string.IsNullOrEmpty(selectedPattern.Id))
				{
					selectedPattern.Id = Guid.NewGuid().ToString();
				}
			}
		}

		private void selectAll_CheckedChanged(object sender, EventArgs e)
		{
			//expressionsGrid.SuspendLayout();
			var shouldSelect = ((CheckBox)sender).Checked;
			foreach (var pattern in RegexPatterns)
			{
				pattern.ShouldEnable = shouldSelect;
				pattern.ShouldEncrypt = shouldSelect;
			}
			Settings.RegexPatterns = RegexPatterns;

			//foreach (DataGridViewRow row in expressionsGrid.Rows)
			//{
			//	var isNewRow = row.IsNewRow;
			//	if (!isNewRow)
			//	{
			//		var item = (RegexPattern)row.DataBoundItem;
			//		item.ShouldEnable = shouldSelect;
			//		item.ShouldEncrypt = shouldSelect;
			//	}
			//}
			//expressionsGrid.ResumeLayout();
			//expressionsGrid.ResetBindings();
		}

		//private void expressionsGrid_KeyDown(object sender, KeyEventArgs e)
		//{
		//	if (e.KeyCode.Equals(Keys.Delete))
		//	{
		//		var result = MessageBox.Show(@"Are you sure you want to delete the expressions?",@"Please confirm",
		//			MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

		//		if (result == DialogResult.OK)
		//		{
		//			for (var i = 0; i < expressionsGrid.SelectedRows.Count; i++)
		//			{
		//				var row = expressionsGrid.SelectedRows[i];
		//				var regexPattern = row.DataBoundItem as RegexPattern;
		//				RegexPatterns.Remove(regexPattern);
		//			}
		//		}
		//	}
		//}
	}
}
