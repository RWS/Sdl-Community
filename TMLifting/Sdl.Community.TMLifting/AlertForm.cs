using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.TMLifting
{
	public partial class AlertForm : Form
	{
		public string Message
		{
			set { labelMessage.Text = value; }
		}

		public int ProgressValue
		{
			set { progressBar1.Value = value; }
		}

		public AlertForm()
		{
			InitializeComponent();
		}
	}
}
