using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Anonymizer.Ui
{
	public class AnonymizerTaskSettingsControl : UserControl, ISettingsAware<AnonymizerSettings>
	{
		private ExpressionsControl expressionsControl1;
		private NewExpressionControl newExpressionControl1;
		private TableLayoutPanel mainTable;

		public AnonymizerSettings Settings
		{
			get;
			set;
		}

		public AnonymizerTaskSettingsControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			newExpressionControl1.SetSettings(Settings);
			expressionsControl1.SetSettings(Settings);
		}

		private void InitializeComponent()
		{
			this.mainTable = new System.Windows.Forms.TableLayoutPanel();
			this.expressionsControl1 = new Sdl.Community.Anonymizer.Ui.ExpressionsControl();
			this.newExpressionControl1 = new Sdl.Community.Anonymizer.Ui.NewExpressionControl();
			this.mainTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTable
			// 
			this.mainTable.ColumnCount = 1;
			this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTable.Controls.Add(this.expressionsControl1, 0, 0);
			this.mainTable.Controls.Add(this.newExpressionControl1, 0, 1);
			this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTable.Location = new System.Drawing.Point(0, 0);
			this.mainTable.Name = "mainTable";
			this.mainTable.RowCount = 2;
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
			this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
			this.mainTable.Size = new System.Drawing.Size(753, 473);
			this.mainTable.TabIndex = 0;
			// 
			// expressionsControl1
			// 
			this.expressionsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionsControl1.Location = new System.Drawing.Point(3, 3);
			this.expressionsControl1.Name = "expressionsControl1";
			this.expressionsControl1.Size = new System.Drawing.Size(747, 301);
			this.expressionsControl1.TabIndex = 0;
			// 
			// newExpressionControl1
			// 
			this.newExpressionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.newExpressionControl1.Location = new System.Drawing.Point(3, 310);
			this.newExpressionControl1.Name = "newExpressionControl1";
			this.newExpressionControl1.Size = new System.Drawing.Size(747, 160);
			this.newExpressionControl1.TabIndex = 1;
			// 
			// AnonymizerTaskSettingsControl
			// 
			this.Controls.Add(this.mainTable);
			this.Name = "AnonymizerTaskSettingsControl";
			this.Size = new System.Drawing.Size(753, 473);
			this.mainTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
