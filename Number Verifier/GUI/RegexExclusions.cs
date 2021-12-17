using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.GUI
{
	public partial class RegexExclusions : Form
	{
		private readonly RegexImporter _regexImporter;

		public RegexExclusions(RegexImporter regexImporter)
		{
			_regexImporter = regexImporter;
			InitializeComponent();
		}

		private BindingList<RegexPattern> RegexPatterns { get; set; }

		public List<RegexPattern> GetData() => RegexPatterns.Where(rp => !string.IsNullOrWhiteSpace(rp.Pattern)).ToList();

		public void SetData(List<RegexPattern> regexExclusionList)
		{
			RegexPatterns = new BindingList<RegexPattern>(regexExclusionList);
			var bindingSource = new BindingSource(RegexPatterns, null);

			RegexGrid.DataSource = bindingSource;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData != Keys.Escape) return base.ProcessCmdKey(ref msg, keyData);
			Close();

			return true;
		}

		private void AddPatterns(List<string> expressions)
		{
			foreach (var expression in expressions)
			{
				if (RegexPatterns.Any(rp => rp.Pattern == expression)) continue;
				RegexPatterns.Add(new RegexPattern { Pattern = expression });
			}
		}

		private async void ExportButton_Click(object sender, EventArgs e)
		{
			var selectedPatterns = GetSelectedPatterns();

			GetExportFilePath(out var fileDialog, out var result);
			if (result != DialogResult.OK || fileDialog.FileName == string.Empty) return;

			var text = (selectedPatterns.Count > 0
				? selectedPatterns
				: RegexPatterns.Select(rp => rp.Pattern)).Aggregate("",
					(current, pattern) => current + pattern + Environment.NewLine);
			await Task.Run(() => File.WriteAllText(fileDialog.FileName, text));
		}

		private static void GetExportFilePath(out SaveFileDialog fileDialog, out DialogResult result)
		{
			fileDialog = new SaveFileDialog
			{
				Title = PluginResources.Export_selected_expressions,
				Filter = @"Text |*.txt",
				FileName = "NumberVerifier_RegexPatterns"
			};
			result = fileDialog.ShowDialog();
		}

		private List<string> GetSelectedPatterns()
		{
			var selectedPatterns = new List<string>();
			foreach (DataGridViewTextBoxCell cell in RegexGrid.SelectedCells)
			{
				if (cell.ColumnIndex == 1) selectedPatterns.Add(cell.Value?.ToString());
			}

			return selectedPatterns;
		}

		private void ImportButton_Click(object sender, EventArgs e)
		{
			GetImportFilePath(out var fileDialog, out var result);
			if (result != DialogResult.OK || fileDialog.FileNames.Length <= 0) return;

			var patterns = GetPatterns(fileDialog);
			AddPatterns(patterns);
		}

		private List<string> GetPatterns(OpenFileDialog fileDialog)
		{
			var patterns = _regexImporter.ImportPatterns(fileDialog.FileNames.ToList());
			patterns.RemoveAll(string.IsNullOrWhiteSpace);
			return patterns;
		}

		private static void GetImportFilePath(out OpenFileDialog fileDialog, out DialogResult result)
		{
			fileDialog = new OpenFileDialog
			{
				Title = PluginResources.Please_select_the_files_you_want_to_import,
				Filter = @"Text |*.txt",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "txt",
				Multiselect = true
			};
			result = fileDialog.ShowDialog();
		}

		private void RegexExclusions_FormClosing(object sender, FormClosingEventArgs e)
		{
			RemoveInvalidPatterns();
		}

		private void RemoveInvalidPatterns()
		{
			var forRemoval = new List<int>();
			for (var index = 0; index < RegexPatterns.Count; index++)
			{
				var pattern = RegexPatterns[index];
				try
				{
					_ = new Regex(pattern.Pattern);
				}
				catch
				{
					forRemoval.Add(index);
				}
			}

			forRemoval.Reverse();
			forRemoval.ForEach(pi => RegexPatterns.RemoveAt(pi));
		}

		private void RegexGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			var headerText = RegexGrid.Columns[e.ColumnIndex].HeaderText;

			if (!headerText.Equals("Pattern")) return;

			try
			{
				RegexGrid.Rows[e.RowIndex].Cells[1].ErrorText = "";
				_ = new Regex(e.FormattedValue.ToString());
			}
			catch (Exception ex)
			{
				RegexGrid.Rows[e.RowIndex].Cells[1].ErrorText = $"{PluginResources.Pattern_not_valid}({ex.Message})";
			}
		}
	}
}