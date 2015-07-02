namespace Sdl.Community.InvoiceAndQuotes.Customers
{
    partial class AddOrEditCustomer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddOrEditCustomer));
            this.groupCustomer = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupTemplateTemplates = new System.Windows.Forms.GroupBox();
            this.rbTemplateGroupedAnalysis = new System.Windows.Forms.RadioButton();
            this.rbTemplateStandardLines = new System.Windows.Forms.RadioButton();
            this.rbTemplateSimpleWordAnalysis = new System.Windows.Forms.RadioButton();
            this.groupStudioAnalysisBands = new System.Windows.Forms.GroupBox();
            this.groupTemplateTemplates.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCustomer
            // 
            this.groupCustomer.Location = new System.Drawing.Point(12, 12);
            this.groupCustomer.Name = "groupCustomer";
            this.groupCustomer.Size = new System.Drawing.Size(316, 452);
            this.groupCustomer.TabIndex = 2;
            this.groupCustomer.TabStop = false;
            this.groupCustomer.Text = "Customer";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 470);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(170, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 499);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(170, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupTemplateTemplates
            // 
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateGroupedAnalysis);
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateStandardLines);
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateSimpleWordAnalysis);
            this.groupTemplateTemplates.Location = new System.Drawing.Point(334, 12);
            this.groupTemplateTemplates.Name = "groupTemplateTemplates";
            this.groupTemplateTemplates.Size = new System.Drawing.Size(140, 94);
            this.groupTemplateTemplates.TabIndex = 6;
            this.groupTemplateTemplates.TabStop = false;
            this.groupTemplateTemplates.Text = "Templates";
            // 
            // rbTemplateGroupedAnalysis
            // 
            this.rbTemplateGroupedAnalysis.AutoSize = true;
            this.rbTemplateGroupedAnalysis.Location = new System.Drawing.Point(8, 62);
            this.rbTemplateGroupedAnalysis.Name = "rbTemplateGroupedAnalysis";
            this.rbTemplateGroupedAnalysis.Size = new System.Drawing.Size(107, 17);
            this.rbTemplateGroupedAnalysis.TabIndex = 17;
            this.rbTemplateGroupedAnalysis.Text = "Grouped Analysis";
            this.rbTemplateGroupedAnalysis.UseVisualStyleBackColor = true;
            this.rbTemplateGroupedAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbTemplateStandardLines
            // 
            this.rbTemplateStandardLines.AutoSize = true;
            this.rbTemplateStandardLines.Location = new System.Drawing.Point(8, 39);
            this.rbTemplateStandardLines.Name = "rbTemplateStandardLines";
            this.rbTemplateStandardLines.Size = new System.Drawing.Size(96, 17);
            this.rbTemplateStandardLines.TabIndex = 16;
            this.rbTemplateStandardLines.Text = "Standard Lines";
            this.rbTemplateStandardLines.UseVisualStyleBackColor = true;
            this.rbTemplateStandardLines.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbTemplateSimpleWordAnalysis
            // 
            this.rbTemplateSimpleWordAnalysis.AutoSize = true;
            this.rbTemplateSimpleWordAnalysis.Location = new System.Drawing.Point(8, 16);
            this.rbTemplateSimpleWordAnalysis.Name = "rbTemplateSimpleWordAnalysis";
            this.rbTemplateSimpleWordAnalysis.Size = new System.Drawing.Size(126, 17);
            this.rbTemplateSimpleWordAnalysis.TabIndex = 15;
            this.rbTemplateSimpleWordAnalysis.TabStop = true;
            this.rbTemplateSimpleWordAnalysis.Text = "Simple Word Analysis";
            this.rbTemplateSimpleWordAnalysis.UseVisualStyleBackColor = true;
            this.rbTemplateSimpleWordAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // groupStudioAnalysisBands
            // 
            this.groupStudioAnalysisBands.Location = new System.Drawing.Point(334, 112);
            this.groupStudioAnalysisBands.Name = "groupStudioAnalysisBands";
            this.groupStudioAnalysisBands.Size = new System.Drawing.Size(568, 407);
            this.groupStudioAnalysisBands.TabIndex = 5;
            this.groupStudioAnalysisBands.TabStop = false;
            this.groupStudioAnalysisBands.Text = "Studio Analysis Bands";
            // 
            // AddOrEditCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 534);
            this.Controls.Add(this.groupTemplateTemplates);
            this.Controls.Add(this.groupStudioAnalysisBands);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupCustomer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddOrEditCustomer";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddOrEditCustomer";
            this.Load += new System.EventHandler(this.AddOrEditCustomer_Load);
            this.groupTemplateTemplates.ResumeLayout(false);
            this.groupTemplateTemplates.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupCustomer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupTemplateTemplates;
        private System.Windows.Forms.RadioButton rbTemplateGroupedAnalysis;
        private System.Windows.Forms.RadioButton rbTemplateStandardLines;
        private System.Windows.Forms.RadioButton rbTemplateSimpleWordAnalysis;
        private System.Windows.Forms.GroupBox groupStudioAnalysisBands;
    }
}