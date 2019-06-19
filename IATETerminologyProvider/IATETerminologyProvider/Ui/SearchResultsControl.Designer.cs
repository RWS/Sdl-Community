namespace IATETerminologyProvider.Ui
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
            this.searchResultsWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // searchResultsWebBrowser
            // 
            this.searchResultsWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchResultsWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.searchResultsWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.searchResultsWebBrowser.Name = "searchResultsWebBrowser";
            this.searchResultsWebBrowser.Size = new System.Drawing.Size(992, 567);
            this.searchResultsWebBrowser.TabIndex = 0;
            this.searchResultsWebBrowser.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // SearchResultsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchResultsWebBrowser);
            this.Name = "SearchResultsControl";
            this.Size = new System.Drawing.Size(992, 567);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser searchResultsWebBrowser;
	}
}
