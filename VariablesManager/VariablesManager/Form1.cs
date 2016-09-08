using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace VariablesManager
{
    public partial class Form1 : Form
    {
        private string _selectedTm;
        private string _selectedLrt;

        public String SelectedTM
        {
            get { return _selectedTm.Trim(); }
            set { _selectedTm = value; }
        }

        public String SelectedLRT
        {
            get { return _selectedLrt.Trim(); }
            set { _selectedLrt = value; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowseTM_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Translation memory (*.sdltm)|*.sdltm";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedTM = openFileDialog.FileName;
                txtTM.Text = Path.GetFileName(openFileDialog.FileName);
            }

        }

        private void btnBrowseLRT_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Language Resource Template (*.resource)|*.resource";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedLRT = openFileDialog.FileName;
                txtLRT.Text = Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void btnImportFromFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {
                        txtVariables.Text = sr.ReadToEnd();
                    }

                }
                catch
                {
                }
            }
        }

        private void btnExportToFile_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        sw.Write(txtVariables.Text);
                    }
                }
                catch
                {
                }
            }

            MessageBox.Show("The variables list was exported to the selected file.");
        }

        private void btnFetchList_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTM.Text))
                FetchFromTM();
            if (!String.IsNullOrEmpty(txtLRT.Text))
                FetchFromLRT();
        }

        private void FetchFromLRT()
        {
            if (String.IsNullOrEmpty(SelectedLRT))
                return;

            String vars = GetVariablesAsTextFromLRT();
            if (String.IsNullOrEmpty(vars))
                return;

            if (!String.IsNullOrEmpty(txtVariables.Text) &&
                              txtVariables.Text[txtVariables.Text.Length - 1] != '\n')
                txtVariables.Text += "\r\n";
            txtVariables.Text += vars;
        }

        private String GetVariablesAsTextFromLRT()
        {
            try
            {
                XmlDocument xFinalDoc = new XmlDocument();
                xFinalDoc.Load(SelectedLRT);
                XmlNodeList languageResources = xFinalDoc.DocumentElement.GetElementsByTagName("LanguageResource");
                if (languageResources.Count > 0)
                {
                    foreach (XmlElement languageResource in languageResources)
                    {
                        if (languageResource.HasAttribute("Type") &&
                            languageResource.Attributes["Type"].Value == "Variables")
                        {
                            IEnumerable<XmlText> textElements = languageResource.ChildNodes.OfType<XmlText>();
                            if (textElements.Any())
                            {
                                XmlText textElement = textElements.FirstOrDefault();
                                String base64Vars = textElement.Value;
                                return Encoding.UTF8.GetString(Convert.FromBase64String(base64Vars));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        private void FetchFromTM()
        {
            if (String.IsNullOrEmpty(txtTM.Text) || String.IsNullOrEmpty(SelectedTM))
                return;

            int count = 0;
            String vars = GetVariablesAsTextFromTM(out count);
            if (String.IsNullOrEmpty(vars))
                return;

            if (!String.IsNullOrEmpty(txtVariables.Text) && txtVariables.Text[txtVariables.Text.Length - 1] != '\n')
                txtVariables.Text += "\r\n";
            txtVariables.Text += vars;
        }

        private string GetVariablesAsTextFromTM(out int count)
        {
            count = 0;
            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = SelectedTM;
                sb.Version = 3;
                sb.JournalMode = SQLiteJournalModeEnum.Off;
                using (var connection = new SQLiteConnection(sb.ConnectionString, true))
                using (var command = new SQLiteCommand(connection))
                {
                    connection.Open();

                    command.CommandText = "SELECT data FROM resources where type = 1";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count++;
                            byte[] buffer = GetBytes(reader);
                            return Encoding.UTF8.GetString(buffer);
                        }
                    }

                }
            }
            catch
            {
            }

            return String.Empty;
        }

        static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        private void btnAddToTMorLRT_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTM.Text))
                AddToTM();
            if (!String.IsNullOrEmpty(txtLRT.Text))
                AddToLRT();
        }

        private void AddToLRT()
        {
            if (String.IsNullOrEmpty(SelectedLRT) || String.IsNullOrEmpty(txtVariables.Text))
                return;

            String vars = GetVariablesAsTextFromLRT() + txtVariables.Text;
            if (!String.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            String base64Vars = Convert.ToBase64String(Encoding.UTF8.GetBytes(vars));
            SetVariablesInLRT(base64Vars);

            MessageBox.Show("The variables list was add to the selected Language Resource Template.");
        }

        private void SetVariablesInLRT(String base64Vars)
        {
            try
            {
                XmlDocument xFinalDoc = new XmlDocument();
                xFinalDoc.Load(SelectedLRT);
                XmlNodeList languageResources = xFinalDoc.DocumentElement.GetElementsByTagName("LanguageResource");
                if (languageResources.Count > 0)
                {
                    foreach (XmlElement languageResource in languageResources)
                    {
                        if (languageResource.HasAttribute("Type") &&
                            languageResource.Attributes["Type"].Value == "Variables")
                        {
                            languageResource.InnerText = base64Vars;
                        }
                    }
                }

                using (var writer = new XmlTextWriter(SelectedLRT, null))
                {
                    writer.Formatting = Formatting.None;
                    xFinalDoc.Save(writer);
                }
            }
            catch
            {
            }
        }

        private void AddToTM()
        {
            if (String.IsNullOrEmpty(SelectedTM) || String.IsNullOrEmpty(txtVariables.Text))
                return;
            int count = 0;
            String vars = GetVariablesAsTextFromTM(out count) + txtVariables.Text;
            if (!String.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            SetVariablesInTM(Encoding.UTF8.GetBytes(vars), count);

            MessageBox.Show("The variables list was add to the selected Translation Memory.");
        }

        private void SetVariablesInTM(byte[] variablesAsBytes, int count)
        {
            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = SelectedTM;
                sb.Version = 3;
                sb.JournalMode = SQLiteJournalModeEnum.Off;
                int maxID = 0;
                if (count == 0)
                {
                    using (var connection = new SQLiteConnection(sb.ConnectionString, true))
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            connection.Open();
                            command.CommandText = "Select max(id) from resources";
                            object oId = command.ExecuteScalar();
                            maxID = oId == DBNull.Value ? 1 : Convert.ToInt32(oId) + 1;
                        }
                    }
                }
                using (var connection = new SQLiteConnection(sb.ConnectionString, true))
                {
                    using (var command = new SQLiteCommand(connection))
                    {
                        connection.Open();
                        if (count == 0) command.CommandText = String.Format("insert into resources (rowid, id, guid, type, language, data) values ({2}, {2}, '{0}', 1, '{1}', @data)",Guid.NewGuid(), GetTMLanguage(), maxID);
                        else
                            command.CommandText = "update resources set data = @data where type = 1";
                        command.Parameters.Add("@data", DbType.Binary).Value = variablesAsBytes;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
            }
        }

        private object GetTMLanguage()
        {
            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = SelectedTM;
                sb.Version = 3;
                sb.JournalMode = SQLiteJournalModeEnum.Off;
                using (var connection = new SQLiteConnection(sb.ConnectionString, true))
                using (var command = new SQLiteCommand(connection))
                {
                    connection.Open();

                    command.CommandText = "SELECT source_language FROM translation_memories";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }

                }
            }
            catch
            {
            }

            return String.Empty;
        }

        private void btnReplateToTMorLRT_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTM.Text))
                ReplaceToTM();
            if (!String.IsNullOrEmpty(txtLRT.Text))
                ReplaceToLRT();
        }

        private void ReplaceToLRT()
        {
            if (String.IsNullOrEmpty(SelectedLRT))
                return;

            String vars = txtVariables.Text;
            if (!String.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            String base64Vars = Convert.ToBase64String(Encoding.UTF8.GetBytes(vars));
            SetVariablesInLRT(base64Vars);

            MessageBox.Show("The variables list from the selected Language Resource Template was replaced with the new one.");
        }

        private void ReplaceToTM()
        {
            if (String.IsNullOrEmpty(SelectedTM))
                return;

            String vars = txtVariables.Text;
            if (!String.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            int count = 0;
            GetVariablesAsTextFromTM(out count);
            SetVariablesInTM(Encoding.UTF8.GetBytes(vars), count);

            MessageBox.Show("The variables list from the selected Translation Memory was replaced with the new one.");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtVariables.Clear();
        }
    }
}
