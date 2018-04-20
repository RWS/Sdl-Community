using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.Anonymizer.Interfaces;
using Sdl.Community.Anonymizer.Models;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Ui
{
	public partial class AnonymizerSettingsControl : UserControl, ISettingsAware<AnonymizerSettings>
	{
		private List<RegexPattern> _expressions;

		public AnonymizerSettingsControl()
		{
			InitializeComponent();

			expressionsGrid.AutoGenerateColumns = false;
			var exportColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Enable?"
			};
			var shouldEncryptColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Encrypt?"
				//Width = 60
			};
			expressionsGrid.Columns.Add(exportColumn);
			var pattern = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Regex Pattern",
				DataPropertyName = "Pattern"
			};
			expressionsGrid.Columns.Add(pattern);
			var description = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Description",
				DataPropertyName = "Description"
			};
			expressionsGrid.Columns.Add(description);

			expressionsGrid.Columns.Add(shouldEncryptColumn);

		}


		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ReadExistingExpressions();
			SetSettings(Settings);
		}

		public string EncryptionKey
		{
			get => encryptionBox.Text;
			set => encryptionBox.Text = value;
		}

		public List<RegexPattern> RegexPatterns { get; set; }

		private void ReadExistingExpressions()
		{
			RegexPatterns = new List<RegexPattern>
			{
				new RegexPattern
				{
					Id = "1",
					Description = "email",
					Pattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A - Z]{ 2,}\b"
				},

				new RegexPattern
				{
					Id = "2",
					Description = "PCI",
					Pattern = @"\b(?:\d[ -]*?){13,16}\b"
				},
				new RegexPattern
				{
					Id = "3",
					Description = "IP6 Address",
					Pattern = @"(?<![:.\w])(?:[A-F0-9]{1,4}:){7}[A-F0-9]{1,4}(?![:.\w])"
				},
				new RegexPattern
				{
					Id = "4",
					Description = "Social Security Numbers",
					Pattern = @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b",
				},
				new RegexPattern
				{
					Id = "5",
					Description = "Telephone Numbers",
					Pattern = @"\b\d{4}\s\d+-\d+\b"
				},
				new RegexPattern
				{
					Id = "6",
					Description = "Car Registrations",
					Pattern = @"\b\p{Lu}+\s\p{Lu}+\s\d+\b|\b\p{Lu}+\s\d+\s\p{Lu}+\b|\b\p{Lu}+\d+\s\p{Lu}+\b|\b\p{Lu}+\s\d+\p{Lu}+\b"
				},
				new RegexPattern
				{
					Id = "7",
					Description = "Passport Numbers",
					Pattern = @"\b\d{9}\b"
				},
				new RegexPattern
				{
					Id = "8",
					Description = "National Insurance Number",
					Pattern = @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b"
				},
				new RegexPattern
				{
					Id = "9",
					Pattern = "Date of Birth",
					Description = @"\b\d{2}/\d{2}/\d{4}\b"
				},
				new RegexPattern
				{
					Pattern = "",
					Description = ""
				}
			};
		}

		private void SetSettings(AnonymizerSettings settings)
		{
			Settings = settings;
			foreach (var pattern in RegexPatterns)
			{
				Settings.AddPattern(pattern);
			}

			SettingsBinder.DataBindSetting<string>(encryptionBox, "Text", Settings, nameof(Settings.EncryptionKey));
			SettingsBinder.DataBindSetting<List<RegexPattern>>(expressionsGrid, "DataSource", Settings,
				nameof(Settings.RegexPatterns));
			UpdateUi(settings);
		}
		public void UpdateSettings(AnonymizerSettings settings)
		{
			Settings = settings;
		}


		private void UpdateUi(AnonymizerSettings settings)
		{
			Settings = settings;
			UpdateSettings(settings);
		}

		public AnonymizerSettings Settings { get; set; }


		private void expressionsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			var selectedPattern = RegexPatterns[e.RowIndex];
			var currentCellValue = expressionsGrid.CurrentCell.Value;

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
			//Export column

			if (!string.IsNullOrEmpty(selectedPattern.Description) && !string.IsNullOrEmpty(selectedPattern.Pattern))
			{
				//that means is a new added expression and the id is empty
				if (string.IsNullOrEmpty(selectedPattern.Id))
				{
					selectedPattern.Id = Guid.NewGuid().ToString();
				}

				//check if last element is empty if not create a new one
				//in grid will appear a new empty row
				var lastElement = RegexPatterns[RegexPatterns.Count-1];
				if (!string.IsNullOrEmpty(lastElement.Id))
				{
					// create an empty row
					var emptyRow = new RegexPattern
					{
						Description = string.Empty,
						Id = string.Empty,
						Pattern = string.Empty
					};
					RegexPatterns.Add(emptyRow);
				}
				//Settings.EditPattern(selectedPattern);
			}
			
			
		}
	}
}
