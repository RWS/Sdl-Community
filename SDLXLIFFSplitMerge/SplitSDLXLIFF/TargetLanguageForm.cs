using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class TargetLanguageForm : Form
    {
        private string _projPath;

        /// <summary>
        /// all languages in project
        /// </summary>
        public List<string> AllLang
        { get; private set; }
        /// <summary>
        /// languages in project user selected
        /// </summary>
        public List<string> SelectedLang
        { get; private set; }

        public TargetLanguageForm(string projecPath, Dictionary<string, string> targetFiles)
        {
            InitializeComponent();
            OkButtonMode(false);

            _projPath = projecPath;

            AllLang = new List<string>();
            foreach (KeyValuePair<string, string> pair in targetFiles)
                if (!AllLang.Contains(pair.Value))
                    AllLang.Add(pair.Value);

            SelectedLang = new List<string>();
            bindLangs();
        }

        private void bindLangs()
        {
            AllLang.Sort();
            foreach (string lang in AllLang)
                clbLangs.Items.Add(lang);
        }

        private void OkButtonMode(bool isEnable)
        {
            btnOK.Enabled = isEnable;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbLangs.Items.Count; i++)
                if (clbLangs.GetItemCheckState(i) == CheckState.Checked && !SelectedLang.Contains((string)clbLangs.Items[i]))
                    SelectedLang.Add((string)clbLangs.Items[i]);

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void clbLangs_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isAnyChecked = false;
            for (int i = 0; i < clbLangs.Items.Count; i++)
                if (clbLangs.GetItemCheckState(i) == CheckState.Checked)
                {
                    isAnyChecked = true;
                    break;
                }
            OkButtonMode(isAnyChecked);
        }
    }
}
