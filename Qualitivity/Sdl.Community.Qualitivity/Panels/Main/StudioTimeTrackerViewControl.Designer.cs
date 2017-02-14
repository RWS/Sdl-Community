using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Sdl.Community.Qualitivity.Panels.Main
{
    partial class QualitivityViewControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityViewControl));
            this.contextMenuStrip_listView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateTheProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeProjectActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.createAnActivitiesReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportActivitiesToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addDQFProjectTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn_client_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_project = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_source = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_target = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_description = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_status = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_billable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_documents = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_pem_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_hr_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_custom_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label_viewer_header = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.linkLabel_turn_off_groups = new System.Windows.Forms.LinkLabel();
            this.linkLabel_unselect_all = new System.Windows.Forms.LinkLabel();
            this.linkLabel_select_all = new System.Windows.Forms.LinkLabel();
            this.linkLabel_collapse_all_groups = new System.Windows.Forms.LinkLabel();
            this.linkLabel_expand_all_groups = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.label_TOTAL_PROJECTS = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_TOTAL_PROJECT_ACTIVITIES = new System.Windows.Forms.Label();
            this.contextMenuStrip_listView.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip_listView
            // 
            this.contextMenuStrip_listView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectActivityToolStripMenuItem,
            this.editProjectActivityToolStripMenuItem,
            this.removeProjectActivityToolStripMenuItem,
            this.toolStripSeparator1,
            this.duplicateTheProjectActivityToolStripMenuItem,
            this.mergeProjectActivitiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.createAnActivitiesReportToolStripMenuItem,
            this.exportActivitiesToExcelToolStripMenuItem,
            this.toolStripSeparator3,
            this.addDQFProjectTaskToolStripMenuItem});
            this.contextMenuStrip_listView.Name = "contextMenuStrip1";
            this.contextMenuStrip_listView.Size = new System.Drawing.Size(266, 198);
            // 
            // newProjectActivityToolStripMenuItem
            // 
            this.newProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectActivityToolStripMenuItem.Image")));
            this.newProjectActivityToolStripMenuItem.Name = "newProjectActivityToolStripMenuItem";
            this.newProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.newProjectActivityToolStripMenuItem.Text = "New Project Activity";
            // 
            // editProjectActivityToolStripMenuItem
            // 
            this.editProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editProjectActivityToolStripMenuItem.Image")));
            this.editProjectActivityToolStripMenuItem.Name = "editProjectActivityToolStripMenuItem";
            this.editProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.editProjectActivityToolStripMenuItem.Text = "Edit Project Activity";
            // 
            // removeProjectActivityToolStripMenuItem
            // 
            this.removeProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeProjectActivityToolStripMenuItem.Image")));
            this.removeProjectActivityToolStripMenuItem.Name = "removeProjectActivityToolStripMenuItem";
            this.removeProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.removeProjectActivityToolStripMenuItem.Text = "Remove Project Activity";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(262, 6);
            // 
            // duplicateTheProjectActivityToolStripMenuItem
            // 
            this.duplicateTheProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("duplicateTheProjectActivityToolStripMenuItem.Image")));
            this.duplicateTheProjectActivityToolStripMenuItem.Name = "duplicateTheProjectActivityToolStripMenuItem";
            this.duplicateTheProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.duplicateTheProjectActivityToolStripMenuItem.Text = "Create a Copy of the Project Activity";
            // 
            // mergeProjectActivitiesToolStripMenuItem
            // 
            this.mergeProjectActivitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mergeProjectActivitiesToolStripMenuItem.Image")));
            this.mergeProjectActivitiesToolStripMenuItem.Name = "mergeProjectActivitiesToolStripMenuItem";
            this.mergeProjectActivitiesToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.mergeProjectActivitiesToolStripMenuItem.Text = "Merge Project Activities";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(262, 6);
            // 
            // createAnActivitiesReportToolStripMenuItem
            // 
            this.createAnActivitiesReportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createAnActivitiesReportToolStripMenuItem.Image")));
            this.createAnActivitiesReportToolStripMenuItem.Name = "createAnActivitiesReportToolStripMenuItem";
            this.createAnActivitiesReportToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.createAnActivitiesReportToolStripMenuItem.Text = "Create an Activities Report";
            // 
            // exportActivitiesToExcelToolStripMenuItem
            // 
            this.exportActivitiesToExcelToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportActivitiesToExcelToolStripMenuItem.Image")));
            this.exportActivitiesToExcelToolStripMenuItem.Name = "exportActivitiesToExcelToolStripMenuItem";
            this.exportActivitiesToExcelToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.exportActivitiesToExcelToolStripMenuItem.Text = "Export Activities";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(262, 6);
            // 
            // addDQFProjectTaskToolStripMenuItem
            // 
            this.addDQFProjectTaskToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addDQFProjectTaskToolStripMenuItem.Image")));
            this.addDQFProjectTaskToolStripMenuItem.Name = "addDQFProjectTaskToolStripMenuItem";
            this.addDQFProjectTaskToolStripMenuItem.Size = new System.Drawing.Size(265, 22);
            this.addDQFProjectTaskToolStripMenuItem.Text = "New DQF Project Task";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "calendar");
            this.imageList1.Images.SetKeyName(1, "client");
            this.imageList1.Images.SetKeyName(2, "xno1");
            this.imageList1.Images.SetKeyName(3, "xno2");
            this.imageList1.Images.SetKeyName(4, "iyes");
            this.imageList1.Images.SetKeyName(5, "ino");
            this.imageList1.Images.SetKeyName(6, "vno");
            this.imageList1.Images.SetKeyName(7, "vyes");
            this.imageList1.Images.SetKeyName(8, "tick");
            this.imageList1.Images.SetKeyName(9, "flag_red");
            this.imageList1.Images.SetKeyName(10, "flag_blue");
            this.imageList1.Images.SetKeyName(11, "flag_green");
            this.imageList1.Images.SetKeyName(12, "started");
            this.imageList1.Images.SetKeyName(13, "stopped");
            this.imageList1.Images.SetKeyName(14, "question_blue");
            this.imageList1.Images.SetKeyName(15, "hours");
            this.imageList1.Images.SetKeyName(16, "words");
            this.imageList1.Images.SetKeyName(17, "af-ZA.gif");
            this.imageList1.Images.SetKeyName(18, "ar.gif");
            this.imageList1.Images.SetKeyName(19, "ar-AE.gif");
            this.imageList1.Images.SetKeyName(20, "ar-BH.gif");
            this.imageList1.Images.SetKeyName(21, "ar-DZ.gif");
            this.imageList1.Images.SetKeyName(22, "ar-EG.gif");
            this.imageList1.Images.SetKeyName(23, "ar-IQ.gif");
            this.imageList1.Images.SetKeyName(24, "ar-JO.gif");
            this.imageList1.Images.SetKeyName(25, "ar-KW.gif");
            this.imageList1.Images.SetKeyName(26, "ar-LB.gif");
            this.imageList1.Images.SetKeyName(27, "ar-LY.gif");
            this.imageList1.Images.SetKeyName(28, "ar-MA.gif");
            this.imageList1.Images.SetKeyName(29, "ar-OM.gif");
            this.imageList1.Images.SetKeyName(30, "ar-QA.gif");
            this.imageList1.Images.SetKeyName(31, "ar-SA.gif");
            this.imageList1.Images.SetKeyName(32, "ar-SY.gif");
            this.imageList1.Images.SetKeyName(33, "ar-TN.gif");
            this.imageList1.Images.SetKeyName(34, "ar-YE.gif");
            this.imageList1.Images.SetKeyName(35, "be.gif");
            this.imageList1.Images.SetKeyName(36, "be-BY.gif");
            this.imageList1.Images.SetKeyName(37, "bg.gif");
            this.imageList1.Images.SetKeyName(38, "bg-BG.gif");
            this.imageList1.Images.SetKeyName(39, "bp.gif");
            this.imageList1.Images.SetKeyName(40, "bs.gif");
            this.imageList1.Images.SetKeyName(41, "bs-Cyrl-BA.gif");
            this.imageList1.Images.SetKeyName(42, "bs-Latn-BA.gif");
            this.imageList1.Images.SetKeyName(43, "ca.gif");
            this.imageList1.Images.SetKeyName(44, "ca-ES.gif");
            this.imageList1.Images.SetKeyName(45, "cf.gif");
            this.imageList1.Images.SetKeyName(46, "ch.gif");
            this.imageList1.Images.SetKeyName(47, "cs.gif");
            this.imageList1.Images.SetKeyName(48, "cs-CZ.gif");
            this.imageList1.Images.SetKeyName(49, "da.gif");
            this.imageList1.Images.SetKeyName(50, "da-DK.gif");
            this.imageList1.Images.SetKeyName(51, "de.gif");
            this.imageList1.Images.SetKeyName(52, "de-AT.gif");
            this.imageList1.Images.SetKeyName(53, "de-CH.gif");
            this.imageList1.Images.SetKeyName(54, "de-DE.gif");
            this.imageList1.Images.SetKeyName(55, "de-LI.gif");
            this.imageList1.Images.SetKeyName(56, "de-LU.gif");
            this.imageList1.Images.SetKeyName(57, "el.gif");
            this.imageList1.Images.SetKeyName(58, "el-GR.gif");
            this.imageList1.Images.SetKeyName(59, "empty.png");
            this.imageList1.Images.SetKeyName(60, "en.gif");
            this.imageList1.Images.SetKeyName(61, "en-AU.gif");
            this.imageList1.Images.SetKeyName(62, "en-BZ.gif");
            this.imageList1.Images.SetKeyName(63, "en-CA.gif");
            this.imageList1.Images.SetKeyName(64, "en-GB.gif");
            this.imageList1.Images.SetKeyName(65, "en-IE.gif");
            this.imageList1.Images.SetKeyName(66, "en-JM.gif");
            this.imageList1.Images.SetKeyName(67, "en-NZ.gif");
            this.imageList1.Images.SetKeyName(68, "en-PH.gif");
            this.imageList1.Images.SetKeyName(69, "en-TT.gif");
            this.imageList1.Images.SetKeyName(70, "en-US.gif");
            this.imageList1.Images.SetKeyName(71, "en-ZW.gif");
            this.imageList1.Images.SetKeyName(72, "es.gif");
            this.imageList1.Images.SetKeyName(73, "es-AR.gif");
            this.imageList1.Images.SetKeyName(74, "es-BO.gif");
            this.imageList1.Images.SetKeyName(75, "es-CL.gif");
            this.imageList1.Images.SetKeyName(76, "es-CO.gif");
            this.imageList1.Images.SetKeyName(77, "es-CR.gif");
            this.imageList1.Images.SetKeyName(78, "es-DO.gif");
            this.imageList1.Images.SetKeyName(79, "es-EC.gif");
            this.imageList1.Images.SetKeyName(80, "es-ES.gif");
            this.imageList1.Images.SetKeyName(81, "es-GT.gif");
            this.imageList1.Images.SetKeyName(82, "es-HN.gif");
            this.imageList1.Images.SetKeyName(83, "es-MX.gif");
            this.imageList1.Images.SetKeyName(84, "es-NI.gif");
            this.imageList1.Images.SetKeyName(85, "es-PA.gif");
            this.imageList1.Images.SetKeyName(86, "es-PE.gif");
            this.imageList1.Images.SetKeyName(87, "es-PR.gif");
            this.imageList1.Images.SetKeyName(88, "es-PY.gif");
            this.imageList1.Images.SetKeyName(89, "es-SV.gif");
            this.imageList1.Images.SetKeyName(90, "es-UY.gif");
            this.imageList1.Images.SetKeyName(91, "es-VE.gif");
            this.imageList1.Images.SetKeyName(92, "et.gif");
            this.imageList1.Images.SetKeyName(93, "et-EE.gif");
            this.imageList1.Images.SetKeyName(94, "eu.gif");
            this.imageList1.Images.SetKeyName(95, "eu-ES.gif");
            this.imageList1.Images.SetKeyName(96, "fa.gif");
            this.imageList1.Images.SetKeyName(97, "fa-IR.gif");
            this.imageList1.Images.SetKeyName(98, "fi.gif");
            this.imageList1.Images.SetKeyName(99, "fi-FI.gif");
            this.imageList1.Images.SetKeyName(100, "fl.gif");
            this.imageList1.Images.SetKeyName(101, "fr.gif");
            this.imageList1.Images.SetKeyName(102, "fr-BE.gif");
            this.imageList1.Images.SetKeyName(103, "fr-CA.gif");
            this.imageList1.Images.SetKeyName(104, "fr-CH.gif");
            this.imageList1.Images.SetKeyName(105, "fr-FR.gif");
            this.imageList1.Images.SetKeyName(106, "fr-LU.gif");
            this.imageList1.Images.SetKeyName(107, "ga.gif");
            this.imageList1.Images.SetKeyName(108, "gb.gif");
            this.imageList1.Images.SetKeyName(109, "he.gif");
            this.imageList1.Images.SetKeyName(110, "he-IL.gif");
            this.imageList1.Images.SetKeyName(111, "hi.gif");
            this.imageList1.Images.SetKeyName(112, "hi-IN.gif");
            this.imageList1.Images.SetKeyName(113, "hr.gif");
            this.imageList1.Images.SetKeyName(114, "hr-HR.gif");
            this.imageList1.Images.SetKeyName(115, "hu.gif");
            this.imageList1.Images.SetKeyName(116, "hu-HU.gif");
            this.imageList1.Images.SetKeyName(117, "id-ID.gif");
            this.imageList1.Images.SetKeyName(118, "in.gif");
            this.imageList1.Images.SetKeyName(119, "is.gif");
            this.imageList1.Images.SetKeyName(120, "is-IS.gif");
            this.imageList1.Images.SetKeyName(121, "it.gif");
            this.imageList1.Images.SetKeyName(122, "it-CH.gif");
            this.imageList1.Images.SetKeyName(123, "it-IT.gif");
            this.imageList1.Images.SetKeyName(124, "ja.gif");
            this.imageList1.Images.SetKeyName(125, "ja-JP.gif");
            this.imageList1.Images.SetKeyName(126, "kk.gif");
            this.imageList1.Images.SetKeyName(127, "kk-KZ.gif");
            this.imageList1.Images.SetKeyName(128, "ko.gif");
            this.imageList1.Images.SetKeyName(129, "ko-KR.gif");
            this.imageList1.Images.SetKeyName(130, "ls.gif");
            this.imageList1.Images.SetKeyName(131, "lt.gif");
            this.imageList1.Images.SetKeyName(132, "lt-LT.gif");
            this.imageList1.Images.SetKeyName(133, "lv.gif");
            this.imageList1.Images.SetKeyName(134, "lv-LV.gif");
            this.imageList1.Images.SetKeyName(135, "lx.gif");
            this.imageList1.Images.SetKeyName(136, "mk.gif");
            this.imageList1.Images.SetKeyName(137, "mk-MK.gif");
            this.imageList1.Images.SetKeyName(138, "ms.gif");
            this.imageList1.Images.SetKeyName(139, "ms-MY.gif");
            this.imageList1.Images.SetKeyName(140, "nb-NO.gif");
            this.imageList1.Images.SetKeyName(141, "nl.gif");
            this.imageList1.Images.SetKeyName(142, "nl-BE.gif");
            this.imageList1.Images.SetKeyName(143, "nl-NL.gif");
            this.imageList1.Images.SetKeyName(144, "nn-NO.gif");
            this.imageList1.Images.SetKeyName(145, "no.gif");
            this.imageList1.Images.SetKeyName(146, "pl.gif");
            this.imageList1.Images.SetKeyName(147, "pl-PL.gif");
            this.imageList1.Images.SetKeyName(148, "pt.gif");
            this.imageList1.Images.SetKeyName(149, "pt-BR.gif");
            this.imageList1.Images.SetKeyName(150, "pt-PT.gif");
            this.imageList1.Images.SetKeyName(151, "ro.gif");
            this.imageList1.Images.SetKeyName(152, "ro-RO.gif");
            this.imageList1.Images.SetKeyName(153, "ru.gif");
            this.imageList1.Images.SetKeyName(154, "ru-RU.gif");
            this.imageList1.Images.SetKeyName(155, "sd.gif");
            this.imageList1.Images.SetKeyName(156, "sf.gif");
            this.imageList1.Images.SetKeyName(157, "si.gif");
            this.imageList1.Images.SetKeyName(158, "sk.gif");
            this.imageList1.Images.SetKeyName(159, "sk-SK.gif");
            this.imageList1.Images.SetKeyName(160, "sl.gif");
            this.imageList1.Images.SetKeyName(161, "sl-SI.gif");
            this.imageList1.Images.SetKeyName(162, "sp.gif");
            this.imageList1.Images.SetKeyName(163, "sq.gif");
            this.imageList1.Images.SetKeyName(164, "sq-AL.gif");
            this.imageList1.Images.SetKeyName(165, "sr.gif");
            this.imageList1.Images.SetKeyName(166, "sr-Cyrl-CS.gif");
            this.imageList1.Images.SetKeyName(167, "sr-Latn-CS.gif");
            this.imageList1.Images.SetKeyName(168, "sv.gif");
            this.imageList1.Images.SetKeyName(169, "sv-FI.gif");
            this.imageList1.Images.SetKeyName(170, "sv-SE.gif");
            this.imageList1.Images.SetKeyName(171, "syr-SY.gif");
            this.imageList1.Images.SetKeyName(172, "th.gif");
            this.imageList1.Images.SetKeyName(173, "th-TH.gif");
            this.imageList1.Images.SetKeyName(174, "tr.gif");
            this.imageList1.Images.SetKeyName(175, "tr-TR.gif");
            this.imageList1.Images.SetKeyName(176, "uk.gif");
            this.imageList1.Images.SetKeyName(177, "uk-UA.gif");
            this.imageList1.Images.SetKeyName(178, "ur-PK.gif");
            this.imageList1.Images.SetKeyName(179, "uz-Cyrl-UZ.gif");
            this.imageList1.Images.SetKeyName(180, "uz-Latn-UZ.gif");
            this.imageList1.Images.SetKeyName(181, "vi.gif");
            this.imageList1.Images.SetKeyName(182, "vi-VN.gif");
            this.imageList1.Images.SetKeyName(183, "xh-ZA.gif");
            this.imageList1.Images.SetKeyName(184, "zh.gif");
            this.imageList1.Images.SetKeyName(185, "zh-CN.gif");
            this.imageList1.Images.SetKeyName(186, "zh-HK.gif");
            this.imageList1.Images.SetKeyName(187, "zh-MO.gif");
            this.imageList1.Images.SetKeyName(188, "zh-SG.gif");
            this.imageList1.Images.SetKeyName(189, "zh-TW.gif");
            this.imageList1.Images.SetKeyName(190, "zt.gif");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label_viewer_header);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1369, 395);
            this.panel1.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.objectListView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 23);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1369, 351);
            this.panel3.TabIndex = 2;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn_client_name);
            this.objectListView1.AllColumns.Add(this.olvColumn_project);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_name);
            this.objectListView1.AllColumns.Add(this.olvColumn_source);
            this.objectListView1.AllColumns.Add(this.olvColumn_target);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_description);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_status);
            this.objectListView1.AllColumns.Add(this.olvColumn_billable);
            this.objectListView1.AllColumns.Add(this.olvColumn5);
            this.objectListView1.AllColumns.Add(this.olvColumn6);
            this.objectListView1.AllColumns.Add(this.olvColumn_documents);
            this.objectListView1.AllColumns.Add(this.olvColumn_pem_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_hr_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_custom_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_total);
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn_client_name,
            this.olvColumn_project,
            this.olvColumn_activity_name,
            this.olvColumn_source,
            this.olvColumn_target,
            this.olvColumn_activity_description,
            this.olvColumn_activity_status,
            this.olvColumn_billable,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn_documents,
            this.olvColumn_pem_total,
            this.olvColumn_hr_total,
            this.olvColumn_custom_total,
            this.olvColumn_activity_total});
            this.objectListView1.ContextMenuStrip = this.contextMenuStrip_listView;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.EmptyListMsg = "Empty!";
            this.objectListView1.EmptyListMsgFont = new System.Drawing.Font("Palatino Linotype", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HeaderUsesThemes = false;
            this.objectListView1.HeaderWordWrap = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.IncludeColumnHeadersInCopy = true;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.OwnerDraw = true;
            this.objectListView1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.ShowHeaderInAllViews = false;
            this.objectListView1.ShowItemCountOnGroups = true;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(1369, 351);
            this.objectListView1.SmallImageList = this.imageList1;
            this.objectListView1.SpaceBetweenGroups = 20;
            this.objectListView1.TabIndex = 4;
            this.objectListView1.UseAlternatingBackColors = true;
            this.objectListView1.UseCellFormatEvents = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFilterIndicator = true;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn_client_name
            // 
            this.olvColumn_client_name.AspectName = "ClientName";
            this.olvColumn_client_name.CellPadding = null;
            this.olvColumn_client_name.Text = "Client";
            this.olvColumn_client_name.Width = 100;
            // 
            // olvColumn_project
            // 
            this.olvColumn_project.AspectName = "ProjectName";
            this.olvColumn_project.CellPadding = null;
            this.olvColumn_project.Text = "Project";
            this.olvColumn_project.Width = 110;
            // 
            // olvColumn_activity_name
            // 
            this.olvColumn_activity_name.AspectName = "Name";
            this.olvColumn_activity_name.CellPadding = null;
            this.olvColumn_activity_name.HeaderImageKey = "(none)";
            this.olvColumn_activity_name.ImageAspectName = "";
            this.olvColumn_activity_name.Text = "Activity Name";
            this.olvColumn_activity_name.Width = 180;
            // 
            // olvColumn_source
            // 
            this.olvColumn_source.CellPadding = null;
            this.olvColumn_source.Text = "Source";
            // 
            // olvColumn_target
            // 
            this.olvColumn_target.CellPadding = null;
            this.olvColumn_target.Text = "Target";
            // 
            // olvColumn_activity_description
            // 
            this.olvColumn_activity_description.AspectName = "ActivityDescription";
            this.olvColumn_activity_description.CellPadding = null;
            this.olvColumn_activity_description.Text = "Description";
            this.olvColumn_activity_description.Width = 80;
            // 
            // olvColumn_activity_status
            // 
            this.olvColumn_activity_status.AspectName = "ActivityStatus";
            this.olvColumn_activity_status.CellPadding = null;
            this.olvColumn_activity_status.Text = "Status";
            this.olvColumn_activity_status.Width = 78;
            // 
            // olvColumn_billable
            // 
            this.olvColumn_billable.AspectName = "Billable";
            this.olvColumn_billable.CellPadding = null;
            this.olvColumn_billable.Text = "Billable";
            this.olvColumn_billable.Width = 75;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Started";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.HeaderImageKey = "started";
            this.olvColumn5.Text = "From";
            this.olvColumn5.Width = 110;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Stopped";
            this.olvColumn6.CellPadding = null;
            this.olvColumn6.HeaderImageKey = "started";
            this.olvColumn6.Text = "To";
            this.olvColumn6.Width = 110;
            // 
            // olvColumn_documents
            // 
            this.olvColumn_documents.CellPadding = null;
            this.olvColumn_documents.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Text = "Documents";
            this.olvColumn_documents.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Width = 70;
            // 
            // olvColumn_pem_total
            // 
            this.olvColumn_pem_total.AspectName = "";
            this.olvColumn_pem_total.CellPadding = null;
            this.olvColumn_pem_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Text = "Language Rate Total";
            this.olvColumn_pem_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Width = 125;
            // 
            // olvColumn_hr_total
            // 
            this.olvColumn_hr_total.AspectName = "";
            this.olvColumn_hr_total.CellPadding = null;
            this.olvColumn_hr_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Text = "Hourly Rate Total";
            this.olvColumn_hr_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Width = 100;
            // 
            // olvColumn_custom_total
            // 
            this.olvColumn_custom_total.CellPadding = null;
            this.olvColumn_custom_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_custom_total.Text = "Custom Rate Total";
            this.olvColumn_custom_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_custom_total.Width = 125;
            // 
            // olvColumn_activity_total
            // 
            this.olvColumn_activity_total.CellPadding = null;
            this.olvColumn_activity_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Text = "Activity Total";
            this.olvColumn_activity_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Width = 90;
            // 
            // label_viewer_header
            // 
            this.label_viewer_header.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_viewer_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label_viewer_header.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_viewer_header.Location = new System.Drawing.Point(0, 0);
            this.label_viewer_header.Name = "label_viewer_header";
            this.label_viewer_header.Size = new System.Drawing.Size(1369, 23);
            this.label_viewer_header.TabIndex = 0;
            this.label_viewer_header.Text = "label_viewer_header";
            this.label_viewer_header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.linkLabel_turn_off_groups);
            this.panel2.Controls.Add(this.linkLabel_unselect_all);
            this.panel2.Controls.Add(this.linkLabel_select_all);
            this.panel2.Controls.Add(this.linkLabel_collapse_all_groups);
            this.panel2.Controls.Add(this.linkLabel_expand_all_groups);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label_TOTAL_PROJECTS);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label_TOTAL_PROJECT_ACTIVITIES);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 374);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.panel2.Size = new System.Drawing.Size(1369, 21);
            this.panel2.TabIndex = 1;
            // 
            // linkLabel_turn_off_groups
            // 
            this.linkLabel_turn_off_groups.AutoSize = true;
            this.linkLabel_turn_off_groups.Location = new System.Drawing.Point(326, 3);
            this.linkLabel_turn_off_groups.Name = "linkLabel_turn_off_groups";
            this.linkLabel_turn_off_groups.Size = new System.Drawing.Size(79, 13);
            this.linkLabel_turn_off_groups.TabIndex = 10;
            this.linkLabel_turn_off_groups.TabStop = true;
            this.linkLabel_turn_off_groups.Text = "Turn off groups";
            this.linkLabel_turn_off_groups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_turn_off_groups_LinkClicked);
            // 
            // linkLabel_unselect_all
            // 
            this.linkLabel_unselect_all.AutoSize = true;
            this.linkLabel_unselect_all.Location = new System.Drawing.Point(229, 3);
            this.linkLabel_unselect_all.Name = "linkLabel_unselect_all";
            this.linkLabel_unselect_all.Size = new System.Drawing.Size(63, 13);
            this.linkLabel_unselect_all.TabIndex = 9;
            this.linkLabel_unselect_all.TabStop = true;
            this.linkLabel_unselect_all.Text = "Unselect All";
            this.linkLabel_unselect_all.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_unselect_all_LinkClicked);
            // 
            // linkLabel_select_all
            // 
            this.linkLabel_select_all.AutoSize = true;
            this.linkLabel_select_all.Location = new System.Drawing.Point(174, 3);
            this.linkLabel_select_all.Name = "linkLabel_select_all";
            this.linkLabel_select_all.Size = new System.Drawing.Size(51, 13);
            this.linkLabel_select_all.TabIndex = 8;
            this.linkLabel_select_all.TabStop = true;
            this.linkLabel_select_all.Text = "Select All";
            this.linkLabel_select_all.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_select_all_LinkClicked);
            // 
            // linkLabel_collapse_all_groups
            // 
            this.linkLabel_collapse_all_groups.AutoSize = true;
            this.linkLabel_collapse_all_groups.Location = new System.Drawing.Point(68, 3);
            this.linkLabel_collapse_all_groups.Name = "linkLabel_collapse_all_groups";
            this.linkLabel_collapse_all_groups.Size = new System.Drawing.Size(61, 13);
            this.linkLabel_collapse_all_groups.TabIndex = 7;
            this.linkLabel_collapse_all_groups.TabStop = true;
            this.linkLabel_collapse_all_groups.Text = "Collapse All";
            this.linkLabel_collapse_all_groups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_collapse_all_groups_LinkClicked);
            // 
            // linkLabel_expand_all_groups
            // 
            this.linkLabel_expand_all_groups.AutoSize = true;
            this.linkLabel_expand_all_groups.Location = new System.Drawing.Point(5, 3);
            this.linkLabel_expand_all_groups.Name = "linkLabel_expand_all_groups";
            this.linkLabel_expand_all_groups.Size = new System.Drawing.Size(57, 13);
            this.linkLabel_expand_all_groups.TabIndex = 7;
            this.linkLabel_expand_all_groups.TabStop = true;
            this.linkLabel_expand_all_groups.Text = "Expand All";
            this.linkLabel_expand_all_groups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_expand_all_groups_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Right;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(952, 4);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Filtered Projects:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_TOTAL_PROJECTS
            // 
            this.label_TOTAL_PROJECTS.AutoSize = true;
            this.label_TOTAL_PROJECTS.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_TOTAL_PROJECTS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_TOTAL_PROJECTS.Location = new System.Drawing.Point(1037, 4);
            this.label_TOTAL_PROJECTS.Margin = new System.Windows.Forms.Padding(0);
            this.label_TOTAL_PROJECTS.Name = "label_TOTAL_PROJECTS";
            this.label_TOTAL_PROJECTS.Size = new System.Drawing.Size(110, 13);
            this.label_TOTAL_PROJECTS.TabIndex = 5;
            this.label_TOTAL_PROJECTS.Text = "[TOTAL_PROJECTS]";
            this.label_TOTAL_PROJECTS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(1147, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Activities:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_TOTAL_PROJECT_ACTIVITIES
            // 
            this.label_TOTAL_PROJECT_ACTIVITIES.AutoSize = true;
            this.label_TOTAL_PROJECT_ACTIVITIES.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_TOTAL_PROJECT_ACTIVITIES.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_TOTAL_PROJECT_ACTIVITIES.Location = new System.Drawing.Point(1199, 4);
            this.label_TOTAL_PROJECT_ACTIVITIES.Margin = new System.Windows.Forms.Padding(0);
            this.label_TOTAL_PROJECT_ACTIVITIES.Name = "label_TOTAL_PROJECT_ACTIVITIES";
            this.label_TOTAL_PROJECT_ACTIVITIES.Size = new System.Drawing.Size(167, 13);
            this.label_TOTAL_PROJECT_ACTIVITIES.TabIndex = 1;
            this.label_TOTAL_PROJECT_ACTIVITIES.Text = "[TOTAL_PROJECT_ACTIVITIES]";
            this.label_TOTAL_PROJECT_ACTIVITIES.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // QualitivityViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "QualitivityViewControl";
            this.Size = new System.Drawing.Size(1369, 395);
            this.contextMenuStrip_listView.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal ToolStripMenuItem newProjectActivityToolStripMenuItem;
        internal ToolStripMenuItem editProjectActivityToolStripMenuItem;
        internal ToolStripMenuItem removeProjectActivityToolStripMenuItem;
        internal ToolStripSeparator toolStripSeparator1;
        internal ToolStripMenuItem duplicateTheProjectActivityToolStripMenuItem;
        internal ToolStripSeparator toolStripSeparator2;
        internal ToolStripMenuItem createAnActivitiesReportToolStripMenuItem;
        internal ToolStripMenuItem exportActivitiesToExcelToolStripMenuItem;
        internal ToolStripMenuItem mergeProjectActivitiesToolStripMenuItem;
        internal ImageList imageList1;
        private Panel panel1;
        private Panel panel3;
        internal ObjectListView objectListView1;
        internal OLVColumn olvColumn_client_name;
        private OLVColumn olvColumn_project;
        internal OLVColumn olvColumn_activity_name;
        internal OLVColumn olvColumn_activity_description;
        private OLVColumn olvColumn_activity_status;
        private OLVColumn olvColumn_billable;
        private OLVColumn olvColumn5;
        private OLVColumn olvColumn6;
        private OLVColumn olvColumn_documents;
        private OLVColumn olvColumn_pem_total;
        private OLVColumn olvColumn_hr_total;
        private OLVColumn olvColumn_activity_total;
        internal Label label_viewer_header;
        internal Panel panel2;
        internal LinkLabel linkLabel_turn_off_groups;
        internal LinkLabel linkLabel_unselect_all;
        internal LinkLabel linkLabel_select_all;
        internal LinkLabel linkLabel_collapse_all_groups;
        internal LinkLabel linkLabel_expand_all_groups;
        private Label label7;
        internal Label label_TOTAL_PROJECTS;
        private Label label5;
        internal Label label_TOTAL_PROJECT_ACTIVITIES;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripMenuItem addDQFProjectTaskToolStripMenuItem;
        public ContextMenuStrip contextMenuStrip_listView;
        private OLVColumn olvColumn_source;
        private OLVColumn olvColumn_target;
        private OLVColumn olvColumn_custom_total;


    }
}