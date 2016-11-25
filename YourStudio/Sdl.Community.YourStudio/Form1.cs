using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using Sdl.Community.YourStudio.Properties;

namespace Sdl.Community.YourStudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private XmlNodeList _sections;
        private XmlDocument _xDoc;
        private XmlElement _selectedSection;
        private XmlElement _theTab;
        private Dictionary<String, String> _tabFiles = new Dictionary<string, string>();
        private String _currentFile;

        private String GetTabFolder()
        {
            var studio2015Path = GetStudioPath();
            return Path.Combine(studio2015Path, "Welcome");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _tabFiles = GetAllTabFiles();
            foreach (var tabFile in _tabFiles)
            {
                tabConfigurationFilesList.Items.Add(tabFile.Value);
            }
            btnUp.Enabled = btnDown.Enabled = false;
            CancelButton = btnClose;
        }

        private Dictionary<String, String> GetAllTabFiles()
        {
            String tabFilesFolder = GetTabFolder();
            String[] files = Directory.GetFiles(tabFilesFolder, "*.xml");
            return files.ToDictionary(file => file, Path.GetFileName);
        }

        private string GetStudioPath()
        {
            String defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"SDL\SDL Trados Studio\Studio3\");
            RegistryKey installKey = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\SDL\Studio5");
            if (installKey == null) installKey = Registry.LocalMachine.OpenSubKey(@"Software\SDL\Studio5");
            if (installKey == null) return defaultPath;

            String installLocation = installKey.GetValue("InstallLocation", defaultPath).ToString();
            return installLocation;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Process.Start(GetTabFolder());
        }

        private void tabConfigurationFilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedSection != null) SaveSectionInfo();
            if (_currentFile != null) SaveTheDocument();
            
            txtIntro.Text = String.Empty;
            txtName.Text = String.Empty;
            txtUri.Text = String.Empty;
            cmbThumbs.SelectedIndex = -1;

            _currentFile = _tabFiles.FirstOrDefault(tab => tab.Value == tabConfigurationFilesList.SelectedItem.ToString()).Key;
            LoadSections(_currentFile);
        }

        private void LoadSections(string tabFile)
        {
            btnUp.Enabled = btnDown.Enabled = false;
            _sections = null;
            _selectedSection = null;
            _theTab = null;
            sectionsList.Items.Clear();
            _xDoc = new XmlDocument();
            _xDoc.Load(tabFile);
            if (_xDoc.DocumentElement != null)
            {
                XmlNodeList tabs = _xDoc.DocumentElement.GetElementsByTagName("tab");
                foreach (XmlElement tab in tabs)
                {
                    if (tab.HasAttribute("name") && tab.Attributes["name"].Value == "MoreResources")
                    {
                        _theTab = tab;
                        _sections = tab.GetElementsByTagName("section");
                        break;
                    }
                }
            }

            if (_sections != null && _sections.Count > 0)
            {
                foreach (XmlElement section in _sections)
                {
                    XmlNodeList names = section.GetElementsByTagName("name");
                    if (names.Count > 0)
                    {
                        XmlElement name = (XmlElement)names[0];
                        if (name != null && name.FirstChild != null && name.FirstChild.Value != null) sectionsList.Items.Add(name.FirstChild.Value);
                    }
                }
            }
        }

        private void sectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedSection != null)
            {
                SaveSectionInfo();
            }
            if (sectionsList.SelectedIndex == -1)
                return;

            btnUp.Enabled = sectionsList.SelectedIndex > 0;
            btnDown.Enabled = sectionsList.SelectedIndex < sectionsList.Items.Count - 1 && sectionsList.SelectedIndex != -1;

            _selectedSection = (XmlElement)_sections[sectionsList.SelectedIndex];
            XmlNodeList names = _selectedSection.GetElementsByTagName("name");
            if (names.Count > 0)
            {
                XmlElement name = (XmlElement) names[0];
                if (name != null && name.FirstChild != null && name.FirstChild.Value != null) txtName.Text = name.FirstChild.Value;
            }
            XmlNodeList uris = _selectedSection.GetElementsByTagName("uri");
            if (uris.Count > 0)
            {
                XmlElement uri = (XmlElement) uris[0];
                if (uri != null && uri.FirstChild != null && uri.FirstChild.Value != null) txtUri.Text = uri.FirstChild.Value;
            }
            XmlNodeList thumbs = _selectedSection.GetElementsByTagName("thumb");
            if (thumbs.Count > 0)
            {
                XmlElement thumb = (XmlElement) thumbs[0];
                if (thumb != null && thumb.FirstChild != null && thumb.FirstChild.Value != null) cmbThumbs.SelectedItem = thumb.FirstChild.Value;
            }
            XmlNodeList intros = _selectedSection.GetElementsByTagName("intro");
            if (intros.Count > 0)
            {
                XmlElement intro = (XmlElement) intros[0];
                if (intro != null && intro.FirstChild != null && intro.FirstChild.Value != null) txtIntro.Text = intro.FirstChild.Value;
            }
        }

        private bool _sectionsHasChanges;
        private void SaveSectionInfo()
        {
            XmlNodeList names = _selectedSection.GetElementsByTagName("name");
            if (names.Count > 0)
            {
                XmlElement name = (XmlElement)names[0];
                if (txtName.Text != String.Empty && name != null && name.FirstChild != null && name.FirstChild.Value != null && name.FirstChild.Value != txtName.Text)
                {
                    name.FirstChild.Value = txtName.Text;
                    _sectionsHasChanges = true;
                }
            }
            XmlNodeList uris = _selectedSection.GetElementsByTagName("uri");
            if (uris.Count > 0)
            {
                XmlElement uri = (XmlElement)uris[0];
                if (uri != null && uri.FirstChild != null && uri.FirstChild.Value != null && uri.FirstChild.Value != txtUri.Text)
                {
                    uri.FirstChild.Value = txtUri.Text;
                    _sectionsHasChanges = true;
                }
            }
            XmlNodeList thumbs = _selectedSection.GetElementsByTagName("thumb");
            if (thumbs.Count > 0)
            {
                XmlElement thumb = (XmlElement) thumbs[0];
                if (cmbThumbs.SelectedIndex != -1 && cmbThumbs.SelectedItem != null && thumb != null && thumb.FirstChild != null && thumb.FirstChild.Value != null && thumb.FirstChild.Value != cmbThumbs.SelectedItem.ToString())
                {
                    thumb.FirstChild.Value = cmbThumbs.SelectedItem.ToString();
                    _sectionsHasChanges = true;
                }
            }
            XmlNodeList intros = _selectedSection.GetElementsByTagName("intro");
            if (intros.Count > 0)
            {
                XmlElement intro = (XmlElement)intros[0];
                if (intro != null && intro.FirstChild != null && intro.FirstChild.Value != null && intro.FirstChild.Value != txtIntro.Text)
                {
                    intro.FirstChild.Value = txtIntro.Text;
                    _sectionsHasChanges = true;
                }
            }
        }

        private void Save()
        {
            if (_selectedSection != null) SaveSectionInfo();
            SaveTheDocument();
        }

        private void SaveTheDocument()
        {
            if (_sectionsHasChanges)
            {
                try
                {
                    using (XmlTextWriter writer = new XmlTextWriter(_currentFile, null))
                    {
                        writer.Formatting = Formatting.None;
                        _xDoc.Save(writer);
                    }
                    _sectionsHasChanges = false;
                } catch
                {
                    MessageBox.Show(Resources.Form1_SaveTheDocument_Please_run_the_application_with_administrator_priviliges);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Save();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_theTab != null && _selectedSection != null)
            {
                _theTab.RemoveChild(_selectedSection);
                _sectionsHasChanges = true;
                SaveTheDocument();
                sectionsList.Items.RemoveAt(sectionsList.SelectedIndex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text == String.Empty || cmbThumbs.SelectedIndex == -1 || cmbThumbs.SelectedItem == null || txtUri.Text == String.Empty || txtIntro.Text == String.Empty)
            {
                MessageBox.Show(Resources.Form1_btnAdd_Click_Please_fill_in_all_the_fields_before_adding_another_section_, Resources.Form1_btnAdd_Click_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_theTab != null)
            {
                XmlElement section = _xDoc.CreateElement("section");
                XmlElement extra = _xDoc.CreateElement("extra");
                section.AppendChild(extra);
                XmlElement items = _xDoc.CreateElement("items");
                section.AppendChild(items);

                XmlElement name = _xDoc.CreateElement("name");
                name.InnerText = txtName.Text;
                section.AppendChild(name);

                XmlElement thumb = _xDoc.CreateElement("thumb");
                thumb.InnerText = cmbThumbs.SelectedItem.ToString();
                section.AppendChild(thumb);

                XmlElement uri = _xDoc.CreateElement("uri");
                uri.InnerText = txtUri.Text;
                section.AppendChild(uri);

                XmlElement intro = _xDoc.CreateElement("intro");
                intro.InnerText = txtIntro.Text;
                section.AppendChild(intro);
                _theTab.AppendChild(section);

                _sectionsHasChanges = true;
                SaveTheDocument();
                LoadSections(_currentFile);
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (_selectedSection == null) return;
            SaveSectionInfo();
            int selectedIndex = sectionsList.SelectedIndex;
            XmlElement upElement = (XmlElement)_sections[selectedIndex - 1];
            XmlElement currentElement = _selectedSection;
            XmlElement tempElement = _xDoc.CreateElement("section");
            _theTab.AppendChild(tempElement);

            _theTab.ReplaceChild(tempElement, currentElement);
            _theTab.ReplaceChild(currentElement, upElement);
            _theTab.ReplaceChild(upElement, tempElement);

            _sectionsHasChanges = true;
            SaveTheDocument();
            LoadSections(_currentFile);
            sectionsList.SelectedIndex = selectedIndex - 1;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (_selectedSection == null) return;
            SaveSectionInfo();
            int selectedIndex = sectionsList.SelectedIndex;
            XmlElement downElement = (XmlElement)_sections[selectedIndex + 1];
            XmlElement currentElement = _selectedSection;
            XmlElement tempElement = _xDoc.CreateElement("section");
            _theTab.AppendChild(tempElement);

            _theTab.ReplaceChild(tempElement, currentElement);
            _theTab.ReplaceChild(currentElement, downElement);
            _theTab.ReplaceChild(downElement, tempElement);

            _sectionsHasChanges = true;
            SaveTheDocument();
            LoadSections(_currentFile);
            sectionsList.SelectedIndex = selectedIndex + 1;
        }
    }
}
