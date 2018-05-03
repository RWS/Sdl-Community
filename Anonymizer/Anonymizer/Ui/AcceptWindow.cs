using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.projectAnonymizer.Helpers;

namespace Sdl.Community.projectAnonymizer.Ui
{
	public partial class AcceptWindow : Form
	{
		public AcceptWindow()
		{
			InitializeComponent();
			descriptionLabel.Text = Constants.AcceptDescription();
		}
	}
}
