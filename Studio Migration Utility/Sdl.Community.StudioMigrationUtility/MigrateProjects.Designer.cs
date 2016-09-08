namespace Sdl.Community.StudioMigrationUtility
{
    partial class MigrateUtility
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MigrateUtility));
            this.projectMigrationWizzard = new CristiPotlog.Controls.Wizard();
            this.welcomePage = new Sdl.Community.Controls.WizardPage();
            this.finalPage = new Sdl.Community.Controls.WizardPage();
            this.translationMemories = new Sdl.Community.Controls.WizardPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkCustomers = new System.Windows.Forms.CheckBox();
            this.chkTranslationMemories = new System.Windows.Forms.CheckBox();
            this.pluginsPage = new Sdl.Community.Controls.WizardPage();
            this.pluginsTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.installedPluginsListView = new BrightIdeasSoftware.ObjectListView();
            this.pluginsColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.moveProjects = new Sdl.Community.Controls.WizardPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.projectsToBeMoved = new BrightIdeasSoftware.ObjectListView();
            this.projectsNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label3 = new System.Windows.Forms.Label();
            this.toStudioVersion = new Sdl.Community.Controls.WizardPage();
            this.tableLayoutPanelDestinationStudioVersion = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkDestinationStudioVersion = new BrightIdeasSoftware.ObjectListView();
            this.destinationStudioVersionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.fromStudioVersion = new Sdl.Community.Controls.WizardPage();
            this.tableLayoutSourceStudioVersions = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkSourceStudioVersions = new BrightIdeasSoftware.ObjectListView();
            this.sourceStudioVersionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label2 = new System.Windows.Forms.Label();
            this.taskRunnerPage = new Sdl.Community.Controls.WizardPage();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressLongTask = new System.Windows.Forms.ProgressBar();
            this.projectMigrationWizzard.SuspendLayout();
            this.translationMemories.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.pluginsPage.SuspendLayout();
            this.pluginsTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installedPluginsListView)).BeginInit();
            this.moveProjects.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectsToBeMoved)).BeginInit();
            this.toStudioVersion.SuspendLayout();
            this.tableLayoutPanelDestinationStudioVersion.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkDestinationStudioVersion)).BeginInit();
            this.fromStudioVersion.SuspendLayout();
            this.tableLayoutSourceStudioVersions.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSourceStudioVersions)).BeginInit();
            this.taskRunnerPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectMigrationWizzard
            // 
            this.projectMigrationWizzard.Controls.Add(this.pluginsPage);
            this.projectMigrationWizzard.Controls.Add(this.moveProjects);
            this.projectMigrationWizzard.Controls.Add(this.toStudioVersion);
            this.projectMigrationWizzard.Controls.Add(this.fromStudioVersion);
            this.projectMigrationWizzard.Controls.Add(this.welcomePage);
            this.projectMigrationWizzard.Controls.Add(this.finalPage);
            this.projectMigrationWizzard.Controls.Add(this.translationMemories);
            this.projectMigrationWizzard.Controls.Add(this.taskRunnerPage);
            this.projectMigrationWizzard.HeaderImage = global::Sdl.Community.StudioMigrationUtility.Properties.Resources.migrate;
            this.projectMigrationWizzard.Location = new System.Drawing.Point(0, 0);
            this.projectMigrationWizzard.Name = "projectMigrationWizzard";
            this.projectMigrationWizzard.Pages.AddRange(new Sdl.Community.Controls.WizardPage[] {
            this.welcomePage,
            this.fromStudioVersion,
            this.toStudioVersion,
            this.moveProjects,
            this.pluginsPage,
            this.translationMemories,
            this.taskRunnerPage,
            this.finalPage});
            this.projectMigrationWizzard.Size = new System.Drawing.Size(550, 355);
            this.projectMigrationWizzard.TabIndex = 0;
            this.projectMigrationWizzard.WelcomeImage = global::Sdl.Community.StudioMigrationUtility.Properties.Resources.sdl_logo;
            this.projectMigrationWizzard.BeforeSwitchPages += new CristiPotlog.Controls.Wizard.BeforeSwitchPagesEventHandler(this.projectMigrationWizzard_BeforeSwitchPages);
            this.projectMigrationWizzard.AfterSwitchPages += new CristiPotlog.Controls.Wizard.AfterSwitchPagesEventHandler(this.projectMigrationWizzard_AfterSwitchPages);
            // 
            // welcomePage
            // 
            this.welcomePage.Description = "This wizard will guide you through the steps of performing a Studio migration.";
            this.welcomePage.Location = new System.Drawing.Point(0, 0);
            this.welcomePage.Name = "welcomePage";
            this.welcomePage.Size = new System.Drawing.Size(550, 307);
            this.welcomePage.Style = Sdl.Community.Controls.WizardPageStyle.Welcome;
            this.welcomePage.TabIndex = 10;
            this.welcomePage.Title = "Studio migration utility";
            // 
            // finalPage
            // 
            this.finalPage.Location = new System.Drawing.Point(0, 0);
            this.finalPage.Name = "finalPage";
            this.finalPage.Size = new System.Drawing.Size(550, 307);
            this.finalPage.Style = Sdl.Community.Controls.WizardPageStyle.Finish;
            this.finalPage.TabIndex = 15;
            this.finalPage.Title = "Migration has finished.";
            // 
            // translationMemories
            // 
            this.translationMemories.Controls.Add(this.tableLayoutPanel2);
            this.translationMemories.Location = new System.Drawing.Point(0, 0);
            this.translationMemories.Name = "translationMemories";
            this.translationMemories.Size = new System.Drawing.Size(428, 208);
            this.translationMemories.TabIndex = 16;
            this.translationMemories.Title = "Translation Memories";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkCustomers, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.chkTranslationMemories, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 78);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 106F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(518, 213);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(239, 26);
            this.label5.TabIndex = 6;
            this.label5.Text = "Selecting this option will result in all Customers recreated in the application y" +
    "ou are migrating to.  ";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 91);
            this.label4.TabIndex = 3;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // chkCustomers
            // 
            this.chkCustomers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCustomers.AutoSize = true;
            this.chkCustomers.Location = new System.Drawing.Point(262, 151);
            this.chkCustomers.Name = "chkCustomers";
            this.chkCustomers.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chkCustomers.Size = new System.Drawing.Size(253, 17);
            this.chkCustomers.TabIndex = 5;
            this.chkCustomers.Text = "Migrate Customers";
            this.chkCustomers.UseVisualStyleBackColor = true;
            // 
            // chkTranslationMemories
            // 
            this.chkTranslationMemories.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTranslationMemories.AutoSize = true;
            this.chkTranslationMemories.Location = new System.Drawing.Point(262, 45);
            this.chkTranslationMemories.Name = "chkTranslationMemories";
            this.chkTranslationMemories.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.chkTranslationMemories.Size = new System.Drawing.Size(253, 17);
            this.chkTranslationMemories.TabIndex = 4;
            this.chkTranslationMemories.Text = "Update translation memories";
            this.chkTranslationMemories.UseVisualStyleBackColor = true;
            // 
            // pluginsPage
            // 
            this.pluginsPage.Controls.Add(this.pluginsTableLayout);
            this.pluginsPage.Description = "Select plugins which you want to migrate.";
            this.pluginsPage.Location = new System.Drawing.Point(0, 0);
            this.pluginsPage.Name = "pluginsPage";
            this.pluginsPage.Size = new System.Drawing.Size(550, 307);
            this.pluginsPage.TabIndex = 17;
            this.pluginsPage.Title = "Installed plugins list";
            // 
            // pluginsTableLayout
            // 
            this.pluginsTableLayout.ColumnCount = 2;
            this.pluginsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pluginsTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pluginsTableLayout.Controls.Add(this.installedPluginsListView, 1, 0);
            this.pluginsTableLayout.Controls.Add(this.descriptionLabel, 0, 0);
            this.pluginsTableLayout.Location = new System.Drawing.Point(12, 78);
            this.pluginsTableLayout.Name = "pluginsTableLayout";
            this.pluginsTableLayout.RowCount = 1;
            this.pluginsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pluginsTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pluginsTableLayout.Size = new System.Drawing.Size(518, 213);
            this.pluginsTableLayout.TabIndex = 0;
            // 
            // installedPluginsListView
            // 
            this.installedPluginsListView.AllColumns.Add(this.pluginsColumn);
            this.installedPluginsListView.CheckBoxes = true;
            this.installedPluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginsColumn});
            this.installedPluginsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.installedPluginsListView.EmptyListMsg = "All plugins from the source Studio version ar available on the destination Studio" +
    " version.";
            this.installedPluginsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.installedPluginsListView.Location = new System.Drawing.Point(262, 3);
            this.installedPluginsListView.Name = "installedPluginsListView";
            this.installedPluginsListView.Size = new System.Drawing.Size(253, 207);
            this.installedPluginsListView.TabIndex = 0;
            this.installedPluginsListView.UseCompatibleStateImageBehavior = false;
            this.installedPluginsListView.View = System.Windows.Forms.View.Details;
            // 
            // pluginsColumn
            // 
            this.pluginsColumn.CheckBoxes = true;
            this.pluginsColumn.FillsFreeSpace = true;
            this.pluginsColumn.Groupable = false;
            this.pluginsColumn.IsEditable = false;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Location = new System.Drawing.Point(3, 93);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(251, 26);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "All selected plugins will be migrated to the version of Studio selected in the pr" +
    "evious steps.";
            // 
            // moveProjects
            // 
            this.moveProjects.Controls.Add(this.tableLayoutPanel1);
            this.moveProjects.Location = new System.Drawing.Point(0, 0);
            this.moveProjects.Name = "moveProjects";
            this.moveProjects.Size = new System.Drawing.Size(550, 307);
            this.moveProjects.TabIndex = 13;
            this.moveProjects.Title = "Change projects location";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 78);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(518, 213);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.projectsToBeMoved);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(262, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(253, 207);
            this.panel3.TabIndex = 2;
            // 
            // projectsToBeMoved
            // 
            this.projectsToBeMoved.AllColumns.Add(this.projectsNameColumn);
            this.projectsToBeMoved.CheckBoxes = true;
            this.projectsToBeMoved.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.projectsNameColumn});
            this.projectsToBeMoved.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectsToBeMoved.EmptyListMsg = "All projects from the source Studio version are available on the destination Stud" +
    "io version.";
            this.projectsToBeMoved.FullRowSelect = true;
            this.projectsToBeMoved.HasCollapsibleGroups = false;
            this.projectsToBeMoved.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.projectsToBeMoved.Location = new System.Drawing.Point(0, 0);
            this.projectsToBeMoved.Name = "projectsToBeMoved";
            this.projectsToBeMoved.Size = new System.Drawing.Size(253, 207);
            this.projectsToBeMoved.TabIndex = 0;
            this.projectsToBeMoved.UseCompatibleStateImageBehavior = false;
            this.projectsToBeMoved.View = System.Windows.Forms.View.Details;
            // 
            // projectsNameColumn
            // 
            this.projectsNameColumn.AspectName = "";
            this.projectsNameColumn.CheckBoxes = true;
            this.projectsNameColumn.FillsFreeSpace = true;
            this.projectsNameColumn.Groupable = false;
            this.projectsNameColumn.IsEditable = false;
            this.projectsNameColumn.Width = 151;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(251, 65);
            this.label3.TabIndex = 3;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // toStudioVersion
            // 
            this.toStudioVersion.Controls.Add(this.tableLayoutPanelDestinationStudioVersion);
            this.toStudioVersion.Description = "Select Studio version to which you want to migrate the projects.";
            this.toStudioVersion.Location = new System.Drawing.Point(0, 0);
            this.toStudioVersion.Name = "toStudioVersion";
            this.toStudioVersion.Size = new System.Drawing.Size(550, 307);
            this.toStudioVersion.TabIndex = 12;
            this.toStudioVersion.Tag = "";
            this.toStudioVersion.Title = "Destination Studio Version";
            // 
            // tableLayoutPanelDestinationStudioVersion
            // 
            this.tableLayoutPanelDestinationStudioVersion.ColumnCount = 2;
            this.tableLayoutPanelDestinationStudioVersion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDestinationStudioVersion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDestinationStudioVersion.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanelDestinationStudioVersion.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelDestinationStudioVersion.Location = new System.Drawing.Point(12, 78);
            this.tableLayoutPanelDestinationStudioVersion.Name = "tableLayoutPanelDestinationStudioVersion";
            this.tableLayoutPanelDestinationStudioVersion.RowCount = 1;
            this.tableLayoutPanelDestinationStudioVersion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelDestinationStudioVersion.Size = new System.Drawing.Size(518, 213);
            this.tableLayoutPanelDestinationStudioVersion.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chkDestinationStudioVersion);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(262, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(253, 207);
            this.panel2.TabIndex = 2;
            // 
            // chkDestinationStudioVersion
            // 
            this.chkDestinationStudioVersion.AllColumns.Add(this.destinationStudioVersionColumn);
            this.chkDestinationStudioVersion.CheckBoxes = true;
            this.chkDestinationStudioVersion.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.destinationStudioVersionColumn});
            this.chkDestinationStudioVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkDestinationStudioVersion.FullRowSelect = true;
            this.chkDestinationStudioVersion.HasCollapsibleGroups = false;
            this.chkDestinationStudioVersion.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.chkDestinationStudioVersion.Location = new System.Drawing.Point(0, 0);
            this.chkDestinationStudioVersion.Name = "chkDestinationStudioVersion";
            this.chkDestinationStudioVersion.Size = new System.Drawing.Size(253, 207);
            this.chkDestinationStudioVersion.TabIndex = 0;
            this.chkDestinationStudioVersion.UseCompatibleStateImageBehavior = false;
            this.chkDestinationStudioVersion.View = System.Windows.Forms.View.Details;
            // 
            // destinationStudioVersionColumn
            // 
            this.destinationStudioVersionColumn.AspectName = "";
            this.destinationStudioVersionColumn.CheckBoxes = true;
            this.destinationStudioVersionColumn.FillsFreeSpace = true;
            this.destinationStudioVersionColumn.Groupable = false;
            this.destinationStudioVersionColumn.IsEditable = false;
            this.destinationStudioVersionColumn.Width = 151;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Studio version to which you want to migrate the projects.";
            // 
            // fromStudioVersion
            // 
            this.fromStudioVersion.Controls.Add(this.tableLayoutSourceStudioVersions);
            this.fromStudioVersion.Description = "Select Studio version from which you want to migrate the projects.";
            this.fromStudioVersion.Location = new System.Drawing.Point(0, 0);
            this.fromStudioVersion.Name = "fromStudioVersion";
            this.fromStudioVersion.Size = new System.Drawing.Size(550, 307);
            this.fromStudioVersion.TabIndex = 11;
            this.fromStudioVersion.Title = "Source Studio version";
            // 
            // tableLayoutSourceStudioVersions
            // 
            this.tableLayoutSourceStudioVersions.ColumnCount = 2;
            this.tableLayoutSourceStudioVersions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSourceStudioVersions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSourceStudioVersions.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutSourceStudioVersions.Controls.Add(this.label2, 0, 0);
            this.tableLayoutSourceStudioVersions.Location = new System.Drawing.Point(12, 78);
            this.tableLayoutSourceStudioVersions.Name = "tableLayoutSourceStudioVersions";
            this.tableLayoutSourceStudioVersions.RowCount = 1;
            this.tableLayoutSourceStudioVersions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSourceStudioVersions.Size = new System.Drawing.Size(518, 213);
            this.tableLayoutSourceStudioVersions.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkSourceStudioVersions);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(262, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 207);
            this.panel1.TabIndex = 2;
            // 
            // chkSourceStudioVersions
            // 
            this.chkSourceStudioVersions.AllColumns.Add(this.sourceStudioVersionColumn);
            this.chkSourceStudioVersions.CheckBoxes = true;
            this.chkSourceStudioVersions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sourceStudioVersionColumn});
            this.chkSourceStudioVersions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSourceStudioVersions.FullRowSelect = true;
            this.chkSourceStudioVersions.HasCollapsibleGroups = false;
            this.chkSourceStudioVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.chkSourceStudioVersions.Location = new System.Drawing.Point(0, 0);
            this.chkSourceStudioVersions.Name = "chkSourceStudioVersions";
            this.chkSourceStudioVersions.Size = new System.Drawing.Size(253, 207);
            this.chkSourceStudioVersions.TabIndex = 0;
            this.chkSourceStudioVersions.UseCompatibleStateImageBehavior = false;
            this.chkSourceStudioVersions.View = System.Windows.Forms.View.Details;
            // 
            // sourceStudioVersionColumn
            // 
            this.sourceStudioVersionColumn.AspectName = "";
            this.sourceStudioVersionColumn.CheckBoxes = true;
            this.sourceStudioVersionColumn.FillsFreeSpace = true;
            this.sourceStudioVersionColumn.Groupable = false;
            this.sourceStudioVersionColumn.IsEditable = false;
            this.sourceStudioVersionColumn.Width = 151;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(222, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Studio version from which you want to migrate the projects.";
            // 
            // taskRunnerPage
            // 
            this.taskRunnerPage.Controls.Add(this.labelProgress);
            this.taskRunnerPage.Controls.Add(this.progressLongTask);
            this.taskRunnerPage.Location = new System.Drawing.Point(0, 0);
            this.taskRunnerPage.Name = "taskRunnerPage";
            this.taskRunnerPage.Size = new System.Drawing.Size(428, 208);
            this.taskRunnerPage.TabIndex = 14;
            this.taskRunnerPage.Title = "Studio projects are being migrated";
            // 
            // labelProgress
            // 
            this.labelProgress.Location = new System.Drawing.Point(16, 132);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(252, 16);
            this.labelProgress.TabIndex = 5;
            this.labelProgress.Text = "Please wait while projects are migrated...";
            // 
            // progressLongTask
            // 
            this.progressLongTask.Location = new System.Drawing.Point(12, 152);
            this.progressLongTask.Name = "progressLongTask";
            this.progressLongTask.Size = new System.Drawing.Size(526, 20);
            this.progressLongTask.TabIndex = 4;
            // 
            // MigrateUtility
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(550, 355);
            this.Controls.Add(this.projectMigrationWizzard);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(800, 394);
            this.MinimumSize = new System.Drawing.Size(16, 394);
            this.Name = "MigrateUtility";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Studio Migration Utility";
            this.projectMigrationWizzard.ResumeLayout(false);
            this.translationMemories.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.pluginsPage.ResumeLayout(false);
            this.pluginsTableLayout.ResumeLayout(false);
            this.pluginsTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installedPluginsListView)).EndInit();
            this.moveProjects.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.projectsToBeMoved)).EndInit();
            this.toStudioVersion.ResumeLayout(false);
            this.tableLayoutPanelDestinationStudioVersion.ResumeLayout(false);
            this.tableLayoutPanelDestinationStudioVersion.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkDestinationStudioVersion)).EndInit();
            this.fromStudioVersion.ResumeLayout(false);
            this.tableLayoutSourceStudioVersions.ResumeLayout(false);
            this.tableLayoutSourceStudioVersions.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSourceStudioVersions)).EndInit();
            this.taskRunnerPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CristiPotlog.Controls.Wizard projectMigrationWizzard;
        private Controls.WizardPage welcomePage;
        private Controls.WizardPage moveProjects;
        private Controls.WizardPage toStudioVersion;
        private Controls.WizardPage fromStudioVersion;
        private Controls.WizardPage finalPage;
        private Controls.WizardPage taskRunnerPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSourceStudioVersions;
        private System.Windows.Forms.Panel panel1;
        private BrightIdeasSoftware.ObjectListView chkSourceStudioVersions;
        private BrightIdeasSoftware.OLVColumn sourceStudioVersionColumn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDestinationStudioVersion;
        private System.Windows.Forms.Panel panel2;
        private BrightIdeasSoftware.ObjectListView chkDestinationStudioVersion;
        private BrightIdeasSoftware.OLVColumn destinationStudioVersionColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel3;
        private BrightIdeasSoftware.ObjectListView projectsToBeMoved;
        private BrightIdeasSoftware.OLVColumn projectsNameColumn;
        private System.Windows.Forms.Label label3;
        private Controls.WizardPage translationMemories;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkTranslationMemories;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressLongTask;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkCustomers;
        private Controls.WizardPage pluginsPage;
        private System.Windows.Forms.TableLayoutPanel pluginsTableLayout;
        private BrightIdeasSoftware.ObjectListView installedPluginsListView;
        private System.Windows.Forms.Label descriptionLabel;
        private BrightIdeasSoftware.OLVColumn pluginsColumn;
    }
}

