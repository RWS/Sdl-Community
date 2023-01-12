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
using TMX_TranslationProvider.UI;

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

		private List<TmxDbInfo> _databases;
		private int _databaseIdx = -1;

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

			_databases = GlobalSettings.Inst.LocalTmxDatabases.ToList();
			_databaseIdx = _databases.FindIndex(db => db.FullFileName.Equals(_newOptions.FileName, StringComparison.InvariantCultureIgnoreCase));
			foreach (var db in _databases)
				dbNames.Items.Add(FriendlyDbInfo(db));
			dbNames.SelectedIndex = _databaseIdx;

			_initialized = true;
			UpdateUI();
		}

		private static string FriendlyDbInfo(TmxDbInfo info) => $"{info.DbName} (File: {info.FileName})";

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
			GlobalSettings.Inst.Save();
			DialogResult = DialogResult.OK;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private string NewDbName(string wantName)
		{
			for (int idx = 0; ; ++idx)
			{
				var name = wantName;
				if (idx > 0)
					wantName += $" ({idx+1})";
				if (_databases.All(db => db.DbName != name))
					return name;
			}
		}

		private (TmxDbInfo db,int index) BrowsedFileToDbInfo(string fileName)
		{
			var index = _databases.FindIndex(db => db.FullFileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
			if (index >= 0)
				return (_databases[index], index);

			// new file
			var newDb = new TmxDbInfo { 
				FullFileName = fileName,
				DbName = NewDbName( Path.GetFileName(fileName)),
			};
			_databases.Add(newDb);
			index = _databases.Count - 1;
			dbNames.Items.Add(FriendlyDbInfo(newDb));
			return (newDb, index);
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
				var (_, idx) = BrowsedFileToDbInfo(dlg.FileName);
				dbNames.SelectedIndex = idx;
				importStatus.Text = "";
				importProgress.Visible = false;
				ClearError();
			}
		}

		private void ClearError()
		{
			error.Visible = false;
			error.Text = "";
		}
		private void SetError(string errorText)
		{
			Debug.Assert(errorText != "");
			const int MAX_ERR_LEN = 70;
			var text = errorText.Length <= MAX_ERR_LEN ? errorText : errorText.Substring(0, MAX_ERR_LEN) + " ...";
			error.Text = text;
			toolTip1.SetToolTip(error, errorText);
			error.Visible = true;
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
				ClearError();
				importProgress.Visible = importStatus.Visible = true;
				UpdateSearchService();
				importProgress.Value = (int)(SearchService.ImportProgress() * 100);
				importStatus.Text = "Importing Data...";
				timerImportProgress.Enabled = true;
			}
			else
			{
				SetError(errorStr);
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
			if (dbNames.SelectedIndex >= 0)
			{
				_newOptions.FileName = _databases[dbNames.SelectedIndex].FileName;
				_newOptions.DatabaseName = _databases[dbNames.SelectedIndex].DbName;
			}

			_newOptions.QuickImport = quickImport.Checked;
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

		private void viewReport_Click(object sender, EventArgs e)
		{
			var view = new TmxViewReport(SearchService.Report);
			view.Show();
			view.Activate();
		}

		private void exportToTmx_Click(object sender, EventArgs e)
		{
			var dlg = new SaveFileDialog();
			if (_newOptions.FileName != "")
				dlg.FileName = Path.GetDirectoryName(_newOptions.FileName) + "\\" + Path.GetFileNameWithoutExtension(_newOptions.FileName)  + "-copy.tmx";
			dlg.Filter = "TMX Files (*.tmx)|*.tmx|All files (*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				var exportForm = new TmxExportForm(SearchService, dlg.FileName);
				exportForm.ShowAndStart();
			}
		}

		private void dbNames_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_initialized && dbNames.SelectedIndex >= 0)
			{
				UpdateOptions();
			}
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
			viewReport.Enabled = SearchService.Report.IsStarted;
			exportToTmx.Enabled = SearchService.HasImportBeenDoneBefore() || SearchService.ImportComplete() ;

			var isLocalhost = IsLocalhostConnection();
			if (_tryDetectLocalMongoDb)
				downloadCommunityServer.Visible = false;

			if (SearchService.ImportError() != "")
			{
				SetError(SearchService.ImportError());
			}
		}
	}
}
