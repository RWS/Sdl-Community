using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	public partial class BatchAnonymizerSettingsControl : UserControl,ISettingsAware<BatchAnonymizerSettings>
	{
		public BatchAnonymizerSettings Settings { get; set; }

		public BatchAnonymizerSettingsControl()
		{
			InitializeComponent();
		}

	}
}
