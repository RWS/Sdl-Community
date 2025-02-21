using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using TMX_Lib.Search;

namespace TMX_TranslationProvider.UI
{
	public partial class TmxExportForm : Form
	{
		private TmxSearchService _searchService;
		private string _fileName;

		private bool _cancelled;
		private double _progress;

		public TmxExportForm(TmxSearchService searchService, string fileName)
		{
			_searchService = searchService;
			_fileName = fileName;
			InitializeComponent();
			this.fileName.Text = fileName;
		}

		public async void ShowAndStart()
		{
			Show();
			Activate();

			await Task.Run(async () =>
			{
				await _searchService.ExportToFileAsync(_fileName, (progress) =>
				{
					bool cancelled;
					lock (this)
					{
						cancelled = _cancelled;
						_progress = progress;
					}
					return !cancelled;
				});
			});
			ok.Enabled = true;
			timerExport.Enabled = false;
			exportProgress.Visible = false;
			fileName.Text = "Import Complete";
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			lock(this)
				_cancelled = true;
			Close();
		}

		private void ok_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void timerExport_Tick(object sender, EventArgs e)
		{
			exportProgress.Value = (int)(_progress * 100);
		}
	}
}
