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

namespace TMX_TranslationProvider
{
	public partial class TmxOptionsForm : Form
	{
		private TmxTranslationsOptions _options;
		public TmxTranslationsOptions Options => _options;
		public TmxOptionsForm(TmxTranslationsOptions options)
		{
			_options = options.Clone();
			InitializeComponent();
			fileName.Text = _options.FileName;
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
			var ok = _options.FileName == "" || File.Exists(_options.FileName);
			fileNameIsInvalid.Visible = !ok;
		}
	}
}
