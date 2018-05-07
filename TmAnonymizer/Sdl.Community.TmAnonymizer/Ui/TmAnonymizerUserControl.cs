using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.TmAnonymizer.Ui
{
	public partial class TmAnonymizerUserControl : UserControl
	{
		public TmAnonymizerUserControl()
		{
			InitializeComponent();
			var wpfMainWindow = new MainViewControl();
			wpfMainWindow.InitializeComponent();
			elementHost.Child =wpfMainWindow;
		}
	}
}
