namespace Sdl.Community.NumberVerifier
{
    partial class NumberVerifierUI
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
            this.components = new System.ComponentModel.Container();
            this.panelNumberVerifierUI = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.rb_ReportExtendedMessages = new System.Windows.Forms.RadioButton();
            this.rb_ReportBriefMessages = new System.Windows.Forms.RadioButton();
            this.combo_ModifiedAlphanumericsErrorType = new System.Windows.Forms.ComboBox();
            this.cb_ReportModifiedAlphanumerics = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.untranslatedCheck = new System.Windows.Forms.CheckBox();
            this.cb_Exclude100Percents = new System.Windows.Forms.CheckBox();
            this.cb_ExcludeLockedSegments = new System.Windows.Forms.CheckBox();
            this.cb_ExcludeTagText = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rb_PreventLocalizations = new System.Windows.Forms.RadioButton();
            this.rb_RequireLocalizations = new System.Windows.Forms.RadioButton();
            this.rb_AllowLocalizations = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.customTBox = new System.Windows.Forms.TextBox();
            this.targetTbox = new System.Windows.Forms.CheckBox();
            this.cb_TargetNoSeparator = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsPeriod = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsComma = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsNobreakThinSpace = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsThinSpace = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsNobreakSpace = new System.Windows.Forms.CheckBox();
            this.cb_TargetThousandsSpace = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.targetDBox = new System.Windows.Forms.TextBox();
            this.customTargetSep = new System.Windows.Forms.CheckBox();
            this.cb_TargetDecimalPeriod = new System.Windows.Forms.CheckBox();
            this.cb_TargetDecimalComma = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.sourceDBox = new System.Windows.Forms.TextBox();
            this.customDSep = new System.Windows.Forms.CheckBox();
            this.cb_SourceDecimalPeriod = new System.Windows.Forms.CheckBox();
            this.cb_SourceDecimalComma = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sourceTBox = new System.Windows.Forms.TextBox();
            this.customTSep = new System.Windows.Forms.CheckBox();
            this.cb_SourceNoSeparator = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsPeriod = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsComma = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsNobreakThinSpace = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsThinSpace = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsNobreakSpace = new System.Windows.Forms.CheckBox();
            this.cb_SourceThousandsSpace = new System.Windows.Forms.CheckBox();
            this.combo_ModifiedNumbersErrorType = new System.Windows.Forms.ComboBox();
            this.combo_RemovedNumbersErrorType = new System.Windows.Forms.ComboBox();
            this.combo_AddedNumbersErrorType = new System.Windows.Forms.ComboBox();
            this.cb_ReportModifiedNumbers = new System.Windows.Forms.CheckBox();
            this.cb_ReportRemovedNumbers = new System.Windows.Forms.CheckBox();
            this.cb_ReportAddedNumbers = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.draftCheck = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panelNumberVerifierUI.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelNumberVerifierUI
            // 
            this.panelNumberVerifierUI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNumberVerifierUI.AutoScroll = true;
            this.panelNumberVerifierUI.Controls.Add(this.groupBox1);
            this.panelNumberVerifierUI.Location = new System.Drawing.Point(0, 0);
            this.panelNumberVerifierUI.Name = "panelNumberVerifierUI";
            this.panelNumberVerifierUI.Size = new System.Drawing.Size(496, 642);
            this.panelNumberVerifierUI.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox8);
            this.groupBox1.Controls.Add(this.combo_ModifiedAlphanumericsErrorType);
            this.groupBox1.Controls.Add(this.cb_ReportModifiedAlphanumerics);
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.combo_ModifiedNumbersErrorType);
            this.groupBox1.Controls.Add(this.combo_RemovedNumbersErrorType);
            this.groupBox1.Controls.Add(this.combo_AddedNumbersErrorType);
            this.groupBox1.Controls.Add(this.cb_ReportModifiedNumbers);
            this.groupBox1.Controls.Add(this.cb_ReportRemovedNumbers);
            this.groupBox1.Controls.Add(this.cb_ReportAddedNumbers);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 642);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Number Verifier";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.rb_ReportExtendedMessages);
            this.groupBox8.Controls.Add(this.rb_ReportBriefMessages);
            this.groupBox8.Location = new System.Drawing.Point(322, 25);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(93, 98);
            this.groupBox8.TabIndex = 37;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Messages";
            // 
            // rb_ReportExtendedMessages
            // 
            this.rb_ReportExtendedMessages.AutoSize = true;
            this.rb_ReportExtendedMessages.Location = new System.Drawing.Point(14, 52);
            this.rb_ReportExtendedMessages.Name = "rb_ReportExtendedMessages";
            this.rb_ReportExtendedMessages.Size = new System.Drawing.Size(70, 17);
            this.rb_ReportExtendedMessages.TabIndex = 1;
            this.rb_ReportExtendedMessages.Text = "Extended";
            this.toolTip1.SetToolTip(this.rb_ReportExtendedMessages, "If this is selected, messages will \r\ninclude source and target text ");
            this.rb_ReportExtendedMessages.UseVisualStyleBackColor = true;
            // 
            // rb_ReportBriefMessages
            // 
            this.rb_ReportBriefMessages.AutoSize = true;
            this.rb_ReportBriefMessages.Checked = true;
            this.rb_ReportBriefMessages.Location = new System.Drawing.Point(14, 29);
            this.rb_ReportBriefMessages.Name = "rb_ReportBriefMessages";
            this.rb_ReportBriefMessages.Size = new System.Drawing.Size(46, 17);
            this.rb_ReportBriefMessages.TabIndex = 0;
            this.rb_ReportBriefMessages.TabStop = true;
            this.rb_ReportBriefMessages.Text = "Brief";
            this.rb_ReportBriefMessages.UseVisualStyleBackColor = true;
            // 
            // combo_ModifiedAlphanumericsErrorType
            // 
            this.combo_ModifiedAlphanumericsErrorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_ModifiedAlphanumericsErrorType.FormattingEnabled = true;
            this.combo_ModifiedAlphanumericsErrorType.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Note"});
            this.combo_ModifiedAlphanumericsErrorType.Location = new System.Drawing.Point(190, 104);
            this.combo_ModifiedAlphanumericsErrorType.Name = "combo_ModifiedAlphanumericsErrorType";
            this.combo_ModifiedAlphanumericsErrorType.Size = new System.Drawing.Size(81, 21);
            this.combo_ModifiedAlphanumericsErrorType.TabIndex = 10;
            // 
            // cb_ReportModifiedAlphanumerics
            // 
            this.cb_ReportModifiedAlphanumerics.AutoSize = true;
            this.cb_ReportModifiedAlphanumerics.Checked = true;
            this.cb_ReportModifiedAlphanumerics.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ReportModifiedAlphanumerics.Location = new System.Drawing.Point(16, 106);
            this.cb_ReportModifiedAlphanumerics.Name = "cb_ReportModifiedAlphanumerics";
            this.cb_ReportModifiedAlphanumerics.Size = new System.Drawing.Size(171, 17);
            this.cb_ReportModifiedAlphanumerics.TabIndex = 9;
            this.cb_ReportModifiedAlphanumerics.Text = "Report m&odified alphanumerics";
            this.toolTip1.SetToolTip(this.cb_ReportModifiedAlphanumerics, "Select this to find issues related to uppercase \r\nletters in alphanumeric names l" +
        "ike V50. ");
            this.cb_ReportModifiedAlphanumerics.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.draftCheck);
            this.groupBox7.Controls.Add(this.untranslatedCheck);
            this.groupBox7.Controls.Add(this.cb_Exclude100Percents);
            this.groupBox7.Controls.Add(this.cb_ExcludeLockedSegments);
            this.groupBox7.Controls.Add(this.cb_ExcludeTagText);
            this.groupBox7.Location = new System.Drawing.Point(16, 131);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(191, 134);
            this.groupBox7.TabIndex = 11;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "&Exclude";
            // 
            // untranslatedCheck
            // 
            this.untranslatedCheck.AutoSize = true;
            this.untranslatedCheck.Location = new System.Drawing.Point(18, 88);
            this.untranslatedCheck.Name = "untranslatedCheck";
            this.untranslatedCheck.Size = new System.Drawing.Size(173, 17);
            this.untranslatedCheck.TabIndex = 15;
            this.untranslatedCheck.Text = "Exclude untranslated segments";
            this.toolTip1.SetToolTip(this.untranslatedCheck, "If this is selected, untranslated segments will not be verified.");
            this.untranslatedCheck.UseVisualStyleBackColor = true;
            // 
            // cb_Exclude100Percents
            // 
            this.cb_Exclude100Percents.AutoSize = true;
            this.cb_Exclude100Percents.Location = new System.Drawing.Point(18, 42);
            this.cb_Exclude100Percents.Name = "cb_Exclude100Percents";
            this.cb_Exclude100Percents.Size = new System.Drawing.Size(136, 17);
            this.cb_Exclude100Percents.TabIndex = 13;
            this.cb_Exclude100Percents.Text = "Exclude 100% matches";
            this.toolTip1.SetToolTip(this.cb_Exclude100Percents, "If this is selected, 100% TM matches \r\nwill not be verified.");
            this.cb_Exclude100Percents.UseVisualStyleBackColor = true;
            // 
            // cb_ExcludeLockedSegments
            // 
            this.cb_ExcludeLockedSegments.AutoSize = true;
            this.cb_ExcludeLockedSegments.Location = new System.Drawing.Point(18, 19);
            this.cb_ExcludeLockedSegments.Name = "cb_ExcludeLockedSegments";
            this.cb_ExcludeLockedSegments.Size = new System.Drawing.Size(147, 17);
            this.cb_ExcludeLockedSegments.TabIndex = 12;
            this.cb_ExcludeLockedSegments.Text = "Exclude locked segments";
            this.toolTip1.SetToolTip(this.cb_ExcludeLockedSegments, "If this is selected, locked segments \r\nwill not be verified.");
            this.cb_ExcludeLockedSegments.UseVisualStyleBackColor = true;
            // 
            // cb_ExcludeTagText
            // 
            this.cb_ExcludeTagText.AutoSize = true;
            this.cb_ExcludeTagText.Location = new System.Drawing.Point(18, 65);
            this.cb_ExcludeTagText.Name = "cb_ExcludeTagText";
            this.cb_ExcludeTagText.Size = new System.Drawing.Size(102, 17);
            this.cb_ExcludeTagText.TabIndex = 14;
            this.cb_ExcludeTagText.Text = "Exclude tag text";
            this.toolTip1.SetToolTip(this.cb_ExcludeTagText, "If this is selected, numbers that are \r\npart of tags will not be verified.");
            this.cb_ExcludeTagText.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rb_PreventLocalizations);
            this.groupBox2.Controls.Add(this.rb_RequireLocalizations);
            this.groupBox2.Controls.Add(this.rb_AllowLocalizations);
            this.groupBox2.Location = new System.Drawing.Point(213, 131);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(202, 134);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&Localizations";
            // 
            // rb_PreventLocalizations
            // 
            this.rb_PreventLocalizations.AutoSize = true;
            this.rb_PreventLocalizations.Location = new System.Drawing.Point(20, 64);
            this.rb_PreventLocalizations.Name = "rb_PreventLocalizations";
            this.rb_PreventLocalizations.Size = new System.Drawing.Size(122, 17);
            this.rb_PreventLocalizations.TabIndex = 18;
            this.rb_PreventLocalizations.Text = "Prevent localizations";
            this.toolTip1.SetToolTip(this.rb_PreventLocalizations, "If this is selected, numbers where separators \r\nhave been changed will be reporte" +
        "d.");
            this.rb_PreventLocalizations.UseVisualStyleBackColor = true;
            this.rb_PreventLocalizations.CheckedChanged += new System.EventHandler(this.rb_PreventLocalizations_CheckedChanged);
            // 
            // rb_RequireLocalizations
            // 
            this.rb_RequireLocalizations.AutoSize = true;
            this.rb_RequireLocalizations.Location = new System.Drawing.Point(20, 19);
            this.rb_RequireLocalizations.Name = "rb_RequireLocalizations";
            this.rb_RequireLocalizations.Size = new System.Drawing.Size(122, 17);
            this.rb_RequireLocalizations.TabIndex = 16;
            this.rb_RequireLocalizations.Text = "Require localizations";
            this.toolTip1.SetToolTip(this.rb_RequireLocalizations, "If this is selected, numbers where separators \r\nhave not been changed will be rep" +
        "orted.");
            this.rb_RequireLocalizations.UseVisualStyleBackColor = true;
            this.rb_RequireLocalizations.CheckedChanged += new System.EventHandler(this.rb_RequireLocalizations_CheckedChanged);
            // 
            // rb_AllowLocalizations
            // 
            this.rb_AllowLocalizations.AutoSize = true;
            this.rb_AllowLocalizations.Checked = true;
            this.rb_AllowLocalizations.Location = new System.Drawing.Point(20, 42);
            this.rb_AllowLocalizations.Name = "rb_AllowLocalizations";
            this.rb_AllowLocalizations.Size = new System.Drawing.Size(110, 17);
            this.rb_AllowLocalizations.TabIndex = 17;
            this.rb_AllowLocalizations.TabStop = true;
            this.rb_AllowLocalizations.Text = "Allow localizations";
            this.rb_AllowLocalizations.UseVisualStyleBackColor = true;
            this.rb_AllowLocalizations.CheckedChanged += new System.EventHandler(this.rb_AllowLocalizations_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.customTBox);
            this.groupBox6.Controls.Add(this.targetTbox);
            this.groupBox6.Controls.Add(this.cb_TargetNoSeparator);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsPeriod);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsComma);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsNobreakThinSpace);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsThinSpace);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsNobreakSpace);
            this.groupBox6.Controls.Add(this.cb_TargetThousandsSpace);
            this.groupBox6.Location = new System.Drawing.Point(225, 273);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(190, 233);
            this.groupBox6.TabIndex = 26;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "&Target thousands separators";
            this.toolTip1.SetToolTip(this.groupBox6, "Select all separators that should be considered thousands separators\r\nwhen they a" +
        "ppear in a place which is appropriate for a thousands separator.");
            // 
            // customTBox
            // 
            this.customTBox.Location = new System.Drawing.Point(20, 204);
            this.customTBox.Name = "customTBox";
            this.customTBox.Size = new System.Drawing.Size(100, 20);
            this.customTBox.TabIndex = 34;
            // 
            // targetTbox
            // 
            this.targetTbox.AutoSize = true;
            this.targetTbox.Location = new System.Drawing.Point(20, 181);
            this.targetTbox.Name = "targetTbox";
            this.targetTbox.Size = new System.Drawing.Size(108, 17);
            this.targetTbox.TabIndex = 33;
            this.targetTbox.Text = "Custom separator";
            this.targetTbox.UseVisualStyleBackColor = true;
            // 
            // cb_TargetNoSeparator
            // 
            this.cb_TargetNoSeparator.AutoSize = true;
            this.cb_TargetNoSeparator.Checked = true;
            this.cb_TargetNoSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetNoSeparator.Location = new System.Drawing.Point(20, 157);
            this.cb_TargetNoSeparator.Name = "cb_TargetNoSeparator";
            this.cb_TargetNoSeparator.Size = new System.Drawing.Size(89, 17);
            this.cb_TargetNoSeparator.TabIndex = 27;
            this.cb_TargetNoSeparator.Text = "No Separator";
            this.cb_TargetNoSeparator.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsPeriod
            // 
            this.cb_TargetThousandsPeriod.AutoSize = true;
            this.cb_TargetThousandsPeriod.Checked = true;
            this.cb_TargetThousandsPeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsPeriod.Location = new System.Drawing.Point(20, 134);
            this.cb_TargetThousandsPeriod.Name = "cb_TargetThousandsPeriod";
            this.cb_TargetThousandsPeriod.Size = new System.Drawing.Size(56, 17);
            this.cb_TargetThousandsPeriod.TabIndex = 32;
            this.cb_TargetThousandsPeriod.Text = "Period";
            this.cb_TargetThousandsPeriod.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsComma
            // 
            this.cb_TargetThousandsComma.AutoSize = true;
            this.cb_TargetThousandsComma.Checked = true;
            this.cb_TargetThousandsComma.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsComma.Location = new System.Drawing.Point(20, 111);
            this.cb_TargetThousandsComma.Name = "cb_TargetThousandsComma";
            this.cb_TargetThousandsComma.Size = new System.Drawing.Size(61, 17);
            this.cb_TargetThousandsComma.TabIndex = 31;
            this.cb_TargetThousandsComma.Text = "Comma";
            this.cb_TargetThousandsComma.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsNobreakThinSpace
            // 
            this.cb_TargetThousandsNobreakThinSpace.AutoSize = true;
            this.cb_TargetThousandsNobreakThinSpace.Checked = true;
            this.cb_TargetThousandsNobreakThinSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsNobreakThinSpace.Location = new System.Drawing.Point(20, 88);
            this.cb_TargetThousandsNobreakThinSpace.Name = "cb_TargetThousandsNobreakThinSpace";
            this.cb_TargetThousandsNobreakThinSpace.Size = new System.Drawing.Size(137, 17);
            this.cb_TargetThousandsNobreakThinSpace.TabIndex = 30;
            this.cb_TargetThousandsNobreakThinSpace.Text = "Narrow no-break space";
            this.cb_TargetThousandsNobreakThinSpace.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsThinSpace
            // 
            this.cb_TargetThousandsThinSpace.AutoSize = true;
            this.cb_TargetThousandsThinSpace.Checked = true;
            this.cb_TargetThousandsThinSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsThinSpace.Location = new System.Drawing.Point(20, 65);
            this.cb_TargetThousandsThinSpace.Name = "cb_TargetThousandsThinSpace";
            this.cb_TargetThousandsThinSpace.Size = new System.Drawing.Size(79, 17);
            this.cb_TargetThousandsThinSpace.TabIndex = 29;
            this.cb_TargetThousandsThinSpace.Text = "Thin space";
            this.cb_TargetThousandsThinSpace.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsNobreakSpace
            // 
            this.cb_TargetThousandsNobreakSpace.AutoSize = true;
            this.cb_TargetThousandsNobreakSpace.Checked = true;
            this.cb_TargetThousandsNobreakSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsNobreakSpace.Location = new System.Drawing.Point(20, 42);
            this.cb_TargetThousandsNobreakSpace.Name = "cb_TargetThousandsNobreakSpace";
            this.cb_TargetThousandsNobreakSpace.Size = new System.Drawing.Size(102, 17);
            this.cb_TargetThousandsNobreakSpace.TabIndex = 28;
            this.cb_TargetThousandsNobreakSpace.Text = "No-break space";
            this.cb_TargetThousandsNobreakSpace.UseVisualStyleBackColor = true;
            // 
            // cb_TargetThousandsSpace
            // 
            this.cb_TargetThousandsSpace.AutoSize = true;
            this.cb_TargetThousandsSpace.Checked = true;
            this.cb_TargetThousandsSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetThousandsSpace.Location = new System.Drawing.Point(20, 19);
            this.cb_TargetThousandsSpace.Name = "cb_TargetThousandsSpace";
            this.cb_TargetThousandsSpace.Size = new System.Drawing.Size(57, 17);
            this.cb_TargetThousandsSpace.TabIndex = 27;
            this.cb_TargetThousandsSpace.Text = "Space";
            this.cb_TargetThousandsSpace.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.targetDBox);
            this.groupBox5.Controls.Add(this.customTargetSep);
            this.groupBox5.Controls.Add(this.cb_TargetDecimalPeriod);
            this.groupBox5.Controls.Add(this.cb_TargetDecimalComma);
            this.groupBox5.Location = new System.Drawing.Point(225, 512);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(190, 114);
            this.groupBox5.TabIndex = 36;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Target &decimal separators";
            this.toolTip1.SetToolTip(this.groupBox5, "Select all separators that should be considered decimal separators\r\nwhen they app" +
        "ear in a place which is appropriate for a decimal separator.");
            // 
            // targetDBox
            // 
            this.targetDBox.Location = new System.Drawing.Point(19, 85);
            this.targetDBox.Name = "targetDBox";
            this.targetDBox.Size = new System.Drawing.Size(100, 20);
            this.targetDBox.TabIndex = 40;
            // 
            // customTargetSep
            // 
            this.customTargetSep.AutoSize = true;
            this.customTargetSep.Location = new System.Drawing.Point(19, 61);
            this.customTargetSep.Name = "customTargetSep";
            this.customTargetSep.Size = new System.Drawing.Size(108, 17);
            this.customTargetSep.TabIndex = 39;
            this.customTargetSep.Text = "Custom separator";
            this.customTargetSep.UseVisualStyleBackColor = true;
            // 
            // cb_TargetDecimalPeriod
            // 
            this.cb_TargetDecimalPeriod.AutoSize = true;
            this.cb_TargetDecimalPeriod.Checked = true;
            this.cb_TargetDecimalPeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetDecimalPeriod.Location = new System.Drawing.Point(20, 42);
            this.cb_TargetDecimalPeriod.Name = "cb_TargetDecimalPeriod";
            this.cb_TargetDecimalPeriod.Size = new System.Drawing.Size(56, 17);
            this.cb_TargetDecimalPeriod.TabIndex = 38;
            this.cb_TargetDecimalPeriod.Text = "Period";
            this.cb_TargetDecimalPeriod.UseVisualStyleBackColor = true;
            // 
            // cb_TargetDecimalComma
            // 
            this.cb_TargetDecimalComma.AutoSize = true;
            this.cb_TargetDecimalComma.Checked = true;
            this.cb_TargetDecimalComma.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_TargetDecimalComma.Location = new System.Drawing.Point(20, 19);
            this.cb_TargetDecimalComma.Name = "cb_TargetDecimalComma";
            this.cb_TargetDecimalComma.Size = new System.Drawing.Size(61, 17);
            this.cb_TargetDecimalComma.TabIndex = 37;
            this.cb_TargetDecimalComma.Text = "Comma";
            this.cb_TargetDecimalComma.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.sourceDBox);
            this.groupBox4.Controls.Add(this.customDSep);
            this.groupBox4.Controls.Add(this.cb_SourceDecimalPeriod);
            this.groupBox4.Controls.Add(this.cb_SourceDecimalComma);
            this.groupBox4.Location = new System.Drawing.Point(17, 512);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(190, 114);
            this.groupBox4.TabIndex = 33;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "S&ource decimal separators";
            this.toolTip1.SetToolTip(this.groupBox4, "Select all separators that should be considered decimal separators\r\nwhen they app" +
        "ear in a place which is appropriate for a decimal separator.");
            // 
            // sourceDBox
            // 
            this.sourceDBox.Location = new System.Drawing.Point(17, 85);
            this.sourceDBox.Name = "sourceDBox";
            this.sourceDBox.Size = new System.Drawing.Size(100, 20);
            this.sourceDBox.TabIndex = 37;
            // 
            // customDSep
            // 
            this.customDSep.AutoSize = true;
            this.customDSep.Location = new System.Drawing.Point(17, 61);
            this.customDSep.Name = "customDSep";
            this.customDSep.Size = new System.Drawing.Size(108, 17);
            this.customDSep.TabIndex = 36;
            this.customDSep.Text = "Custom separator";
            this.customDSep.UseVisualStyleBackColor = true;
            // 
            // cb_SourceDecimalPeriod
            // 
            this.cb_SourceDecimalPeriod.AutoSize = true;
            this.cb_SourceDecimalPeriod.Checked = true;
            this.cb_SourceDecimalPeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceDecimalPeriod.Location = new System.Drawing.Point(18, 42);
            this.cb_SourceDecimalPeriod.Name = "cb_SourceDecimalPeriod";
            this.cb_SourceDecimalPeriod.Size = new System.Drawing.Size(56, 17);
            this.cb_SourceDecimalPeriod.TabIndex = 35;
            this.cb_SourceDecimalPeriod.Text = "Period";
            this.cb_SourceDecimalPeriod.UseVisualStyleBackColor = true;
            // 
            // cb_SourceDecimalComma
            // 
            this.cb_SourceDecimalComma.AutoSize = true;
            this.cb_SourceDecimalComma.Checked = true;
            this.cb_SourceDecimalComma.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceDecimalComma.Location = new System.Drawing.Point(18, 19);
            this.cb_SourceDecimalComma.Name = "cb_SourceDecimalComma";
            this.cb_SourceDecimalComma.Size = new System.Drawing.Size(61, 17);
            this.cb_SourceDecimalComma.TabIndex = 34;
            this.cb_SourceDecimalComma.Text = "Comma";
            this.cb_SourceDecimalComma.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sourceTBox);
            this.groupBox3.Controls.Add(this.customTSep);
            this.groupBox3.Controls.Add(this.cb_SourceNoSeparator);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsPeriod);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsComma);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsNobreakThinSpace);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsThinSpace);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsNobreakSpace);
            this.groupBox3.Controls.Add(this.cb_SourceThousandsSpace);
            this.groupBox3.Location = new System.Drawing.Point(17, 271);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(190, 235);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Source thousands separators";
            this.toolTip1.SetToolTip(this.groupBox3, "Select all separators that should be considered thousands separators\r\nwhen they a" +
        "ppear in a place which is appropriate for a thousands separator.");
            // 
            // sourceTBox
            // 
            this.sourceTBox.Location = new System.Drawing.Point(17, 205);
            this.sourceTBox.Name = "sourceTBox";
            this.sourceTBox.Size = new System.Drawing.Size(100, 20);
            this.sourceTBox.TabIndex = 28;
            // 
            // customTSep
            // 
            this.customTSep.AutoSize = true;
            this.customTSep.Location = new System.Drawing.Point(17, 181);
            this.customTSep.Name = "customTSep";
            this.customTSep.Size = new System.Drawing.Size(108, 17);
            this.customTSep.TabIndex = 27;
            this.customTSep.Text = "Custom separator";
            this.customTSep.UseVisualStyleBackColor = true;
            // 
            // cb_SourceNoSeparator
            // 
            this.cb_SourceNoSeparator.AutoSize = true;
            this.cb_SourceNoSeparator.Checked = true;
            this.cb_SourceNoSeparator.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceNoSeparator.Location = new System.Drawing.Point(17, 157);
            this.cb_SourceNoSeparator.Name = "cb_SourceNoSeparator";
            this.cb_SourceNoSeparator.Size = new System.Drawing.Size(89, 17);
            this.cb_SourceNoSeparator.TabIndex = 26;
            this.cb_SourceNoSeparator.Text = "No Separator";
            this.cb_SourceNoSeparator.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsPeriod
            // 
            this.cb_SourceThousandsPeriod.AutoSize = true;
            this.cb_SourceThousandsPeriod.Checked = true;
            this.cb_SourceThousandsPeriod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsPeriod.Location = new System.Drawing.Point(17, 134);
            this.cb_SourceThousandsPeriod.Name = "cb_SourceThousandsPeriod";
            this.cb_SourceThousandsPeriod.Size = new System.Drawing.Size(56, 17);
            this.cb_SourceThousandsPeriod.TabIndex = 25;
            this.cb_SourceThousandsPeriod.Text = "Period";
            this.cb_SourceThousandsPeriod.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsComma
            // 
            this.cb_SourceThousandsComma.AutoSize = true;
            this.cb_SourceThousandsComma.Checked = true;
            this.cb_SourceThousandsComma.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsComma.Location = new System.Drawing.Point(17, 111);
            this.cb_SourceThousandsComma.Name = "cb_SourceThousandsComma";
            this.cb_SourceThousandsComma.Size = new System.Drawing.Size(61, 17);
            this.cb_SourceThousandsComma.TabIndex = 24;
            this.cb_SourceThousandsComma.Text = "Comma";
            this.cb_SourceThousandsComma.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsNobreakThinSpace
            // 
            this.cb_SourceThousandsNobreakThinSpace.AutoSize = true;
            this.cb_SourceThousandsNobreakThinSpace.Checked = true;
            this.cb_SourceThousandsNobreakThinSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsNobreakThinSpace.Location = new System.Drawing.Point(17, 88);
            this.cb_SourceThousandsNobreakThinSpace.Name = "cb_SourceThousandsNobreakThinSpace";
            this.cb_SourceThousandsNobreakThinSpace.Size = new System.Drawing.Size(137, 17);
            this.cb_SourceThousandsNobreakThinSpace.TabIndex = 23;
            this.cb_SourceThousandsNobreakThinSpace.Text = "Narrow no-break space";
            this.cb_SourceThousandsNobreakThinSpace.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsThinSpace
            // 
            this.cb_SourceThousandsThinSpace.AutoSize = true;
            this.cb_SourceThousandsThinSpace.Checked = true;
            this.cb_SourceThousandsThinSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsThinSpace.Location = new System.Drawing.Point(17, 65);
            this.cb_SourceThousandsThinSpace.Name = "cb_SourceThousandsThinSpace";
            this.cb_SourceThousandsThinSpace.Size = new System.Drawing.Size(79, 17);
            this.cb_SourceThousandsThinSpace.TabIndex = 22;
            this.cb_SourceThousandsThinSpace.Text = "Thin space";
            this.cb_SourceThousandsThinSpace.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsNobreakSpace
            // 
            this.cb_SourceThousandsNobreakSpace.AutoSize = true;
            this.cb_SourceThousandsNobreakSpace.Checked = true;
            this.cb_SourceThousandsNobreakSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsNobreakSpace.Location = new System.Drawing.Point(17, 42);
            this.cb_SourceThousandsNobreakSpace.Name = "cb_SourceThousandsNobreakSpace";
            this.cb_SourceThousandsNobreakSpace.Size = new System.Drawing.Size(102, 17);
            this.cb_SourceThousandsNobreakSpace.TabIndex = 21;
            this.cb_SourceThousandsNobreakSpace.Text = "No-break space";
            this.cb_SourceThousandsNobreakSpace.UseVisualStyleBackColor = true;
            // 
            // cb_SourceThousandsSpace
            // 
            this.cb_SourceThousandsSpace.AutoSize = true;
            this.cb_SourceThousandsSpace.Checked = true;
            this.cb_SourceThousandsSpace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_SourceThousandsSpace.Location = new System.Drawing.Point(17, 19);
            this.cb_SourceThousandsSpace.Name = "cb_SourceThousandsSpace";
            this.cb_SourceThousandsSpace.Size = new System.Drawing.Size(57, 17);
            this.cb_SourceThousandsSpace.TabIndex = 20;
            this.cb_SourceThousandsSpace.Text = "Space";
            this.cb_SourceThousandsSpace.UseVisualStyleBackColor = true;
            // 
            // combo_ModifiedNumbersErrorType
            // 
            this.combo_ModifiedNumbersErrorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_ModifiedNumbersErrorType.FormattingEnabled = true;
            this.combo_ModifiedNumbersErrorType.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Note"});
            this.combo_ModifiedNumbersErrorType.Location = new System.Drawing.Point(190, 77);
            this.combo_ModifiedNumbersErrorType.Name = "combo_ModifiedNumbersErrorType";
            this.combo_ModifiedNumbersErrorType.Size = new System.Drawing.Size(81, 21);
            this.combo_ModifiedNumbersErrorType.TabIndex = 8;
            // 
            // combo_RemovedNumbersErrorType
            // 
            this.combo_RemovedNumbersErrorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_RemovedNumbersErrorType.FormattingEnabled = true;
            this.combo_RemovedNumbersErrorType.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Note"});
            this.combo_RemovedNumbersErrorType.Location = new System.Drawing.Point(190, 50);
            this.combo_RemovedNumbersErrorType.Name = "combo_RemovedNumbersErrorType";
            this.combo_RemovedNumbersErrorType.Size = new System.Drawing.Size(81, 21);
            this.combo_RemovedNumbersErrorType.TabIndex = 6;
            // 
            // combo_AddedNumbersErrorType
            // 
            this.combo_AddedNumbersErrorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_AddedNumbersErrorType.FormattingEnabled = true;
            this.combo_AddedNumbersErrorType.Items.AddRange(new object[] {
            "Error",
            "Warning",
            "Note"});
            this.combo_AddedNumbersErrorType.Location = new System.Drawing.Point(190, 23);
            this.combo_AddedNumbersErrorType.Name = "combo_AddedNumbersErrorType";
            this.combo_AddedNumbersErrorType.Size = new System.Drawing.Size(81, 21);
            this.combo_AddedNumbersErrorType.TabIndex = 4;
            // 
            // cb_ReportModifiedNumbers
            // 
            this.cb_ReportModifiedNumbers.AutoSize = true;
            this.cb_ReportModifiedNumbers.Checked = true;
            this.cb_ReportModifiedNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ReportModifiedNumbers.Location = new System.Drawing.Point(16, 79);
            this.cb_ReportModifiedNumbers.Name = "cb_ReportModifiedNumbers";
            this.cb_ReportModifiedNumbers.Size = new System.Drawing.Size(143, 17);
            this.cb_ReportModifiedNumbers.TabIndex = 7;
            this.cb_ReportModifiedNumbers.Text = "Report &modified numbers";
            this.cb_ReportModifiedNumbers.UseVisualStyleBackColor = true;
            // 
            // cb_ReportRemovedNumbers
            // 
            this.cb_ReportRemovedNumbers.AutoSize = true;
            this.cb_ReportRemovedNumbers.Checked = true;
            this.cb_ReportRemovedNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ReportRemovedNumbers.Location = new System.Drawing.Point(16, 52);
            this.cb_ReportRemovedNumbers.Name = "cb_ReportRemovedNumbers";
            this.cb_ReportRemovedNumbers.Size = new System.Drawing.Size(145, 17);
            this.cb_ReportRemovedNumbers.TabIndex = 5;
            this.cb_ReportRemovedNumbers.Text = "Report &removed numbers";
            this.cb_ReportRemovedNumbers.UseVisualStyleBackColor = true;
            // 
            // cb_ReportAddedNumbers
            // 
            this.cb_ReportAddedNumbers.AutoSize = true;
            this.cb_ReportAddedNumbers.Checked = true;
            this.cb_ReportAddedNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ReportAddedNumbers.Location = new System.Drawing.Point(16, 25);
            this.cb_ReportAddedNumbers.Name = "cb_ReportAddedNumbers";
            this.cb_ReportAddedNumbers.Size = new System.Drawing.Size(134, 17);
            this.cb_ReportAddedNumbers.TabIndex = 3;
            this.cb_ReportAddedNumbers.Text = "Report &added numbers";
            this.cb_ReportAddedNumbers.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 8000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            // 
            // draftCheck
            // 
            this.draftCheck.AutoSize = true;
            this.draftCheck.Location = new System.Drawing.Point(17, 111);
            this.draftCheck.Name = "draftCheck";
            this.draftCheck.Size = new System.Drawing.Size(136, 17);
            this.draftCheck.TabIndex = 16;
            this.draftCheck.Text = "Exclude draft segments";
            this.draftCheck.UseVisualStyleBackColor = true;
            // 
            // NumberVerifierUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panelNumberVerifierUI);
            this.Name = "NumberVerifierUI";
            this.Size = new System.Drawing.Size(499, 666);
            this.panelNumberVerifierUI.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelNumberVerifierUI;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox combo_ModifiedNumbersErrorType;
        private System.Windows.Forms.ComboBox combo_RemovedNumbersErrorType;
        private System.Windows.Forms.ComboBox combo_AddedNumbersErrorType;
        private System.Windows.Forms.CheckBox cb_ReportModifiedNumbers;
        private System.Windows.Forms.CheckBox cb_ReportRemovedNumbers;
        private System.Windows.Forms.CheckBox cb_ReportAddedNumbers;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cb_TargetThousandsPeriod;
        private System.Windows.Forms.CheckBox cb_TargetThousandsComma;
        private System.Windows.Forms.CheckBox cb_TargetThousandsNobreakThinSpace;
        private System.Windows.Forms.CheckBox cb_TargetThousandsThinSpace;
        private System.Windows.Forms.CheckBox cb_TargetThousandsNobreakSpace;
        private System.Windows.Forms.CheckBox cb_TargetThousandsSpace;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox cb_TargetDecimalPeriod;
        private System.Windows.Forms.CheckBox cb_TargetDecimalComma;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cb_SourceDecimalPeriod;
        private System.Windows.Forms.CheckBox cb_SourceDecimalComma;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cb_SourceThousandsPeriod;
        private System.Windows.Forms.CheckBox cb_SourceThousandsComma;
        private System.Windows.Forms.CheckBox cb_SourceThousandsNobreakThinSpace;
        private System.Windows.Forms.CheckBox cb_SourceThousandsThinSpace;
        private System.Windows.Forms.CheckBox cb_SourceThousandsNobreakSpace;
        private System.Windows.Forms.CheckBox cb_SourceThousandsSpace;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox cb_Exclude100Percents;
        private System.Windows.Forms.CheckBox cb_ExcludeLockedSegments;
        private System.Windows.Forms.CheckBox cb_ExcludeTagText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rb_PreventLocalizations;
        private System.Windows.Forms.RadioButton rb_RequireLocalizations;
        private System.Windows.Forms.RadioButton rb_AllowLocalizations;
        private System.Windows.Forms.ComboBox combo_ModifiedAlphanumericsErrorType;
        private System.Windows.Forms.CheckBox cb_ReportModifiedAlphanumerics;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.RadioButton rb_ReportExtendedMessages;
        private System.Windows.Forms.RadioButton rb_ReportBriefMessages;
        private System.Windows.Forms.CheckBox cb_TargetNoSeparator;
        private System.Windows.Forms.CheckBox cb_SourceNoSeparator;
        private System.Windows.Forms.CheckBox untranslatedCheck;
        private System.Windows.Forms.TextBox sourceTBox;
        private System.Windows.Forms.CheckBox customTSep;
        private System.Windows.Forms.TextBox customTBox;
        private System.Windows.Forms.CheckBox targetTbox;
        private System.Windows.Forms.CheckBox customDSep;
        private System.Windows.Forms.CheckBox customTargetSep;
        private System.Windows.Forms.TextBox targetDBox;
        private System.Windows.Forms.TextBox sourceDBox;
        private System.Windows.Forms.CheckBox draftCheck;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
