using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class ProjectTermsBatchTaskSettingsControl
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBoxFilters = new System.Windows.Forms.GroupBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonResetList = new System.Windows.Forms.Button();
            this.numericUpDownTermsLength = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTermsOccurrences = new System.Windows.Forms.NumericUpDown();
            this.labelTermsLength = new System.Windows.Forms.Label();
            this.labelOccurrences = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTerm = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxTerm = new System.Windows.Forms.TextBox();
            this.labelBlackList = new System.Windows.Forms.Label();
            this.listViewBlackList = new System.Windows.Forms.ListView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBoxFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsOccurrences)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxFilters
            // 
            this.groupBoxFilters.Controls.Add(this.buttonSave);
            this.groupBoxFilters.Controls.Add(this.buttonLoad);
            this.groupBoxFilters.Controls.Add(this.buttonDelete);
            this.groupBoxFilters.Controls.Add(this.buttonResetList);
            this.groupBoxFilters.Controls.Add(this.numericUpDownTermsLength);
            this.groupBoxFilters.Controls.Add(this.numericUpDownTermsOccurrences);
            this.groupBoxFilters.Controls.Add(this.labelTermsLength);
            this.groupBoxFilters.Controls.Add(this.labelOccurrences);
            this.groupBoxFilters.Controls.Add(this.label1);
            this.groupBoxFilters.Controls.Add(this.labelTerm);
            this.groupBoxFilters.Controls.Add(this.buttonAdd);
            this.groupBoxFilters.Controls.Add(this.textBoxTerm);
            this.groupBoxFilters.Controls.Add(this.labelBlackList);
            this.groupBoxFilters.Controls.Add(this.listViewBlackList);
            this.groupBoxFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBoxFilters.Location = new System.Drawing.Point(3, 3);
            this.groupBoxFilters.Name = "groupBoxFilters";
            this.groupBoxFilters.Size = new System.Drawing.Size(581, 346);
            this.groupBoxFilters.TabIndex = 2;
            this.groupBoxFilters.TabStop = false;
            this.groupBoxFilters.Text = "Project terms filters";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(201, 204);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(130, 23);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(201, 175);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(130, 23);
            this.buttonLoad.TabIndex = 17;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(201, 115);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(130, 23);
            this.buttonDelete.TabIndex = 16;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonResetList
            // 
            this.buttonResetList.Location = new System.Drawing.Point(201, 145);
            this.buttonResetList.Name = "buttonResetList";
            this.buttonResetList.Size = new System.Drawing.Size(130, 23);
            this.buttonResetList.TabIndex = 15;
            this.buttonResetList.Text = "Reset";
            this.buttonResetList.UseVisualStyleBackColor = true;
            this.buttonResetList.Click += new System.EventHandler(this.buttonResetList_Click);
            // 
            // numericUpDownTermsLength
            // 
            this.numericUpDownTermsLength.Location = new System.Drawing.Point(139, 302);
            this.numericUpDownTermsLength.Name = "numericUpDownTermsLength";
            this.numericUpDownTermsLength.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownTermsLength.TabIndex = 11;
            // 
            // numericUpDownTermsOccurrences
            // 
            this.numericUpDownTermsOccurrences.Location = new System.Drawing.Point(139, 273);
            this.numericUpDownTermsOccurrences.Name = "numericUpDownTermsOccurrences";
            this.numericUpDownTermsOccurrences.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownTermsOccurrences.TabIndex = 10;
            // 
            // labelTermsLength
            // 
            this.labelTermsLength.AutoSize = true;
            this.labelTermsLength.Location = new System.Drawing.Point(10, 304);
            this.labelTermsLength.Name = "labelTermsLength";
            this.labelTermsLength.Size = new System.Drawing.Size(71, 13);
            this.labelTermsLength.TabIndex = 7;
            this.labelTermsLength.Text = "Terms length:";
            // 
            // labelOccurrences
            // 
            this.labelOccurrences.AutoSize = true;
            this.labelOccurrences.Location = new System.Drawing.Point(10, 274);
            this.labelOccurrences.Name = "labelOccurrences";
            this.labelOccurrences.Size = new System.Drawing.Size(104, 13);
            this.labelOccurrences.TabIndex = 6;
            this.labelOccurrences.Text = "Terms  occurrences:";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(6, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(571, 2);
            this.label1.TabIndex = 5;
            this.label1.Text = "labelSeparator1";
            // 
            // labelTerm
            // 
            this.labelTerm.AutoSize = true;
            this.labelTerm.Location = new System.Drawing.Point(198, 34);
            this.labelTerm.Name = "labelTerm";
            this.labelTerm.Size = new System.Drawing.Size(111, 13);
            this.labelTerm.TabIndex = 4;
            this.labelTerm.Text = "Enter term to blacklist:";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(201, 85);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(130, 23);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxTerm
            // 
            this.textBoxTerm.Location = new System.Drawing.Point(201, 55);
            this.textBoxTerm.Name = "textBoxTerm";
            this.textBoxTerm.Size = new System.Drawing.Size(130, 20);
            this.textBoxTerm.TabIndex = 2;
            // 
            // labelBlackList
            // 
            this.labelBlackList.AutoSize = true;
            this.labelBlackList.Location = new System.Drawing.Point(7, 18);
            this.labelBlackList.Name = "labelBlackList";
            this.labelBlackList.Size = new System.Drawing.Size(49, 13);
            this.labelBlackList.TabIndex = 1;
            this.labelBlackList.Text = "Blacklist:";
            // 
            // listViewBlackList
            // 
            this.listViewBlackList.FullRowSelect = true;
            this.listViewBlackList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewBlackList.Columns.Add("Terms", 178);
            this.listViewBlackList.Location = new System.Drawing.Point(7, 37);
            this.listViewBlackList.Name = "listViewBlackList";
            this.listViewBlackList.Size = new System.Drawing.Size(182, 190);
            this.listViewBlackList.TabIndex = 0;
            this.listViewBlackList.UseCompatibleStateImageBehavior = false;
            this.listViewBlackList.View = System.Windows.Forms.View.Details;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ProjectTermsBatchTaskSettingsControl
            // 
            this.Controls.Add(this.groupBoxFilters);
            this.Name = "ProjectTermsBatchTaskSettingsControl";
            this.Size = new System.Drawing.Size(587, 400);
            this.groupBoxFilters.ResumeLayout(false);
            this.groupBoxFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsOccurrences)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        private GroupBox groupBoxFilters;
        private ListView listViewBlackList;
        private Label labelTermsLength;
        private Label labelOccurrences;
        private Label label1;
        private Label labelTerm;
        private Button buttonAdd;
        private TextBox textBoxTerm;
        private Label labelBlackList;
        private BindingSource bindingSource1;
        private System.ComponentModel.IContainer components;
        private NumericUpDown numericUpDownTermsLength;
        private NumericUpDown numericUpDownTermsOccurrences;
        private ErrorProvider errorProvider1;
        private Button buttonResetList;
        private Button buttonDelete;
        private Button buttonSave;
        private Button buttonLoad;
    }
}
