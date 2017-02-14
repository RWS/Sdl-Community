using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using Sdl.Community.WPFListView;

namespace Sdl.Community.Qualitivity.Panels.Document_Reports
{
    partial class QualitivityViewTrackChangesControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityViewTrackChangesControl));
            var chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            var stripLine1 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            var stripLine2 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            var stripLine3 = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            var legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            var title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            var chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            var chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            var legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            var series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            var series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            var series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            var dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint4 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint5 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint6 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint7 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var dataPoint8 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(2D, 0D);
            var title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            var title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.mergeProjectActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateTheProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.createAnActivitiesReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportActivitiesToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.webBrowser2 = new System.Windows.Forms.WebBrowser();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.listViewControl1 = new WPFListView.ListViewControl();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.treeView_navigation = new System.Windows.Forms.TreeView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.chart_segmentPerSecond = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel_segment_per_second_properties = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chart_words_per_minute = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel_words_per_min_control = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxShowUnusual = new System.Windows.Forms.CheckBox();
            this.checkBoxShowMedian = new System.Windows.Forms.CheckBox();
            this.checkBoxShowAverage = new System.Windows.Forms.CheckBox();
            this.comboBoxPercentiles = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.webBrowser4 = new System.Windows.Forms.WebBrowser();
            this.webBrowser3 = new System.Windows.Forms.WebBrowser();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.button_create_dqf_project = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_segmentPerSecond)).BeginInit();
            this.panel_segment_per_second_properties.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_words_per_minute)).BeginInit();
            this.panel_words_per_min_control.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Calendar-32(6).png");
            this.imageList1.Images.SetKeyName(1, "Calendar1");
            this.imageList1.Images.SetKeyName(2, "Book-Red-32.png");
            this.imageList1.Images.SetKeyName(3, "Bookmark(1).ico");
            this.imageList1.Images.SetKeyName(4, "Bookmark");
            this.imageList1.Images.SetKeyName(5, "Bookmark1");
            this.imageList1.Images.SetKeyName(6, "Calendar");
            // 
            // mergeProjectActivitiesToolStripMenuItem
            // 
            this.mergeProjectActivitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mergeProjectActivitiesToolStripMenuItem.Image")));
            this.mergeProjectActivitiesToolStripMenuItem.Name = "mergeProjectActivitiesToolStripMenuItem";
            this.mergeProjectActivitiesToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.mergeProjectActivitiesToolStripMenuItem.Text = "Merge Project Activities";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectActivityToolStripMenuItem,
            this.editProjectActivityToolStripMenuItem,
            this.removeProjectActivityToolStripMenuItem,
            this.toolStripSeparator1,
            this.duplicateTheProjectActivityToolStripMenuItem,
            this.mergeProjectActivitiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.createAnActivitiesReportToolStripMenuItem,
            this.exportActivitiesToExcelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(214, 170);
            // 
            // newProjectActivityToolStripMenuItem
            // 
            this.newProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectActivityToolStripMenuItem.Image")));
            this.newProjectActivityToolStripMenuItem.Name = "newProjectActivityToolStripMenuItem";
            this.newProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.newProjectActivityToolStripMenuItem.Text = "New Project Activity";
            // 
            // editProjectActivityToolStripMenuItem
            // 
            this.editProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editProjectActivityToolStripMenuItem.Image")));
            this.editProjectActivityToolStripMenuItem.Name = "editProjectActivityToolStripMenuItem";
            this.editProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.editProjectActivityToolStripMenuItem.Text = "Edit Project Activity";
            // 
            // removeProjectActivityToolStripMenuItem
            // 
            this.removeProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeProjectActivityToolStripMenuItem.Image")));
            this.removeProjectActivityToolStripMenuItem.Name = "removeProjectActivityToolStripMenuItem";
            this.removeProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.removeProjectActivityToolStripMenuItem.Text = "Remove Project Activity";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
            // 
            // duplicateTheProjectActivityToolStripMenuItem
            // 
            this.duplicateTheProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("duplicateTheProjectActivityToolStripMenuItem.Image")));
            this.duplicateTheProjectActivityToolStripMenuItem.Name = "duplicateTheProjectActivityToolStripMenuItem";
            this.duplicateTheProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.duplicateTheProjectActivityToolStripMenuItem.Text = "Duplicate Project Activity";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // createAnActivitiesReportToolStripMenuItem
            // 
            this.createAnActivitiesReportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createAnActivitiesReportToolStripMenuItem.Image")));
            this.createAnActivitiesReportToolStripMenuItem.Name = "createAnActivitiesReportToolStripMenuItem";
            this.createAnActivitiesReportToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.createAnActivitiesReportToolStripMenuItem.Text = "Create an Activities Report";
            // 
            // exportActivitiesToExcelToolStripMenuItem
            // 
            this.exportActivitiesToExcelToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportActivitiesToExcelToolStripMenuItem.Image")));
            this.exportActivitiesToExcelToolStripMenuItem.Name = "exportActivitiesToExcelToolStripMenuItem";
            this.exportActivitiesToExcelToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.exportActivitiesToExcelToolStripMenuItem.Text = "Export Activities to Excel";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tabControl1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(2, 2);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(3);
            this.panel3.Size = new System.Drawing.Size(1289, 471);
            this.panel3.TabIndex = 5;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList2;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1283, 465);
            this.tabControl1.TabIndex = 2;            
            this.tabControl1.VisibleChanged += new System.EventHandler(this.tabControl1_VisibleChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.ImageIndex = 6;
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1275, 430);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Document Overview ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.webBrowser2);
            this.panel1.Controls.Add(this.webBrowser1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(1269, 424);
            this.panel1.TabIndex = 0;
            // 
            // webBrowser2
            // 
            this.webBrowser2.Location = new System.Drawing.Point(269, 55);
            this.webBrowser2.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser2.Name = "webBrowser2";
            this.webBrowser2.Size = new System.Drawing.Size(270, 116);
            this.webBrowser2.TabIndex = 1;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(50, 35);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(199, 136);
            this.webBrowser1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.ImageIndex = 16;
            this.tabPage2.Location = new System.Drawing.Point(4, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1275, 430);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Document Records ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.splitter1);
            this.panel2.Controls.Add(this.treeView_navigation);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(1269, 424);
            this.panel2.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.elementHost1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(229, 3);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.panel4.Size = new System.Drawing.Size(1037, 418);
            this.panel4.TabIndex = 9;
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(2, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(1035, 418);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.listViewControl1;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(226, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 418);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // treeView_navigation
            // 
            this.treeView_navigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView_navigation.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView_navigation.FullRowSelect = true;
            this.treeView_navigation.HideSelection = false;
            this.treeView_navigation.ImageIndex = 0;
            this.treeView_navigation.ImageList = this.imageList1;
            this.treeView_navigation.Indent = 18;
            this.treeView_navigation.ItemHeight = 22;
            this.treeView_navigation.Location = new System.Drawing.Point(3, 3);
            this.treeView_navigation.Name = "treeView_navigation";
            this.treeView_navigation.SelectedImageIndex = 0;
            this.treeView_navigation.ShowNodeToolTips = true;
            this.treeView_navigation.Size = new System.Drawing.Size(223, 418);
            this.treeView_navigation.TabIndex = 1;
            this.treeView_navigation.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_navigation_AfterSelect);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.ImageIndex = 11;
            this.tabPage3.Location = new System.Drawing.Point(4, 31);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1275, 430);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Document Reports";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.tabControl2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(2);
            this.panel5.Size = new System.Drawing.Size(1275, 430);
            this.panel5.TabIndex = 0;
            // 
            // tabControl2
            // 
            this.tabControl2.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.ImageList = this.imageList3;
            this.tabControl2.Location = new System.Drawing.Point(2, 2);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1271, 426);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.chart_segmentPerSecond);
            this.tabPage5.Controls.Add(this.panel_segment_per_second_properties);
            this.tabPage5.ImageIndex = 1;
            this.tabPage5.Location = new System.Drawing.Point(31, 4);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1236, 418);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "Time p/s ";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // chart_segmentPerSecond
            // 
            this.chart_segmentPerSecond.BorderlineWidth = 0;
            chartArea1.Area3DStyle.Inclination = 15;
            chartArea1.Area3DStyle.IsClustered = true;
            chartArea1.Area3DStyle.IsRightAngleAxes = false;
            chartArea1.Area3DStyle.Perspective = 10;
            chartArea1.Area3DStyle.Rotation = 10;
            chartArea1.Area3DStyle.WallWidth = 0;
            chartArea1.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisX.LabelStyle.Interval = 0D;
            chartArea1.AxisX.LabelStyle.IntervalOffset = 0D;
            chartArea1.AxisX.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MajorTickMark.Interval = 0D;
            chartArea1.AxisX.MajorTickMark.IntervalOffset = 0D;
            chartArea1.AxisX.MajorTickMark.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisX2.LabelStyle.Interval = 0D;
            chartArea1.AxisX2.LabelStyle.IntervalOffset = 0D;
            chartArea1.AxisX2.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorGrid.Interval = 0D;
            chartArea1.AxisX2.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisX2.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorTickMark.Interval = 0D;
            chartArea1.AxisX2.MajorTickMark.IntervalOffset = 0D;
            chartArea1.AxisX2.MajorTickMark.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX2.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.LabelStyle.Interval = 0D;
            chartArea1.AxisY.LabelStyle.IntervalOffset = 0D;
            chartArea1.AxisY.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.Interval = 0D;
            chartArea1.AxisY.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisY.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorTickMark.Interval = 0D;
            chartArea1.AxisY.MajorTickMark.IntervalOffset = 0D;
            chartArea1.AxisY.MajorTickMark.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            stripLine1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(241)))), ((int)(((byte)(185)))), ((int)(((byte)(168)))));
            stripLine1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            stripLine1.IntervalOffset = 20D;
            stripLine1.StripWidth = 50D;
            stripLine1.Text = "Standard Deviation";
            stripLine1.TextLineAlignment = System.Drawing.StringAlignment.Far;
            stripLine2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(180)))), ((int)(((byte)(65)))));
            stripLine2.BorderWidth = 2;
            stripLine2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            stripLine2.IntervalOffset = 40D;
            stripLine2.Text = "Mean";
            stripLine2.TextLineAlignment = System.Drawing.StringAlignment.Far;
            stripLine3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(64)))), ((int)(((byte)(10)))));
            stripLine3.BorderWidth = 2;
            stripLine3.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            stripLine3.IntervalOffset = 50D;
            stripLine3.Text = "Median";
            stripLine3.TextAlignment = System.Drawing.StringAlignment.Near;
            stripLine3.TextLineAlignment = System.Drawing.StringAlignment.Far;
            chartArea1.AxisY.StripLines.Add(stripLine1);
            chartArea1.AxisY.StripLines.Add(stripLine2);
            chartArea1.AxisY.StripLines.Add(stripLine3);
            chartArea1.AxisY2.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisY2.LabelStyle.Interval = 0D;
            chartArea1.AxisY2.LabelStyle.IntervalOffset = 0D;
            chartArea1.AxisY2.LabelStyle.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorGrid.Interval = 0D;
            chartArea1.AxisY2.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisY2.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorTickMark.Interval = 0D;
            chartArea1.AxisY2.MajorTickMark.IntervalOffset = 0D;
            chartArea1.AxisY2.MajorTickMark.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisY2.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.BackColor = System.Drawing.Color.Ivory;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.BackSecondaryColor = System.Drawing.Color.White;
            chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.Name = "Default";
            chartArea1.ShadowColor = System.Drawing.Color.Transparent;
            this.chart_segmentPerSecond.ChartAreas.Add(chartArea1);
            this.chart_segmentPerSecond.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Far;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.Enabled = false;
            legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            legend1.IsTextAutoFit = false;
            legend1.Name = "Default";
            legend1.Position.Auto = false;
            legend1.Position.Height = 15F;
            legend1.Position.Width = 30F;
            legend1.Position.X = 63F;
            legend1.Position.Y = 5F;
            this.chart_segmentPerSecond.Legends.Add(legend1);
            this.chart_segmentPerSecond.Location = new System.Drawing.Point(3, 3);
            this.chart_segmentPerSecond.Name = "chart_segmentPerSecond";
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            series1.ChartArea = "Default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.CustomProperties = "LabelStyle=\"down\"";
            series1.Legend = "Default";
            series1.MarkerSize = 3;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "Series1";
            series1.ShadowOffset = 1;
            series1.ToolTip = "#VAL{G}";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series1.YValuesPerPoint = 2;
            this.chart_segmentPerSecond.Series.Add(series1);
            this.chart_segmentPerSecond.Size = new System.Drawing.Size(976, 412);
            this.chart_segmentPerSecond.TabIndex = 6;
            title1.DockedToChartArea = "Default";
            title1.Font = new System.Drawing.Font("Trebuchet MS", 10F, System.Drawing.FontStyle.Bold);
            title1.IsDockedInsideChartArea = false;
            title1.Name = "Title1";
            title1.Text = "Average Time per Segment (Average/Mean = 0.0 seconds)";
            this.chart_segmentPerSecond.Titles.Add(title1);
            // 
            // panel_segment_per_second_properties
            // 
            this.panel_segment_per_second_properties.Controls.Add(this.label8);
            this.panel_segment_per_second_properties.Controls.Add(this.label9);
            this.panel_segment_per_second_properties.Controls.Add(this.label5);
            this.panel_segment_per_second_properties.Controls.Add(this.label6);
            this.panel_segment_per_second_properties.Controls.Add(this.label3);
            this.panel_segment_per_second_properties.Controls.Add(this.label4);
            this.panel_segment_per_second_properties.Controls.Add(this.label2);
            this.panel_segment_per_second_properties.Controls.Add(this.label7);
            this.panel_segment_per_second_properties.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_segment_per_second_properties.Location = new System.Drawing.Point(979, 3);
            this.panel_segment_per_second_properties.Name = "panel_segment_per_second_properties";
            this.panel_segment_per_second_properties.Size = new System.Drawing.Size(254, 412);
            this.panel_segment_per_second_properties.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(145, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(101, 23);
            this.label8.TabIndex = 15;
            this.label8.Text = "...";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(7, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(130, 23);
            this.label9.TabIndex = 14;
            this.label9.Text = "Total Records Evaluated:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(144, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 23);
            this.label5.TabIndex = 13;
            this.label5.Text = "...";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(17, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 23);
            this.label6.TabIndex = 12;
            this.label6.Text = "Median:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(144, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "...";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(17, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 23);
            this.label4.TabIndex = 10;
            this.label4.Text = "Standard Deviation:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(144, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(17, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 23);
            this.label7.TabIndex = 8;
            this.label7.Text = "Mean:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chart_words_per_minute);
            this.tabPage4.Controls.Add(this.panel_words_per_min_control);
            this.tabPage4.ImageIndex = 3;
            this.tabPage4.Location = new System.Drawing.Point(31, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1236, 418);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Words p/m ";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chart_words_per_minute
            // 
            this.chart_words_per_minute.AntiAliasing = System.Windows.Forms.DataVisualization.Charting.AntiAliasingStyles.Graphics;
            this.chart_words_per_minute.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart_words_per_minute.BackSecondaryColor = System.Drawing.Color.White;
            this.chart_words_per_minute.BorderlineWidth = 0;
            this.chart_words_per_minute.BorderSkin.BackColor = System.Drawing.Color.Transparent;
            chartArea2.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea2.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisY.IsStartedFromZero = false;
            chartArea2.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea2.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea2.AxisY.Minimum = 0D;
            chartArea2.BackColor = System.Drawing.Color.WhiteSmoke;
            chartArea2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea2.Name = "Data Chart Area";
            chartArea2.Position.Auto = false;
            chartArea2.Position.Height = 82F;
            chartArea2.Position.Width = 60F;
            chartArea2.Position.X = 2F;
            chartArea2.Position.Y = 12F;
            chartArea3.AlignmentOrientation = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.Horizontal;
            chartArea3.AlignWithChartArea = "Data Chart Area";
            chartArea3.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisX.Maximum = 10D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisY.IsStartedFromZero = false;
            chartArea3.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY.MajorGrid.Enabled = false;
            chartArea3.AxisY.Minimum = 0D;
            chartArea3.BackColor = System.Drawing.Color.Transparent;
            chartArea3.BorderColor = System.Drawing.Color.Empty;
            chartArea3.Name = "Box Chart Area";
            chartArea3.Position.Auto = false;
            chartArea3.Position.Height = 82F;
            chartArea3.Position.Width = 35F;
            chartArea3.Position.X = 62F;
            chartArea3.Position.Y = 12F;
            this.chart_words_per_minute.ChartAreas.Add(chartArea2);
            this.chart_words_per_minute.ChartAreas.Add(chartArea3);
            this.chart_words_per_minute.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Enabled = false;
            legend2.Name = "Default";
            this.chart_words_per_minute.Legends.Add(legend2);
            this.chart_words_per_minute.Location = new System.Drawing.Point(3, 3);
            this.chart_words_per_minute.Name = "chart_words_per_minute";
            series2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            series2.ChartArea = "Data Chart Area";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series2.Legend = "Default";
            series2.MarkerSize = 8;
            series2.Name = "DataSeries";
            series2.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            series2.ShadowOffset = 1;
            series2.ToolTip = "#VAL{G}";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series3.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.VerticalCenter;
            series3.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(224)))), ((int)(((byte)(64)))), ((int)(((byte)(10)))));
            series3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            series3.ChartArea = "Box Chart Area";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.BoxPlot;
            series3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(64)))), ((int)(((byte)(10)))));
            series3.CustomProperties = "DrawSideBySide=True, BoxPlotSeries=DataSeries, PointWidth=1.2, BoxPlotWhiskerPerc" +
    "entile=15";
            series3.Legend = "Default";
            series3.Name = "BoxPlotSeries";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series3.YValuesPerPoint = 6;
            series4.ChartArea = "Box Chart Area";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series4.CustomProperties = "LabelStyle=Right";
            series4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            series4.Legend = "Default";
            series4.Name = "BoxPlotLabels";
            dataPoint1.Color = System.Drawing.Color.Transparent;
            dataPoint2.Color = System.Drawing.Color.Transparent;
            dataPoint3.Color = System.Drawing.Color.Transparent;
            dataPoint4.Color = System.Drawing.Color.Transparent;
            dataPoint5.Color = System.Drawing.Color.Transparent;
            dataPoint6.Color = System.Drawing.Color.Transparent;
            dataPoint7.Color = System.Drawing.Color.Transparent;
            dataPoint8.Color = System.Drawing.Color.Transparent;
            series4.Points.Add(dataPoint1);
            series4.Points.Add(dataPoint2);
            series4.Points.Add(dataPoint3);
            series4.Points.Add(dataPoint4);
            series4.Points.Add(dataPoint5);
            series4.Points.Add(dataPoint6);
            series4.Points.Add(dataPoint7);
            series4.Points.Add(dataPoint8);
            series4.SmartLabelStyle.Enabled = false;
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.chart_words_per_minute.Series.Add(series2);
            this.chart_words_per_minute.Series.Add(series3);
            this.chart_words_per_minute.Series.Add(series4);
            this.chart_words_per_minute.Size = new System.Drawing.Size(1004, 412);
            this.chart_words_per_minute.TabIndex = 8;
            title2.DockedToChartArea = "Data Chart Area";
            title2.DockingOffset = -8;
            title2.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            title2.Name = "Title1";
            title2.Position.Auto = false;
            title2.Position.Height = 5.813029F;
            title2.Position.Width = 48.49561F;
            title2.Position.X = 10F;
            title2.Position.Y = 6F;
            title2.Text = "Words Per Minute";
            title3.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
            title3.DockedToChartArea = "Box Chart Area";
            title3.DockingOffset = -7;
            title3.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Bold);
            title3.Name = "Title2";
            title3.Position.Auto = false;
            title3.Position.Height = 5.813029F;
            title3.Position.Width = 29.7426F;
            title3.Position.X = 69.30817F;
            title3.Position.Y = 6F;
            title3.Text = "Words Distribution";
            this.chart_words_per_minute.Titles.Add(title2);
            this.chart_words_per_minute.Titles.Add(title3);
            this.chart_words_per_minute.PrePaint += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ChartPaintEventArgs>(this.chart1_PrePaint);
            // 
            // panel_words_per_min_control
            // 
            this.panel_words_per_min_control.Controls.Add(this.label10);
            this.panel_words_per_min_control.Controls.Add(this.label11);
            this.panel_words_per_min_control.Controls.Add(this.checkBoxShowUnusual);
            this.panel_words_per_min_control.Controls.Add(this.checkBoxShowMedian);
            this.panel_words_per_min_control.Controls.Add(this.checkBoxShowAverage);
            this.panel_words_per_min_control.Controls.Add(this.comboBoxPercentiles);
            this.panel_words_per_min_control.Controls.Add(this.label1);
            this.panel_words_per_min_control.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel_words_per_min_control.Location = new System.Drawing.Point(1007, 3);
            this.panel_words_per_min_control.Name = "panel_words_per_min_control";
            this.panel_words_per_min_control.Size = new System.Drawing.Size(226, 412);
            this.panel_words_per_min_control.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(129, 158);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 23);
            this.label10.TabIndex = 17;
            this.label10.Text = "label10";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(4, 158);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 23);
            this.label11.TabIndex = 16;
            this.label11.Text = "Total Groups Evaluated:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBoxShowUnusual
            // 
            this.checkBoxShowUnusual.Checked = true;
            this.checkBoxShowUnusual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowUnusual.Location = new System.Drawing.Point(6, 126);
            this.checkBoxShowUnusual.Name = "checkBoxShowUnusual";
            this.checkBoxShowUnusual.Size = new System.Drawing.Size(192, 24);
            this.checkBoxShowUnusual.TabIndex = 4;
            this.checkBoxShowUnusual.Text = "Show &Unusual Points";
            this.checkBoxShowUnusual.CheckedChanged += new System.EventHandler(this.checkBoxShowUnusual_CheckedChanged);
            // 
            // checkBoxShowMedian
            // 
            this.checkBoxShowMedian.Checked = true;
            this.checkBoxShowMedian.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowMedian.Location = new System.Drawing.Point(6, 94);
            this.checkBoxShowMedian.Name = "checkBoxShowMedian";
            this.checkBoxShowMedian.Size = new System.Drawing.Size(192, 24);
            this.checkBoxShowMedian.TabIndex = 3;
            this.checkBoxShowMedian.Text = "Show &Median Line";
            this.checkBoxShowMedian.CheckedChanged += new System.EventHandler(this.checkBoxShowMedian_CheckedChanged);
            // 
            // checkBoxShowAverage
            // 
            this.checkBoxShowAverage.Checked = true;
            this.checkBoxShowAverage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowAverage.Location = new System.Drawing.Point(6, 62);
            this.checkBoxShowAverage.Name = "checkBoxShowAverage";
            this.checkBoxShowAverage.Size = new System.Drawing.Size(192, 24);
            this.checkBoxShowAverage.TabIndex = 2;
            this.checkBoxShowAverage.Text = "Show &Average Line";
            this.checkBoxShowAverage.CheckedChanged += new System.EventHandler(this.checkBoxShowAverage_CheckedChanged);
            // 
            // comboBoxPercentiles
            // 
            this.comboBoxPercentiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPercentiles.Items.AddRange(new object[] {
            "15/85th Percentile",
            "10/90th Percentile",
            "5/95th Percentile",
            "0/100th Percentile (Min/Max)"});
            this.comboBoxPercentiles.Location = new System.Drawing.Point(6, 30);
            this.comboBoxPercentiles.Name = "comboBoxPercentiles";
            this.comboBoxPercentiles.Size = new System.Drawing.Size(168, 21);
            this.comboBoxPercentiles.TabIndex = 1;
            this.comboBoxPercentiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxPercentiles_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Percentiles:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.webBrowser4);
            this.tabPage6.Controls.Add(this.webBrowser3);
            this.tabPage6.ImageIndex = 6;
            this.tabPage6.Location = new System.Drawing.Point(31, 4);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(1236, 418);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Quality Metrics";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // webBrowser4
            // 
            this.webBrowser4.Location = new System.Drawing.Point(274, 207);
            this.webBrowser4.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser4.Name = "webBrowser4";
            this.webBrowser4.Size = new System.Drawing.Size(253, 111);
            this.webBrowser4.TabIndex = 2;
            // 
            // webBrowser3
            // 
            this.webBrowser3.Location = new System.Drawing.Point(339, 56);
            this.webBrowser3.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser3.Name = "webBrowser3";
            this.webBrowser3.Size = new System.Drawing.Size(253, 111);
            this.webBrowser3.TabIndex = 2;
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "Clock-32.png");
            this.imageList3.Images.SetKeyName(1, "Time-Clock-32.png");
            this.imageList3.Images.SetKeyName(2, "Words-With-Friends-Alt-32.png");
            this.imageList3.Images.SetKeyName(3, "Time-Machine-32.png");
            this.imageList3.Images.SetKeyName(4, "Chart-simple-32.png");
            this.imageList3.Images.SetKeyName(5, "Question-type-drag-drop-32.png");
            this.imageList3.Images.SetKeyName(6, "Edit-Yes-32.png");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Chart-Bar-blue-32.png");
            this.imageList2.Images.SetKeyName(1, "Folder-blue-32.png");
            this.imageList2.Images.SetKeyName(2, "Chart-Area-Down-blue-32.png");
            this.imageList2.Images.SetKeyName(3, "Application-blue-32.png");
            this.imageList2.Images.SetKeyName(4, "Camembert-32.png");
            this.imageList2.Images.SetKeyName(5, "Candlestickchart-32.png");
            this.imageList2.Images.SetKeyName(6, "chart_bar-32.png");
            this.imageList2.Images.SetKeyName(7, "Chart-Bar-32.png");
            this.imageList2.Images.SetKeyName(8, "Chart-Down-Color-32.png");
            this.imageList2.Images.SetKeyName(9, "Chart-Graph-32.png");
            this.imageList2.Images.SetKeyName(10, "Chart-Graph-Ascending-32.png");
            this.imageList2.Images.SetKeyName(11, "Chart-Stock-32.png");
            this.imageList2.Images.SetKeyName(12, "Chart-Vertical-Bar-Poll-32.png");
            this.imageList2.Images.SetKeyName(13, "Document-Folder-32.png");
            this.imageList2.Images.SetKeyName(14, "Gnome-Logviewer-32.png");
            this.imageList2.Images.SetKeyName(15, "My-Photos-Folder-32.png");
            this.imageList2.Images.SetKeyName(16, "Product-documentation-32.png");
            this.imageList2.Images.SetKeyName(17, "tausdata1.png");
            // 
            // button_create_dqf_project
            // 
            this.button_create_dqf_project.Location = new System.Drawing.Point(0, 0);
            this.button_create_dqf_project.Name = "button_create_dqf_project";
            this.button_create_dqf_project.Size = new System.Drawing.Size(75, 23);
            this.button_create_dqf_project.TabIndex = 0;
            // 
            // StudioTimeTrackerViewTrackChangesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel3);
            this.DoubleBuffered = true;
            this.Name = "StudioTimeTrackerViewTrackChangesControl";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(1293, 475);
            this.Load += new System.EventHandler(this.StudioTimeTrackerViewTrackChangesControl_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_segmentPerSecond)).EndInit();
            this.panel_segment_per_second_properties.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_words_per_minute)).EndInit();
            this.panel_words_per_min_control.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal ImageList imageList1;
        internal ToolStripMenuItem mergeProjectActivitiesToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        internal ToolStripMenuItem newProjectActivityToolStripMenuItem;
        internal ToolStripMenuItem editProjectActivityToolStripMenuItem;
        internal ToolStripMenuItem removeProjectActivityToolStripMenuItem;
        internal ToolStripSeparator toolStripSeparator1;
        internal ToolStripMenuItem duplicateTheProjectActivityToolStripMenuItem;
        internal ToolStripSeparator toolStripSeparator2;
        internal ToolStripMenuItem createAnActivitiesReportToolStripMenuItem;
        internal ToolStripMenuItem exportActivitiesToExcelToolStripMenuItem;
        private Panel panel3;
        internal WebBrowser webBrowser1;
        internal WebBrowser webBrowser2;
        private TabPage tabPage1;
        private Panel panel1;
        private TabPage tabPage2;
        private Panel panel2;
        public TreeView treeView_navigation;
        private Splitter splitter1;
        private Panel panel4;
        protected internal ElementHost elementHost1;
        protected internal ListViewControl listViewControl1;
        private ImageList imageList2;
        private TabPage tabPage3;
        private Panel panel5;
        private TabControl tabControl2;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Chart chart_words_per_minute;
        private Panel panel_words_per_min_control;
        private CheckBox checkBoxShowUnusual;
        private CheckBox checkBoxShowMedian;
        private CheckBox checkBoxShowAverage;
        private ComboBox comboBoxPercentiles;
        private Label label1;
        private Panel panel_segment_per_second_properties;
        private Label label5;
        private Label label6;
        private Label label3;
        private Label label4;
        private Label label2;
        private Label label7;
        private Chart chart_segmentPerSecond;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private ImageList imageList3;
        private TabPage tabPage6;
        internal WebBrowser webBrowser3;
        internal WebBrowser webBrowser4;
        private Button button_create_dqf_project;
        internal TabControl tabControl1;
        


    }
}