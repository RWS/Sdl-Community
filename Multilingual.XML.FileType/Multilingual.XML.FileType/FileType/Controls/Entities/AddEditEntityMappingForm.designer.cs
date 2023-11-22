namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
    partial class AddEditEntityMappingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddEditEntityMappingForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonNumericEntity = new System.Windows.Forms.RadioButton();
            this.radioButtonCharacterEntity = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxEntityName = new System.Windows.Forms.TextBox();
            this.maskedTextBoxUnicodeValue = new System.Windows.Forms.MaskedTextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelEntitySample = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxCharacter = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.radioButtonNumericEntity);
            this.groupBox1.Controls.Add(this.radioButtonCharacterEntity);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonNumericEntity
            // 
            resources.ApplyResources(this.radioButtonNumericEntity, "radioButtonNumericEntity");
            this.radioButtonNumericEntity.Name = "radioButtonNumericEntity";
            this.radioButtonNumericEntity.TabStop = true;
            this.radioButtonNumericEntity.UseVisualStyleBackColor = true;
            this.radioButtonNumericEntity.CheckedChanged += new System.EventHandler(this.radioButtonNumericEntity_CheckedChanged);
            // 
            // radioButtonCharacterEntity
            // 
            resources.ApplyResources(this.radioButtonCharacterEntity, "radioButtonCharacterEntity");
            this.radioButtonCharacterEntity.Name = "radioButtonCharacterEntity";
            this.radioButtonCharacterEntity.TabStop = true;
            this.radioButtonCharacterEntity.UseVisualStyleBackColor = true;
            this.radioButtonCharacterEntity.CheckedChanged += new System.EventHandler(this.radioButtonCharacterEntity_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxEntityName
            // 
            resources.ApplyResources(this.textBoxEntityName, "textBoxEntityName");
            this.textBoxEntityName.Name = "textBoxEntityName";
            this.textBoxEntityName.TextChanged += new System.EventHandler(this.textBoxEntityName_TextChanged);
            // 
            // maskedTextBoxUnicodeValue
            // 
            resources.ApplyResources(this.maskedTextBoxUnicodeValue, "maskedTextBoxUnicodeValue");
            this.maskedTextBoxUnicodeValue.Name = "maskedTextBoxUnicodeValue";
            this.maskedTextBoxUnicodeValue.TextChanged += new System.EventHandler(this.maskedTextBoxUnicodeValue_TextChanged);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // labelEntitySample
            // 
            resources.ApplyResources(this.labelEntitySample, "labelEntitySample");
            this.labelEntitySample.Name = "labelEntitySample";
            this.labelEntitySample.UseMnemonic = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // textBoxCharacter
            // 
            resources.ApplyResources(this.textBoxCharacter, "textBoxCharacter");
            this.textBoxCharacter.Name = "textBoxCharacter";
            this.textBoxCharacter.TextChanged += new System.EventHandler(this.textBoxCharacter_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelEntitySample, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxCharacter, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxEntityName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.maskedTextBoxUnicodeValue, 1, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // AddEditEntityMappingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditEntityMappingForm";
            this.ShowIcon = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonNumericEntity;
        private System.Windows.Forms.RadioButton radioButtonCharacterEntity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxEntityName;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxUnicodeValue;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelEntitySample;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxCharacter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
