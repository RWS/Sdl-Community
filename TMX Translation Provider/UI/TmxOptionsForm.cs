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

		public ISearchServiceParameters Options => _newOptions;
		public TmxSearchService SearchService => TmxSearchServiceProvider.GetSearchService(_newOptions);

		private SearchServiceParameters _oldOptions, _newOptions;
		private bool _initialized = false;

		private bool _tryDetectLocalMongoDb;

		private List<SearchServiceParameters> _databases;
		private int _databaseIdx = -1;

		public TmxOptionsForm(TmxSearchService searchService)
		{
			InitializeComponent();
			_oldOptions = SearchServiceParameters.Copy(searchService.Options);
			_newOptions = SearchServiceParameters.Copy(searchService.Options);

			_tryDetectLocalMongoDb = TryDetectLocalMongoDb();

			_databases = GlobalSettings.Inst.LocalTmxDatabases.ToList();
			_databaseIdx = _databases.FindIndex(db => db.FullFileName.Equals(_newOptions.FullFileName, StringComparison.InvariantCultureIgnoreCase));
			foreach (var db in _databases)
				dbNames.Items.Add(FriendlyDbInfo(db));
			dbNames.SelectedIndex = _databaseIdx;

			_initialized = true;
			UpdateUI();
		}

		private static string FriendlyDbInfo(SearchServiceParameters info) => $"{info.DbName} (File: {info.FileName})";

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

		private (SearchServiceParameters db,int index) BrowsedFileToDbInfo(string fileName)
		{
			var index = _databases.FindIndex(db => db.FullFileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
			if (index >= 0)
				return (_databases[index], index);

			// new file
			var newDb = new SearchServiceParameters
			{ 
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
			if (_newOptions.FullFileName != "")
				dlg.FileName = _newOptions.FullFileName;
			dlg.Filter = "TMX Files (*.tmx)|*.tmx|All files (*.*)|*.*";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				_newOptions.FullFileName = dlg.FileName;
				var (db, idx) = BrowsedFileToDbInfo(dlg.FileName);
				GlobalSettings.Inst.AddTmxDatabase(db);
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
			var (ok, errorStr) = await TmxSearchService.TryParametersAsync(_newOptions);
			if (ok)
			{
				ClearError();
				importProgress.Visible = importStatus.Visible = true;
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
			bool same = _oldOptions.Equals(_newOptions);
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
				_newOptions.FullFileName = _databases[dbNames.SelectedIndex].FullFileName;
				_newOptions.DbName = _databases[dbNames.SelectedIndex].DbName;
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

		private bool IsLocalhostConnection() => _newOptions.DbConnectionNoPassword.StartsWith("localhost", StringComparison.CurrentCultureIgnoreCase);

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
			if (_newOptions.FullFileName != "")
				dlg.FileName = Path.GetDirectoryName(_newOptions.FullFileName) + "\\" + Path.GetFileNameWithoutExtension(_newOptions.FullFileName)  + "-copy.tmx";
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

			var connectionChanged = SearchService.Options.DbConnectionNoPassword != _newOptions.DbConnectionNoPassword;
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
