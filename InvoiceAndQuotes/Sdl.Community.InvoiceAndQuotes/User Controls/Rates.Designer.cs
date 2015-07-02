namespace Sdl.Community.InvoiceAndQuotes
{
    partial class Rates
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
            this.gridRates = new System.Windows.Forms.DataGridView();
            this.gridAdditionalRates = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridRates)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAdditionalRates)).BeginInit();
            this.SuspendLayout();
            // 
            // gridRates
            // 
            this.gridRates.AllowUserToAddRows = false;
            this.gridRates.AllowUserToDeleteRows = false;
            this.gridRates.AllowUserToOrderColumns = true;
            this.gridRates.AllowUserToResizeColumns = false;
            this.gridRates.AllowUserToResizeRows = false;
            this.gridRates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridRates.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gridRates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridRates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRates.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridRates.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridRates.GridColor = System.Drawing.SystemColors.Control;
            this.gridRates.Location = new System.Drawing.Point(0, 109);
            this.gridRates.Name = "gridRates";
            this.gridRates.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridRates.RowHeadersVisible = false;
            this.gridRates.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridRates.Size = new System.Drawing.Size(295, 202);
            this.gridRates.TabIndex = 0;
            // 
            // gridAdditionalRates
            // 
            this.gridAdditionalRates.AllowUserToAddRows = false;
            this.gridAdditionalRates.AllowUserToDeleteRows = false;
            this.gridAdditionalRates.AllowUserToOrderColumns = true;
            this.gridAdditionalRates.AllowUserToResizeColumns = false;
            this.gridAdditionalRates.AllowUserToResizeRows = false;
            this.gridAdditionalRates.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAdditionalRates.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gridAdditionalRates.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridAdditionalRates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAdditionalRates.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridAdditionalRates.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridAdditionalRates.GridColor = System.Drawing.SystemColors.Control;
            this.gridAdditionalRates.Location = new System.Drawing.Point(0, 0);
            this.gridAdditionalRates.Name = "gridAdditionalRates";
            this.gridAdditionalRates.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridAdditionalRates.RowHeadersVisible = false;
            this.gridAdditionalRates.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridAdditionalRates.Size = new System.Drawing.Size(295, 88);
            this.gridAdditionalRates.TabIndex = 1;
            // 
            // Rates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridAdditionalRates);
            this.Controls.Add(this.gridRates);
            this.Name = "Rates";
            this.Size = new System.Drawing.Size(295, 311);
            this.Load += new System.EventHandler(this.Rates_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridRates)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAdditionalRates)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridRates;
        private System.Windows.Forms.DataGridView gridAdditionalRates;
    }
}
