using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing.Imaging;

namespace Sdl.Community.TermInjector
{
    public partial class TermInjectorTranslationProviderConfDialog : Form
    {        
        TermInjectorTranslationProvider provider;

        IWin32Window owner;
        #region "TermInjectorConfDialog"
        public TermInjectorTranslationProviderConfDialog(TermInjectorTranslationOptions options,TermInjectorTranslationProvider provider,IWin32Window owner)
        {
            this.provider = provider;
            this.Options = options;
            this.owner = owner;
            InitializeComponent();
            UpdateDialog();
        }

        public TermInjectorTranslationProviderConfDialog(TermInjectorTranslationOptions options,IWin32Window owner)
        {
            this.provider = null;
            this.Options = options;
            this.owner = owner;
            InitializeComponent();
            UpdateDialog();
        }

        public TermInjectorTranslationOptions Options
        {
            get;
            set;
        }
        #endregion

        #region "UpdateDialog"
        private void UpdateDialog()
        {
            glosFile.Text = Options.GlossaryFileName;
            delimiterCharacter.Text = Options.Delimiter;
            tmFile.Text = Options.TMFileName;
            regexFile.Text = Options.RegexFileName;
            termAdditionSeparator.Text = Options.TermAdditionSeparator;
            tokenBoundaryCharacters.Text = Options.TokenBoundaryCharacters;
            matchCase.CheckState =
                Options.MatchCase == "true" ?  CheckState.Checked : CheckState.Unchecked;
            InjectIntoFullMatchesCheckBox.CheckState =
                Options.InjectIntoFullMatches == "true" ? CheckState.Checked : CheckState.Unchecked;
            NewSegmentPercentageBox.Text = Options.NewSegmentPercentage.ToString();
            addnewtermscheckbox.CheckState =
                Options.InjectNewTermsIntoFuzzies == "true" ? CheckState.Checked : CheckState.Unchecked;
            useBoundaryChars.CheckState =
                Options.UseBoundaryCharacters == "true" ? CheckState.Checked : CheckState.Unchecked;
        }
        #endregion

        #region "Browse"
        private void btn_Browse_Click(object sender, EventArgs e)
        {
            this.dlg_OpenFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.dlg_OpenFile.FilterIndex = 1;
            this.dlg_OpenFile.ShowDialog();
            string fileName = dlg_OpenFile.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                glosFile.Text = fileName;
            }
        }
        #endregion        

        #region "OK"
        private void bnt_OK_Click(object sender, EventArgs e)
        {
            Options.Delimiter = delimiterCharacter.Text;
            Options.GlossaryFileName = glosFile.Text;
            Options.TMFileName = tmFile.Text;
            Options.RegexFileName = regexFile.Text;
            Options.TermAdditionSeparator = termAdditionSeparator.Text;
            Options.TokenBoundaryCharacters = tokenBoundaryCharacters.Text;
            Options.MatchCase =
                matchCase.CheckState == CheckState.Checked ? "true" : "false";
            Options.InjectIntoFullMatches =
                InjectIntoFullMatchesCheckBox.CheckState == CheckState.Checked ? "true" : "false";
            Options.InjectNewTermsIntoFuzzies =
                addnewtermscheckbox.CheckState == CheckState.Checked ? "true" : "false";
            Options.UseBoundaryCharacters =
                useBoundaryChars.CheckState == CheckState.Checked ? "true" : "false";
            //If new segment match percentage value is over 100, record it as 100
            if (Convert.ToInt32(NewSegmentPercentageBox.Text) > 100)
            {
                Options.NewSegmentPercentage = 100;
            }
            else
            {
                Options.NewSegmentPercentage = Convert.ToInt32(NewSegmentPercentageBox.Text);
            }
            
        }
        #endregion

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

        }

        private void btn_browseTM_Click_1(object sender, EventArgs e)
        {
            this.dlg_OpenFile.Filter = "SDL translation memories (*.sdltm)|*.sdltm|All files (*.*)|*.*";
            this.dlg_OpenFile.FilterIndex = 1;
            this.dlg_OpenFile.ShowDialog();
            string fileName = dlg_OpenFile.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                tmFile.Text = fileName;
            }
        }

        //This hides or shows the advanced settings
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
            {
                useBoundaryLabel.Visible = true;
                label3.Visible = true;
                tokenBoundaryCharacters.Visible = true;
                termAdditionSeparator.Visible = true;
                label1.Visible = true;
                delimiterCharacter.Visible = true;
                newmatchpercentageLabel.Visible = true;
                NewSegmentPercentageBox.Visible = true;
                addnewtermscheckbox.Visible = true;
                InjectIntoFullMatchesCheckBox.Visible = true;
                useBoundaryChars.Visible = true;
            }
            else
            {
                useBoundaryLabel.Visible = false;
                label3.Visible = false;
                tokenBoundaryCharacters.Visible = false;
                termAdditionSeparator.Visible = false;
                label1.Visible = false;
                delimiterCharacter.Visible = false;
                newmatchpercentageLabel.Visible = false;
                NewSegmentPercentageBox.Visible = false;
                addnewtermscheckbox.Visible = false;
                InjectIntoFullMatchesCheckBox.Visible = false;
                useBoundaryChars.Visible = false;
            }
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            Stream myStream;

            this.dlg_CreateFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.dlg_CreateFile.FilterIndex = 1;
            this.dlg_CreateFile.RestoreDirectory = true;

            if (this.dlg_CreateFile.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = this.dlg_CreateFile.OpenFile()) != null)
                {
                    myStream.Close();
                }
            }
            glosFile.Text = dlg_CreateFile.FileName;
        }

        private void btn_Help_Click(object sender, EventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var helpFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TermInjectorHelp");

            if (!Directory.Exists(helpFilePath))
            {
                Directory.CreateDirectory(helpFilePath);
            }

            // Get TermInjectorHelp.html images and .css file from resources 
            GetImageResourceForHelpFile(assembly, helpFilePath);
            GetCssResourceForHelpFile(assembly);

            // Get TermInjector.html resource
            Stream stream = assembly.GetManifestResourceStream("Sdl.Community.TermInjector.TermInjector_Help.TermInjectorHelp.html");
            var htmlPath = Path.Combine(helpFilePath, "TermInjectorHelp.html");

            WriteFileContent(stream, htmlPath);

            if (File.Exists(htmlPath))
            {
                System.Diagnostics.Process.Start(htmlPath);
            }
            else
            {
                MessageBox.Show("Help file has not been installed", "TermInjector");
            }
        }

        private void btn_browse_regex_Click(object sender, EventArgs e)
        {
         
            this.dlg_OpenFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.dlg_OpenFile.FilterIndex = 1;
            this.dlg_OpenFile.ShowDialog();
            string fileName = dlg_OpenFile.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                regexFile.Text = fileName;
            }
        }

        private void btn_create_regex_Click(object sender, EventArgs e)
        {
            Stream myStream;

            this.dlg_CreateFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.dlg_CreateFile.FilterIndex = 1;
            this.dlg_CreateFile.RestoreDirectory = true;

            if (this.dlg_CreateFile.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = this.dlg_CreateFile.OpenFile()) != null)
                {
                    myStream.Close();
                }
            }
            regexFile.Text = dlg_CreateFile.FileName;
        }

        private void btn_reload_Click(object sender, EventArgs e)
        {
            //Reload the tries
            if (this.provider != null)
            {
                
                this.provider.Options = this.Options;
                //this.provider.checkExistenceOfFiles();
                try
                {
                    this.provider.loadTries();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                this.provider.initializeVisitors();
            }            
        }

        private void EditExact_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.glosFile.Text))
                {
                    System.Diagnostics.Process.Start(this.glosFile.Text);
                }
            }
            catch
            {
            }
        }

        private void EditRegex_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.regexFile.Text))
                {
                    System.Diagnostics.Process.Start(this.regexFile.Text);
                }
            }
            catch
            {
            }
        }

        private void GetImageResourceForHelpFile(Assembly assembly, string helpFilePath)
        {
            var resources = assembly.GetManifestResourceNames();
            string imagePath = Path.Combine(helpFilePath, "images");
            int imageNumber = 1;

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            for (var i = 0; i < resources.Count(); i++)
            {
                if (resources[i].Contains(PluginResources.HelpImage_Name))
                {
                    Bitmap image = new Bitmap(assembly.GetManifestResourceStream(string.Concat(PluginResources.HelpImage_Name, imageNumber.ToString(), ".png")));
                    image.Save(Path.Combine(imagePath, string.Concat("HelpPic", imageNumber.ToString(), ".png")), ImageFormat.Png);
                    imageNumber += 1;
                }
            }
        }

        private void GetCssResourceForHelpFile(Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream("Sdl.Community.TermInjector.TermInjector_Help.help.css");

            var cssHelpFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TermInjectorHelp\\help.css");

            WriteFileContent(stream, cssHelpFile);
        }

        private void WriteFileContent(Stream stream, string filePath)
        {
            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var fileContent = reader.ReadToEnd();

                    // Create a file to write to. 
                    StreamWriter fileWriter = new StreamWriter(filePath);
                    fileWriter.WriteLine(fileContent);
                    fileWriter.Close();
                }
            }
        }
    }
}