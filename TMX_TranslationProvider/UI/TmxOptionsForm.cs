using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMX_Lib.Search;

namespace TMX_TranslationProvider
{
	public partial class TmxOptionsForm : Form
	{
		private TmxTranslationsOptions _options;
		private TmxSearchService _oldSearchService;

		// this can change, if the user updates the connection
		// the idea -> don't modify the initial search service, just in case the user presses Cancel
		private TmxSearchService _newSearchService = null;

		public TmxTranslationsOptions Options => _options;
		public TmxSearchService SearchService => _newSearchService ?? _oldSearchService;

		private EditOptions _oldOptions, _newOptions;
		private bool _initialized = false;

		public TmxOptionsForm(TmxTranslationsOptions options, TmxSearchService searchService)
		{
			_options = options.Clone();
			_oldSearchService = searchService;
			_newSearchService = null;
			InitializeComponent();
			_initialized = true;
			_oldOptions = EditOptions.FromTranslationOptions(options);
			_newOptions = EditOptions.FromTranslationOptions(options);

			fileName.Text = _newOptions.FileName ;
			dbConnection.Text = _newOptions.Connection ;
			dbPassword.Text = _newOptions.Password; 
			dbName.Text = _newOptions.DatabaseName ;

			UpdateUI();
		}

		private void UpdateSearchService()
		{
			var same = _oldOptions == _newOptions;
			if (!same)
			{
				if (_newSearchService == null)
					_newSearchService = new TmxSearchService(_newOptions.ToOptions());
			}

			if (_newSearchService == null)
				return ;

			// update options
			var sameNewOptions = _newSearchService.Options.Equals(_newOptions.ToOptions());
			if (!sameNewOptions)
				// async call
				_newSearchService.SetOptionsAsync(_newOptions.ToOptions());
		}

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void browse_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			if (_options.FileName != "")
				dlg.FileName = _options.FileName;
			dlg.Filter = "TMX Files (*.tmx)|*.tmx|All files (*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				_options.FileName = dlg.FileName;
				fileName.Text = dlg.FileName;
			}
		}

		private void fileName_TextChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private async void tryConnect_Click(object sender, EventArgs e)
		{
			UpdateOptions();
			Enabled = false;
			Cursor = Cursors.WaitCursor;
			var (ok, errorStr) = await TmxSearchService.TryParametersAsync(_newOptions.ToOptions());
			if (ok)
			{
				error.Text = "";
				importProgress.Visible = importStatus.Visible = true;
				UpdateSearchService();
				importProgress.Value = (int)(SearchService.ImportProgress() * 100);
				importStatus.Text = "Importing Data...";
				timerImportProgress.Enabled = true;
			}
			else
			{
				error.Text = errorStr;
				importProgress.Visible = importStatus.Visible = false;
				timerImportProgress.Enabled = false;
			}
			Cursor = Cursors.Default;
			Enabled = true;
		}

		private void UpdateTryConnectEnabled()
		{
			bool same = SearchService.Options.Equals(_newOptions.ToOptions());
			tryConnect.Enabled = !same || error.Visible;
		}

		private void UpdateOptions()
		{
			if (!_initialized)
				return;
			_newOptions.FileName = fileName.Text;
			_newOptions.Connection = dbConnection.Text;
			_newOptions.Password = dbPassword.Text;
			_newOptions.DatabaseName = dbName.Text;
			UpdateTryConnectEnabled();
		}

		private void timerImportProgress_Tick(object sender, EventArgs e)
		{
			UpdateUI();
			if (SearchService.ImportComplete())
				timerImportProgress.Enabled = false;
		}

		private void dbConnection_TextChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void dbPassword_TextChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void dbName_TextChanged(object sender, EventArgs e)
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			if (!_initialized)
				return;
			UpdateOptions();

			importProgress.Visible = SearchService.IsImporting() && !SearchService.ImportComplete();
			importStatus.Visible = SearchService.IsImporting() || SearchService.ImportComplete();
			importProgress.Value = (int)(SearchService.ImportProgress() * 100);
			importStatus.Text = !SearchService.ImportComplete() ? "Importing Data..." : "Import Complete";
		}
	}
}
