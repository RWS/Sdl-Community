using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TMX_Lib.Db;

namespace TMX_TranslationProvider.UI
{
	public partial class TmxViewReport : Form
	{
		private TmxImportReport _report;
		public TmxViewReport(TmxImportReport report)
		{
			_report = report;
			InitializeComponent();
			UpdateReport();
		}

		private void timerUpdateReport_Tick(object sender, EventArgs e)
		{
			if (_report.IsEnded)
				timerUpdateReport.Enabled = false;
			UpdateReport();
		}

		private void UpdateReport()
		{
			tusRead.Text = _report.TUsRead.ToString();
			tusImportedSuccessfully.Text = _report.TUsImportedSuccessfully.ToString();
			tusWithSyntaxErrors.Text = _report.TUsWithSyntaxErrors.ToString();
			tusWithInvalidChars.Text = _report.TUsWithInvalidChars.ToString();
			importedLanguages.Text = _report.LanguageCount > 0 ? _report.LanguageCount.ToString() : "Unknown yet";
			importStartTime.Text = _report.IsStarted ? _report.StartTime.ToLongTimeString() : "";
			importTimeSecs.Text = _report.ReportTimeSecs.ToString();
			error.Text = _report.FatalError;
			if (_report.FatalError != "")
				toolTip1.SetToolTip(error, _report.FatalError);
		}

		private void close_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
