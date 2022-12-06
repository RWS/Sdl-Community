using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using TMX_Lib.Search;
using TMX_Lib.Utils;

namespace TMX_TranslationProvider
{
	public partial class TmxOptionsForm : Form
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private TmxSearchService _oldSearchService;

		// this can change, if the user updates the connection
		// the idea -> don't modify the initial search service, just in case the user presses Cancel
		private TmxSearchService _newSearchService = null;

		public TmxTranslationsOptions Options => _newOptions.ToOptions();
		public TmxSearchService SearchService => _newSearchService ?? _oldSearchService;

		private EditOptions _oldOptions, _newOptions;
		private bool _initialized = false;

		private bool _tryDetectLocalMongoDb;

		public TmxOptionsForm(TmxTranslationsOptions options, TmxSearchService searchService)
		{
			_oldSearchService = searchService;
			_newSearchService = null;
			InitializeComponent();
			_oldOptions = EditOptions.FromTranslationOptions(options);
			_newOptions = EditOptions.FromTranslationOptions(options);
			var guid = options.Guid;
			if (guid == "")
				guid = System.Guid.NewGuid().ToString();
			_oldOptions.Guid = guid;
			_newOptions.Guid = guid;

			_tryDetectLocalMongoDb = TryDetectLocalMongoDb();
			if (_newOptions.Connection == "")
				if (_tryDetectLocalMongoDb)
					_newOptions.Connection = "localhost:27017";

			fileName.Text = _newOptions.FileName ;
			dbConnection.Text = _newOptions.Connection ;
			dbPassword.Text = _newOptions.Password; 
			dbName.Text = _newOptions.DatabaseName ;

			_initialized = true;

			UpdateUI();
		}

		// very simple way to verify if user has Mongodb Community Server installed locally 
		// obviously, it doesn't always work, but it's a very simple method that works probably 98% of the cases
		private static bool TryDetectLocalMongoDb()
		{
			var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			if (programFiles.EndsWith("(x86)"))
				// the idea - we can be run as a 32-bit plugin
				programFiles = programFiles.Substring(0, programFiles.Length - 5).Trim();

			var mongodbServer = $"{programFiles}\\MongoDB\\Server\\6.0\\bin\\mongod.exe";
			var exists = File.Exists(mongodbServer);
			return exists;
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
			UpdateOptions();
			DialogResult = DialogResult.OK;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void browse_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			if (_newOptions.FileName != "")
				dlg.FileName = _newOptions.FileName;
			dlg.Filter = "TMX Files (*.tmx)|*.tmx|All files (*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				_newOptions.FileName = dlg.FileName;
				fileName.Text = dlg.FileName;
				importStatus.Text = "";
				importProgress.Visible = false;
				error.Visible = false;
				error.Text = "";
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
				error.Visible = false;
				error.Text = "";
				importProgress.Visible = importStatus.Visible = true;
				UpdateSearchService();
				importProgress.Value = (int)(SearchService.ImportProgress() * 100);
				importStatus.Text = "Importing Data...";
				timerImportProgress.Enabled = true;
			}
			else
			{
				error.Visible = true;
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
			tryConnect.Enabled = !same || error.Visible 
			                           // ... or, there was no import in this database
			                           || (!SearchService.HasImportBeenDoneBefore() && !SearchService.IsImporting());
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

		private bool IsLocalhostConnection() => _newOptions.Connection.StartsWith("localhost", StringComparison.CurrentCultureIgnoreCase);

		private void downloadCommunityServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(downloadCommunityServer.Tag.ToString());
		}

		private void viewLog_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(LogUtil.LogFileName);
			Process.Start(LogUtil.LogFileName);
		}

		private void UpdateUI()
		{
			if (!_initialized)
				return;
			UpdateOptions();

			var connectionChanged = SearchService.Options.DbConnectionNoPassword != _newOptions.Connection;
			importProgress.Visible = SearchService.IsImporting() && !SearchService.ImportComplete() && !connectionChanged;
			importStatus.Visible = SearchService.IsImporting() || SearchService.ImportComplete() && !connectionChanged;
			importProgress.Value = (int)(SearchService.ImportProgress() * 100);
			importStatus.Text = !SearchService.ImportComplete() ? "Importing Data..." : (SearchService.ImportError() == "" ? "Import Complete" : "Import Failed");

			var isLocalhost = IsLocalhostConnection();
			dbPassword.Visible = dbPasswordLabel.Visible = dbPasswordTip.Visible = !isLocalhost;
			if (_tryDetectLocalMongoDb)
				downloadCommunityServer.Visible = false;

			if (SearchService.ImportError() != "")
			{
				error.Visible = true;
				error.Text = SearchService.ImportError();
			}
		}
	}
}
