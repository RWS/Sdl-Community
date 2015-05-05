namespace Sdl.Community.Jobs.UI
{
    partial class JobsControl
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
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.colTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLanguages = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colJobDetails = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colPostedBy = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLWA = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.colTime);
            this.objectListView1.AllColumns.Add(this.colLanguages);
            this.objectListView1.AllColumns.Add(this.colJobDetails);
            this.objectListView1.AllColumns.Add(this.colPostedBy);
            this.objectListView1.AllColumns.Add(this.colLWA);
            this.objectListView1.AllColumns.Add(this.colStatus);
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTime,
            this.colLanguages,
            this.colJobDetails,
            this.colPostedBy,
            this.colLWA,
            this.colStatus});
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.Size = new System.Drawing.Size(825, 535);
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // colTime
            // 
            this.colTime.Groupable = false;
            this.colTime.Text = "Time";
            // 
            // colLanguages
            // 
            this.colLanguages.Groupable = false;
            this.colLanguages.IsEditable = false;
            this.colLanguages.MaximumWidth = 88;
            this.colLanguages.MinimumWidth = 88;
            this.colLanguages.Text = "Languages";
            this.colLanguages.Width = 88;
            // 
            // colJobDetails
            // 
            this.colJobDetails.FillsFreeSpace = true;
            this.colJobDetails.Text = "Job details";
            // 
            // colPostedBy
            // 
            this.colPostedBy.Text = "Posted by";
            // 
            // colLWA
            // 
            this.colLWA.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colLWA.MaximumWidth = 122;
            this.colLWA.MinimumWidth = 122;
            this.colLWA.Text = "Outsourcer LWA avg";
            this.colLWA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colLWA.Width = 122;
            // 
            // colStatus
            // 
            this.colStatus.Text = "Status";
            // 
            // JobsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.objectListView1);
            this.Name = "JobsControl";
            this.Size = new System.Drawing.Size(825, 535);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn colTime;
        private BrightIdeasSoftware.OLVColumn colLanguages;
        private BrightIdeasSoftware.OLVColumn colJobDetails;
        private BrightIdeasSoftware.OLVColumn colPostedBy;
        private BrightIdeasSoftware.OLVColumn colLWA;
        private BrightIdeasSoftware.OLVColumn colStatus;
    }
}
