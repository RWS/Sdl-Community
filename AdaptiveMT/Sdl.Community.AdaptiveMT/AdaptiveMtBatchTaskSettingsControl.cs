using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.AdaptiveMT
{
	public partial class AdaptiveMtBatchTaskSettingsControl : UserControl,ISettingsAware<AdaptiveMtBatchTaskSettings>
	{
		public AdaptiveMtBatchTaskSettingsControl()
		{
			InitializeComponent();
		}

		public AdaptiveMtBatchTaskSettings Settings { get; set; }
	}
}
