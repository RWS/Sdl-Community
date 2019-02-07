using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Community.TargetWordCount.Utilities;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.TargetWordCount
{
	public partial class WordCountBatchTaskSettingsControl : UserControl, ISettingsAware<WordCountBatchTaskSettings>
	{
		private readonly Dictionary<RateType, string> displayString = new Dictionary<RateType, string>()
		{
			{ RateType.Locked, "Locked" },
			{ RateType.PerfectMatch, "Perfect Match" },
			{ RateType.ContextMatch, "Context Match" },
			{ RateType.Repetitions, "Repetitions" },
			{ RateType.CrossFileRepetitions, "Cross-file Repetitions" },
			{ RateType.OneHundred, "100%" },
			{ RateType.NinetyFive, "95% - 99%" },
			{ RateType.EightyFive, "85% - 94%" },
			{ RateType.SeventyFive, "75% - 84%" },
			{ RateType.Fifty, "50% - 74%" },
			{ RateType.New, "New" },
			{ RateType.Total, "Total" }
		};

		public WordCountBatchTaskSettingsControl()
		{
			InitializeComponent();
			Settings = new WordCountBatchTaskSettings();
			lineCountCheckBox.CheckedChanged += LineCountCheckBox_CheckedChanged;
			loadButton.Click += LoadButton_Click;
			saveButton.Click += SaveButton_Click;
			reportLockedCheckBox.CheckedChanged += ReportLockedCheckBox_CheckedChanged;
			cultureComboBox.SelectedIndexChanged += CultureComboBox_SelectedIndexChanged;
		}

		public WordCountBatchTaskSettings Settings { get; set; }

		protected override void OnLeave(EventArgs e)
		{
			UpdateSettings();
		}

		protected override void OnLoad(EventArgs e)
		{
			Initialize();

			AddCultures();
			AddRows();

			dataGridView.CellEndEdit += DataGridView_CellEndEdit;

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				column.SortMode = DataGridViewColumnSortMode.NotSortable;
			}
		}

		private void AddCultures()
		{
			cultureComboBox.BeginUpdate();

			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(c => c.EnglishName))
			{
				cultureComboBox.Items.Add(culture.EnglishName);
			}

			cultureComboBox.Text = Settings.Culture;

			cultureComboBox.EndUpdate();
		}

		private void AddRows()
		{
			// Display all rows
			for (int i = 0; i < Enum.GetValues(typeof(RateType)).Length; ++i)
			{
				dataGridView.Rows.Add(new object[] { displayString[Settings.InvoiceRates[i].Type], Settings.InvoiceRates[i].Rate });
			}

			if (Settings.UseLineCount)
			{
				foreach (DataGridViewRow row in dataGridView.Rows)
				{
					row.Visible = false;
				}

				if (Settings.ReportLockedSeperately)
				{
					// Locked is at index 0
					dataGridView.Rows[0].Visible = true;
				}

				// Total is at index 11
				dataGridView.Rows[11].Visible = true;
			}
			else
			{
				if (!Settings.ReportLockedSeperately)
				{
					// Locked is at index 0
					dataGridView.Rows[0].Visible = false;
				}
			}
		}

		private void CultureComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var prevCulture = Settings.Culture;
			Settings.Culture = cultureComboBox.SelectedItem.ToString();

			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				var cell = row.Cells[1];
				var rate = cell.Value.ToString();

				if (!string.IsNullOrWhiteSpace(rate))
				{
					decimal r = 0M;
					string value = string.Empty;
					try
					{
						r = decimal.Parse(rate, System.Globalization.NumberStyles.Currency, CultureRepository.Cultures[prevCulture]);
						value = r.ToString("C2", CultureRepository.Cultures[Settings.Culture]);
					}
					catch (Exception)
					{
					}
					finally
					{
						cell.Value = value;
					}
				}
			}
		}

		private void DataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 1)
			{
				var cell = dataGridView[e.ColumnIndex, e.RowIndex];
				decimal d;

				if (!string.IsNullOrWhiteSpace((string)cell.Value))
				{
					if (decimal.TryParse(cell.Value.ToString(), out d))
					{
						string value = string.Empty;
						try
						{
							value = d.ToString("C2", CultureRepository.Cultures[cultureComboBox.SelectedItem.ToString()]);
						}
						catch (Exception)
						{
						}
						finally
						{
							cell.Value = value;
						}
					}
				}
			}
		}

		private void Initialize()
		{
			if (Settings.UseSource)
			{
				sourceRadioButton.Checked = true;
			}
			else
			{
				targetRadioButton.Checked = true;
			}

			reportLockedCheckBox.Checked = Settings.ReportLockedSeperately;

			lineCountCheckBox.Checked = Settings.UseLineCount;

			charPerLineTextBox.Text = Settings.CharactersPerLine;

			includeSpacesCheckBox.Checked = Settings.IncludeSpaces;
		}

		private void LineCountCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (dataGridView != null && dataGridView.Rows != null && dataGridView.Rows.Count > 0)
			{
				UpdateSettings();
				dataGridView.Rows.Clear();
				AddRows();
			}
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "xml files (*.xml)|*.xml";

			DialogResult dr = ofd.ShowDialog();
			if (dr == DialogResult.OK)
			{
				var fileName = ofd.FileName;
				var s = XmlUtilities.Deserialize(fileName);
				SettingsManager.UpdateSettings(s, Settings);

				Initialize();
				cultureComboBox.Text = Settings.Culture;
				dataGridView.Rows.Clear();
				AddRows();
			}
		}

		//[ContractInvariantMethod]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant()
		{
			//Contract.Invariant(Settings != null);
		}

		private void ReportLockedCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (dataGridView != null && dataGridView.Rows != null && dataGridView.Rows.Count > 0)
			{
				UpdateSettings();
				dataGridView.Rows.Clear();
				AddRows();
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			var sfdg = new SaveFileDialog();
			sfdg.Filter = "xml files (*.xml)|*.xml";

			DialogResult dr = sfdg.ShowDialog();

			if (dr == DialogResult.OK)
			{
				UpdateSettings();
				var fileName = sfdg.FileName;
				var s = SettingsManager.ConvertToSerializableSettings(Settings);
				XmlUtilities.Serialize(s, fileName);
			}
		}

		private void UpdateSettings()
		{
			var invoiceRates = new List<InvoiceItem>();

			foreach (DataGridViewRow row in dataGridView.Rows)
			{
				var rateType = displayString.First(p => p.Value == row.Cells[0].Value.ToString()).Key;
				var rate = row.Cells[1].Value.ToString();

				invoiceRates.Add(new InvoiceItem(rateType, rate));
			}

			Settings.InvoiceRates = invoiceRates;

			Settings.UseSource = sourceRadioButton.Checked;

			Settings.ReportLockedSeperately = reportLockedCheckBox.Checked;

			Settings.Culture = cultureComboBox.SelectedItem.ToString();

			Settings.UseLineCount = lineCountCheckBox.Checked;

			Settings.CharactersPerLine = charPerLineTextBox.Text;

			Settings.IncludeSpaces = includeSpacesCheckBox.Checked;
		}
	}
}