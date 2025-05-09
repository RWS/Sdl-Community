using System.Threading.Tasks;

namespace GroupshareExcelAddIn
{
    partial class GroupshareRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.groupShareTab = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.btnConnect = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.showProjectFormButton = this.Factory.CreateRibbonButton();
            this.group3 = this.Factory.CreateRibbonGroup();
            this.btnShowResourcesForm = this.Factory.CreateRibbonButton();
            this.group4 = this.Factory.CreateRibbonGroup();
            this.btnGetUserData = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.groupShareTab.SuspendLayout();
            this.group1.SuspendLayout();
            this.group2.SuspendLayout();
            this.group3.SuspendLayout();
            this.group4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // groupShareTab
            // 
            this.groupShareTab.Groups.Add(this.group1);
            this.groupShareTab.Groups.Add(this.group2);
            this.groupShareTab.Groups.Add(this.group3);
            this.groupShareTab.Groups.Add(this.group4);
            this.groupShareTab.Label = "GROUPSHARE";
            this.groupShareTab.Name = "groupShareTab";
            // 
            // group1
            // 
            this.group1.Items.Add(this.btnConnect);
            this.group1.Label = "Authentication";
            this.group1.Name = "group1";
            // 
            // btnConnect
            // 
            this.btnConnect.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConnect.Image = global::GroupshareExcelAddIn.Properties.Resources.login;
            this.btnConnect.Label = "Login";
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.ShowImage = true;
            this.btnConnect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.LoginButton_Click);
            // 
            // group2
            // 
            this.group2.Items.Add(this.showProjectFormButton);
            this.group2.Label = "Projects";
            this.group2.Name = "group2";
            // 
            // showProjectFormButton
            // 
            this.showProjectFormButton.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.showProjectFormButton.Image = global::GroupshareExcelAddIn.Properties.Resources.show_project;
            this.showProjectFormButton.Label = "Get Project Data";
            this.showProjectFormButton.Name = "showProjectFormButton";
            this.showProjectFormButton.ShowImage = true;
            this.showProjectFormButton.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ShowProjectFormButton_Click);
            // 
            // group3
            // 
            this.group3.Items.Add(this.btnShowResourcesForm);
            this.group3.Label = "Resources";
            this.group3.Name = "group3";
            // 
            // btnShowResourcesForm
            // 
            this.btnShowResourcesForm.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnShowResourcesForm.Image = global::GroupshareExcelAddIn.Properties.Resources.resource;
            this.btnShowResourcesForm.Label = "Get Resources";
            this.btnShowResourcesForm.Name = "btnShowResourcesForm";
            this.btnShowResourcesForm.ShowImage = true;
            this.btnShowResourcesForm.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ShowResourcesFormButton_Click);
            // 
            // group4
            // 
            this.group4.Items.Add(this.btnGetUserData);
            this.group4.Label = "Users";
            this.group4.Name = "group4";
            // 
            // btnGetUserData
            // 
            this.btnGetUserData.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnGetUserData.Image = global::GroupshareExcelAddIn.Properties.Resources.user_data;
            this.btnGetUserData.Label = "Get User Data";
            this.btnGetUserData.Name = "btnGetUserData";
            this.btnGetUserData.ShowImage = true;
            this.btnGetUserData.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ShowUserDataButton_Click);
            // 
            // GroupshareRibbon
            // 
            this.Name = "GroupshareRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Tabs.Add(this.groupShareTab);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.groupShareTab.ResumeLayout(false);
            this.groupShareTab.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.group3.ResumeLayout(false);
            this.group3.PerformLayout();
            this.group4.ResumeLayout(false);
            this.group4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        private Microsoft.Office.Tools.Ribbon.RibbonTab groupShareTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConnect;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton showProjectFormButton;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnShowResourcesForm;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group4;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnGetUserData;
    }

    partial class ThisRibbonCollection
    {
        internal GroupshareRibbon GroupshareRibbon
        {
            get { return this.GetRibbon<GroupshareRibbon>(); }
        }
    }
}
