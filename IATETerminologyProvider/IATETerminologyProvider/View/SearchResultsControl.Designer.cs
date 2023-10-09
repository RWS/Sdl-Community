namespace Sdl.Community.IATETerminologyProvider.View
{
	partial class SearchResultsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.searchResultsWebBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
			((System.ComponentModel.ISupportInitialize)(this.searchResultsWebBrowser)).BeginInit();
			this.SuspendLayout();
			// 
			// searchResultsWebBrowser
			// 
			this.searchResultsWebBrowser.AllowExternalDrop = true;
			this.searchResultsWebBrowser.CreationProperties = null;
			this.searchResultsWebBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
			this.searchResultsWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchResultsWebBrowser.Location = new System.Drawing.Point(0, 0);
			this.searchResultsWebBrowser.Name = "searchResultsWebBrowser";
			this.searchResultsWebBrowser.Size = new System.Drawing.Size(1488, 872);
			this.searchResultsWebBrowser.TabIndex = 0;
			this.searchResultsWebBrowser.ZoomFactor = 1D;
			// 
			// SearchResultsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.searchResultsWebBrowser);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "SearchResultsControl";
			this.Size = new System.Drawing.Size(1488, 872);
			((System.ComponentModel.ISupportInitialize)(this.searchResultsWebBrowser)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public Microsoft.Web.WebView2.WinForms.WebView2 searchResultsWebBrowser;
	}
}
