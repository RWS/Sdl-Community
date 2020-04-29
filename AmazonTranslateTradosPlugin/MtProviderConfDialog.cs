/* 

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.*/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sdl.Community.AmazonTranslateProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    public partial class MtProviderConfDialog : Form
    {
        ITranslationProviderCredentialStore credstore;
        Point showcredsloc;
        Uri uriAmz;
        TranslationProviderCredential amzCred;

        public MtProviderConfDialog(MtTranslationOptions options, ITranslationProviderCredentialStore credentialStore)
        {
            this.credstore = credentialStore;
            uriAmz = new Uri("amazontranslateprovider:///");
            Options = options;
            InitializeComponent();
            UpdateDialog();
        }

        public MtProviderConfDialog(MtTranslationOptions options, string caption, ITranslationProviderCredentialStore credentialStore)
        {
            this.credstore = credentialStore;
            uriAmz = new Uri("amazontranslateprovider:///");
            Options = options;
            InitializeComponent();
            UpdateDialog();
            this.Text = caption;

        }

        /// <summary>
        /// Allows disabling all controls except for the ones to put in the credentials. 
        /// This is useful when Trados Studio receives a credential exception 
        /// (i.e. when we throw it from the Factory class if there are no credentials configured), 
        /// because any changes in the form to other settings cannot be saved in this case, 
        /// so the user could be confused. This converts the settings form into a form that just takes the credentials.
        /// </summary>
        public void DisableForCredentialsOnly()
        {
            tabControl1.TabPages.Remove(tabPage3);
            tabPage1.Text = MtProviderConfDialogResources.tabPage1_Text_creds_only;
            groupBox3.Text = MtProviderConfDialogResources.groupBox3_Text_creds_only;
            groupBox3.Enabled = false;
            ShowCredBox();

        }

        public MtTranslationOptions Options
        {
            get;
            set;
        }

        private void UpdateDialog()
        {
            showcredsloc = groupBoxAuth.Location; //holds our location of where to place the group box we are showing
            txtRegion.Text = Options.RegionName;
            txtAccessKey.Text = Options.AccessKey;
            txtSecretKey.Text = Options.SecretKey;
            txtAWSProfileName.Text = Options.ProfileName;
            chkSaveCred.Checked = Options.PersistAWSCreds;
            chkPlainTextOnly.Checked = Options.SendPlainTextOnly;
            comboAuthType.Text = MtTranslationOptions.GetAuthTypeDescription(Options.SelectedAuthType);
            chkResendDrafts.Checked = Options.ResendDrafts;
            chkUsePreEdit.Checked = Options.UsePreEdit;
            chkUsePostEdit.Checked = Options.UsePostEdit;
            txtPreEditFileName.Text = Options.PreLookupFilename;
            txtPostEditFileName.Text = Options.PostLookupFilename;


            //enable/disable controls
            groupBoxPostedit.Enabled = chkUsePostEdit.Checked;
            groupBoxPreedit.Enabled = chkUsePreEdit.Checked;

            this.Icon = MtProviderConfDialogResources.form_Icon;

            ShowCredBox();
            try
            {
                LoadResources();
            }
            catch { }
        }

        /// <summary>
        /// Loads strings to the form from our resources file....defaults have been set in the designer so this can be called in a try block
        /// </summary>
        private void LoadResources()
        {
            //FUTURE: localization of UI strings

            //the code in this region can be used to test localization...do not leave it in in production
            //change the culture of the current thread to test loading of resource file
            //CultureInfo newCulture = new CultureInfo("es-ES");
            //CultureInfo newCulture = new CultureInfo("it-IT");
            //CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            //CultureInfo originalUiCulture = Thread.CurrentThread.CurrentUICulture;
            //Thread.CurrentThread.CurrentCulture = newCulture;
            //Thread.CurrentThread.CurrentUICulture = newCulture;

            this.Text = MtProviderConfDialogResources.form_Text;
            this.btn_OK.Text = MtProviderConfDialogResources.btn_OK_Text;
            this.btn_Cancel.Text = MtProviderConfDialogResources.btn_Cancel_Text;
            this.btnBrowsePostEditFile.Text = MtProviderConfDialogResources.btnBrowsePostEditFile_Text;
            this.btnBrowsePreEdit.Text = MtProviderConfDialogResources.btnBrowsePreEdit_Text;
            this.btnDeleteSavedAWSKeys.Text = MtProviderConfDialogResources.btnDeleteSavedCreds_Text;
            this.chkPlainTextOnly.Text = MtProviderConfDialogResources.chkPlainTextOnly_Text;
            this.chkResendDrafts.Text = MtProviderConfDialogResources.chkResendDrafts_Text;
            this.chkSaveCred.Text = MtProviderConfDialogResources.chkSaveKey_Text;
            this.chkUsePostEdit.Text = MtProviderConfDialogResources.chkUsePostEdit_Text;
            this.chkUsePreEdit.Text = MtProviderConfDialogResources.chkUsePreEdit_Text;
            this.groupBox3.Text = MtProviderConfDialogResources.groupBox3_Text;
            this.groupBox4.Text = MtProviderConfDialogResources.groupBox4_Text;
            this.groupBoxAuth.Text = MtProviderConfDialogResources.groupBoxAuth_Text;
            this.groupBoxPostedit.Text = MtProviderConfDialogResources.groupBoxPostedit_Text;
            this.groupBoxPreedit.Text = MtProviderConfDialogResources.groupBoxPreedit_Text;
            this.lblAccessKey.Text = MtProviderConfDialogResources.lblAccessKey_Text;
            this.lblSecretKey.Text = MtProviderConfDialogResources.lblSecretKey_Text;
            this.lblAWSProfileName.Text = MtProviderConfDialogResources.lblAWSProfileName_Text;
            this.tabPage1.Text = MtProviderConfDialogResources.tabPage1_Text;
            this.tabPage3.Text = MtProviderConfDialogResources.tabPage3_Text;

            //create multiline tooltip text from strings in form resource file
            string ttip = MtProviderConfDialogResources.KeyForm_SaveKeyTooltip1;
            ttip += System.Environment.NewLine + MtProviderConfDialogResources.KeyForm_SaveKeyTooltip2;
            ttip += System.Environment.NewLine + MtProviderConfDialogResources.KeyForm_SaveKeyTooltip3;
            toolTip1.SetToolTip(chkSaveCred, ttip);


        }


        private void ShowCredBox()
        {
            if (comboAuthType.SelectedIndex == 0)
            {

                lblRegion.Enabled = false;
                lblAccessKey.Enabled = false;
                lblSecretKey.Enabled = false;
                txtRegion.Enabled = false;
                txtAccessKey.Enabled = false;
                txtSecretKey.Enabled = false;
                chkSaveCred.Enabled = false;
                btnDeleteSavedAWSKeys.Enabled = false;

                lblAWSProfileName.Enabled = true;
                txtAWSProfileName.Enabled = true;
            }
            else if (comboAuthType.SelectedIndex == 1)
            {
                lblRegion.Enabled = true;
                lblAccessKey.Enabled = true;
                lblSecretKey.Enabled = true;
                txtRegion.Enabled = true;
                txtAccessKey.Enabled = true;
                txtSecretKey.Enabled = true;
                chkSaveCred.Enabled = true;
                btnDeleteSavedAWSKeys.Enabled = true;

                lblAWSProfileName.Enabled = false;
                txtAWSProfileName.Enabled = false;
            }

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            Options.AccessKey = txtAccessKey.Text;
            Options.SecretKey = txtSecretKey.Text;
            Options.RegionName = txtRegion.Text;
            Options.ProfileName = txtAWSProfileName.Text;
            Options.PersistAWSCreds = chkSaveCred.Checked;
            Options.SendPlainTextOnly = chkPlainTextOnly.Checked;
            Options.SelectedAuthType = comboAuthType.SelectedIndex == 1 ? MtTranslationOptions.AWSAuthType.AccessKeys : MtTranslationOptions.AWSAuthType.Profile;
            Options.ResendDrafts = chkResendDrafts.Checked;
            Options.UsePreEdit = chkUsePreEdit.Checked;
            Options.UsePostEdit = chkUsePostEdit.Checked;
            Options.PreLookupFilename = txtPreEditFileName.Text;
            Options.PostLookupFilename = txtPostEditFileName.Text;

            this.DialogResult = DialogResult.OK;
            this.Close(); //dispose????
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Enables or disables a set of controls based on whether the given checkbox is checked or unchecked. 
        /// </summary>
        /// <param name="enableSet">An array of Controls whose Enabled property is to be modified.</param>
        /// <param name="boxToConsult">The checkbox whose Checked value will be consulted.</param>
        private void editControlsSetEnabled(Control[] enableSet, CheckBox boxToConsult)
        {

            for (int i = 0; i < enableSet.Length; i++)
            {
                if (boxToConsult.Checked) enableSet[i].Enabled = true;
                else enableSet[i].Enabled = false;
            }

        }

        private void comboProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCredBox();
        }


        private void chkUsePreEdit_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxPreedit.Enabled = chkUsePreEdit.Checked;
        }

        private void chkUsePostEdit_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxPostedit.Enabled = chkUsePostEdit.Checked;
        }

        private bool ValidateForm()
        {
            string newLine = System.Environment.NewLine;
            string prompt = MtProviderConfDialogResources.validationMessageHeader + newLine;
            bool result = true;
            if (comboAuthType.Text == string.Empty)
            {
                prompt += newLine + MtProviderConfDialogResources.validationMessageNoProvider;
                result = false;
            }
            if (Options.SelectedAuthType == MtTranslationOptions.AWSAuthType.AccessKeys
                && (txtAccessKey.Text == string.Empty 
                || txtSecretKey.Text == string.Empty
                || txtRegion.Text == string.Empty))
            {
                prompt += newLine + MtProviderConfDialogResources.validationMessageNoKey;
                result = false;
            }
            if (Options.SelectedAuthType == MtTranslationOptions.AWSAuthType.Profile //these strings should not be localized and are therefore hard-coded
                && txtAWSProfileName.Text == string.Empty)
            {
                prompt += newLine + MtProviderConfDialogResources.validationMessageNoProfileName;
                result = false;
            }
            if (chkUsePreEdit.Checked && txtPreEditFileName.Text == string.Empty)
            {
                prompt += newLine + MtProviderConfDialogResources.validationMessageNoPreLookupFile;
                result = false;
            }
            if (chkUsePostEdit.Checked && txtPostEditFileName.Text.Equals(""))
            {
                prompt += newLine + MtProviderConfDialogResources.validationMessageNoPostLookupFile;
                result = false;
            }
            if (!result)
            {
                MessageBox.Show(prompt);
            }
            return result;
        }


        private string BrowseEditFile()
        {
            //Note: The current thread culture will not affect the language of message boxes and other dialogs
            //there seem to be some tricky workarounds out there involving win32 API hooks...but that might be a bit much
            //for now the message boxes and other system dialogs will be in language set for Windows, no matter the language set for Studio (i.e. the language of the current thread);
            //another option could be to make a custom message box as a form, but custom file dialogs could be a pain
            //FUTURE: consider possibly making the plugin localized to the system settings and not the current thread set by Studio

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try //a simple way to check for the proper xml format is to try and deserialize it
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(openFile.FileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(EditCollection));
                        EditCollection edcoll = (EditCollection)serializer.Deserialize(reader);
                        edcoll = null;
                        return openFile.FileName;
                    }
                }
                catch (InvalidOperationException) //invalid operation is what happens when the xml can't be parsed into the objects correctly
                {
                    string caption = MtProviderConfDialogResources.lookupFileStructureCheckErrorCaption;
                    string message = string.Format(MtProviderConfDialogResources.lookupFileStructureCheckXmlProblemErrorMessage, System.IO.Path.GetFileName(openFile.FileName));
                    MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
                catch (Exception exp) //catch-all for any other kind of error...passes up a general message with the error description
                {
                    string caption = MtProviderConfDialogResources.lookupFileStructureCheckErrorCaption;
                    string message = MtProviderConfDialogResources.lookupFileStructureCheckGenericErrorMessage + " " + exp.Message;
                    MessageBox.Show(this, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void btnBrowsePreEdit_Click(object sender, EventArgs e)
        {
            openFile.Title = MtProviderConfDialogResources.lookupFileDialogPreLookupCaption;
            string file = BrowseEditFile();
            if (file != null)
                txtPreEditFileName.Text = openFile.FileName;
        }

        private void btnBrowsePostEditFile_Click(object sender, EventArgs e)
        {
            openFile.Title = MtProviderConfDialogResources.lookupFileDialogPostLookupCaption;
            string file = BrowseEditFile();
            if (file != null)
                txtPostEditFileName.Text = openFile.FileName;
        }



        private void btnDeleteSavedAWSKeys_Click(object sender, EventArgs e)
        {
            this.amzCred = credstore.GetCredential(uriAmz);
            if (amzCred != null && amzCred.Persist)
            {
                credstore.RemoveCredential(uriAmz);
                chkSaveCred.Checked = false;
                MessageBox.Show(MtProviderConfDialogResources.deleteCredentialsMessageSavedCredsDeleted);
            }
            else
            {
                MessageBox.Show(MtProviderConfDialogResources.deleteCredentialsMessageSavedCredsEmpty);
            }
        }

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(
				"https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3315.amazon-translate-mt-provider");
		}
	}
}
