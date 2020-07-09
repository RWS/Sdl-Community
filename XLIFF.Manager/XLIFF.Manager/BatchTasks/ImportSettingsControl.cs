using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.Model.ProjectSettings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public partial class ImportSettingsControl : UserControl, ISettingsAware<XliffManagerImportSettings>, IUISettingsControl
	{
		public ImportSettingsControl()
		{
			InitializeComponent();
		}
		
		public XliffManagerImportSettings Settings { get; set; }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			
			base.Dispose(disposing);
		}
	}
}
