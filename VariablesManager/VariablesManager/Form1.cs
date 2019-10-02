using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

        public string SelectedTm
        {
            get => _selectedTm.Trim();
	        set => _selectedTm = value;
        }

        public string SelectedLrt
        {
            get => _selectedLrt.Trim();
	        set => _selectedLrt = value;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowseTM_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = @"Translation memory (*.sdltm)|*.sdltm";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedTm = openFileDialog.FileName;
                txtTM.Text = Path.GetFileName(openFileDialog.FileName);
            }

        }

        private void btnBrowseLRT_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = @"Language Resource Template (*.resource)|*.resource";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedLrt = openFileDialog.FileName;
                txtLRT.Text = Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void btnImportFromFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = @"Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var sr = new StreamReader(openFileDialog.FileName))
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
            saveFileDialog.Filter = @"Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.AddExtension = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var sw = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        sw.Write(txtVariables.Text);
                    }
                }
                catch
                {
                }
            }

            MessageBox.Show(@"The variables list was exported to the selected file.");
        }

        private void btnFetchList_Click(object sender, EventArgs e)
        {
	        if (!string.IsNullOrEmpty(txtTM.Text))
	        {
				FetchFromTm();
			}
                
	        if (!string.IsNullOrEmpty(txtLRT.Text))
	        {
				FetchFromLrt();
			}
                
        }

        private void FetchFromLrt()
        {
            if (string.IsNullOrEmpty(SelectedLrt))
                return;

            var vars = GetVariablesAsTextFromLrt();
            if (string.IsNullOrEmpty(vars))
                return;

            if (!string.IsNullOrEmpty(txtVariables.Text) &&
                              txtVariables.Text[txtVariables.Text.Length - 1] != '\n')
                txtVariables.Text += "\r\n";
            txtVariables.Text += vars;
        }

        private string GetVariablesAsTextFromLrt()
        {
            try
            {
                var xFinalDoc = new XmlDocument();
                xFinalDoc.Load(SelectedLrt);
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
                                var textElement = textElements.FirstOrDefault();
                                var base64Vars = textElement.Value;
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

        private void FetchFromTm()
        {
            if (string.IsNullOrEmpty(txtTM.Text) || string.IsNullOrEmpty(SelectedTm))
                return;

            int count = 0;
            var vars = GetVariablesAsTextFromTm(out count);
            if (string.IsNullOrEmpty(vars))
                return;

            if (!string.IsNullOrEmpty(txtVariables.Text) && txtVariables.Text[txtVariables.Text.Length - 1] != '\n')
                txtVariables.Text += "\r\n";
            txtVariables.Text += vars;
        }

        private string GetVariablesAsTextFromTm(out int count)
        {
            count = 0;
            try
            {
                SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
                sb.DataSource = SelectedTm;
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
                           var buffer = GetBytes(reader);
                            return Encoding.UTF8.GetString(buffer);
                        }
                    }

                }
            }
            catch
            {
            }

            return string.Empty;
        }

        static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            var buffer = new byte[CHUNK_SIZE];
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
            if (!string.IsNullOrEmpty(txtTM.Text))
                AddToTm();
            if (!string.IsNullOrEmpty(txtLRT.Text))
                AddToLrt();
        }

        private void AddToLrt()
        {
            if (string.IsNullOrEmpty(SelectedLrt) || string.IsNullOrEmpty(txtVariables.Text))
                return;

            var vars = GetVariablesAsTextFromLrt() + txtVariables.Text;
            if (!string.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            var base64Vars = Convert.ToBase64String(Encoding.UTF8.GetBytes(vars));
            SetVariablesInLrt(base64Vars);

            MessageBox.Show(@"The variables list was add to the selected Language Resource Template.");
        }

        private void SetVariablesInLrt(string base64Vars)
        {
            try
            {
                var xFinalDoc = new XmlDocument();
                xFinalDoc.Load(SelectedLrt);
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

                using (var writer = new XmlTextWriter(SelectedLrt, null))
                {
                    writer.Formatting = Formatting.None;
                    xFinalDoc.Save(writer);
                }
            }
            catch
            {
            }
        }

        private void AddToTm()
        {
            if (string.IsNullOrEmpty(SelectedTm) || string.IsNullOrEmpty(txtVariables.Text))
                return;
            int count = 0;
            var vars = GetVariablesAsTextFromTm(out count) + txtVariables.Text;
            if (!string.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            SetVariablesInTm(Encoding.UTF8.GetBytes(vars), count);

            MessageBox.Show(@"The variables list was add to the selected Translation Memory.");
        }

        private void SetVariablesInTm(byte[] variablesAsBytes, int count)
        {
            try
            {
				var sb = new SQLiteConnectionStringBuilder
				{
					DataSource = SelectedTm,
					Version = 3,
					JournalMode = SQLiteJournalModeEnum.Off
				};
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
	                    if (count == 0)
	                    {
		                    command.CommandText =
			                    string.Format(
				                    "insert into resources (rowid, id, guid, type, language, data) values ({2}, {2}, '{0}', 1, '{1}', @data)",
				                    Guid.NewGuid(), GetTmLanguage(), maxID);
	                    }
	                    else
	                    {
							command.CommandText = "update resources set data = @data where type = 1";
						}
	                    command.Parameters.Add("@data", DbType.Binary).Value = variablesAsBytes;
	                    command.ExecuteNonQuery();
					}
                }
            }
            catch (Exception e)
            {
            }
        }

        private object GetTmLanguage()
        {
            try
            {
	            var sb = new SQLiteConnectionStringBuilder
	            {
		            DataSource = SelectedTm,
		            Version = 3,
		            JournalMode = SQLiteJournalModeEnum.Off
	            };
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
            return string.Empty;
        }

        private void btnReplateToTMorLRT_Click(object sender, EventArgs e)
        {
	        if (!string.IsNullOrEmpty(txtTM.Text))
	        {
				ReplaceToTm();
			}

	        if (!string.IsNullOrEmpty(txtLRT.Text))
	        {
				ReplaceToLrt();
			}
        }

        private void ReplaceToLrt()
        {
	        if (string.IsNullOrEmpty(SelectedLrt))
	        {
				return;
			}
	        var vars = txtVariables.Text;
	        if (!string.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
	        {
				vars += "\r\n";
			}
            var base64Vars = Convert.ToBase64String(Encoding.UTF8.GetBytes(vars));
            SetVariablesInLrt(base64Vars);

            MessageBox.Show(@"The variables list from the selected Language Resource Template was replaced with the new one.");
        }

        private void ReplaceToTm()
        {
            if (string.IsNullOrEmpty(SelectedTm))
                return;

            var vars = txtVariables.Text;
            if (!string.IsNullOrEmpty(vars) && vars[vars.Length - 1] != '\n')
                vars += "\r\n";

            var count = 0;
            GetVariablesAsTextFromTm(out count);
            SetVariablesInTm(Encoding.UTF8.GetBytes(vars), count);

            MessageBox.Show(@"The variables list from the selected Translation Memory was replaced with the new one.");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtVariables.Clear();
        }
    }
}
