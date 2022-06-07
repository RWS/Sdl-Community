using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Sdl.Utilities.SplitSDLXLIFF.Wizard
{
	/// <summary>
	/// Summary description for WizardSidebar.
	/// </summary>
	public class WizardSidebar : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WizardSidebar()
		{
			this.Dock = DockStyle.Left;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Set a default image.
			Bitmap image = new Bitmap(this.GetType(), "Bitmaps.ExampleSidebar.bmp");
			this.BackgroundImage = image;

			// Avoid getting the focus.
			this.SetStyle(ControlStyles.Selectable, false);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// WizardSidebar
			// 
			this.Name = "WizardSidebar";
			this.Size = new System.Drawing.Size(165, 320);
		}
		#endregion
	}
}
