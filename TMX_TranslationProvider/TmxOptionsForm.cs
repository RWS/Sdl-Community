using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
			_options = options;
			InitializeComponent();
		}

		private void ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
