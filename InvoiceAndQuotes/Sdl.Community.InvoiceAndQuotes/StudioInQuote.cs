using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml.XPath;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.FolderSelect;
using Sdl.Community.InvoiceAndQuotes.Projects;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes
{
    public partial class StudioInQuote : Form
    {
        #region private controls
        private Rates _rates;
        private NoLabelTextBox txtCustomerName;
        private NoLabelTextBox txtCustomerStreet;
        private NoLabelTextBox txtCustomerCountry;
        private NoLabelTextBox txtCustomerZip;
        private NoLabelTextBox txtCustomerState;
        private NoLabelTextBox txtCustomerCity;
        private NoLabelTextBox txtUserTwitter;
        private NoLabelTextBox txtUserWebAdress;
        private NoLabelTextBox txtUserSkype;
        private NoLabelTextBox txtUserEmail;
        private NoLabelTextBox txtUserMobile;
        private NoLabelTextBox txtUserPhone;
        private NoLabelTextBox txtUserCountry;
        private NoLabelTextBox txtUserZip;
        private NoLabelTextBox txtUserState;
        private NoLabelTextBox txtUserCity;
        private NoLabelTextBox txtUserStreet;
        private NoLabelTextBox txtUserName;
        private string _culture;
        #endregion

        public StudioInQuote()
        {
            InitializeComponent();
            InitializeCustomControls();
            //var test = new ResXResourceReader("InvoiceAndQuotes.de-DE.resx");
            //var test1 = new ResXResourceReader("InvoiceAndQuotes.en-US.resx");
            var res = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            _rates = new Rates {Name = "rates", Dock = DockStyle.Fill, Visible = true};
            groupStudioAnalysisBands.Controls.Add(_rates);
        }

        private void InitializeCustomControls()
        {
// 
            // txtCustomerCountry
            // 
            txtCustomerCountry = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Country:",
                    Location = new Point(5, 255),
                    Name = "txtCustomerCountry",
                    Size = new Size(301, 20),
                    TabIndex = 6
                };
            // 
            // txtCustomerZip
            // 
            txtCustomerZip = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Zip/Postal Code:",
                    Location = new Point(5, 229),
                    Name = "txtCustomerZip",
                    Size = new Size(301, 20),
                    TabIndex = 5
                };
            // 
            // txtCustomerState
            // 
            txtCustomerState = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "State/Province:",
                    Location = new Point(5, 203),
                    Name = "txtCustomerState",
                    Size = new Size(301, 20),
                    TabIndex = 4
                };
            // 
            // txtCustomerCity
            // 
            txtCustomerCity = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "City:",
                    Location = new Point(5, 177),
                    Name = "txtCustomerCity",
                    Size = new Size(301, 20),
                    TabIndex = 3
                };
            // 
            // txtCustomerStreet
            // 
            txtCustomerStreet = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Street:",
                    Location = new Point(5, 74),
                    Multiline = true,
                    Name = "txtCustomerStreet",
                    Size = new Size(301, 97),
                    TabIndex = 2
                };

            // 
            // txtCustomerName
            // 
            txtCustomerName = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Customer name:",
                    Location = new Point(6, 48),
                    Name = "txtCustomerName",
                    Size = new Size(301, 20),
                    TabIndex = 0
                };
            // 
            // txtUserTwitter
            // 
            txtUserTwitter = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Twitter:",
                    Location = new Point(363, 170),
                    Name = "txtUserTwitter",
                    Size = new Size(301, 20),
                    TabIndex = 18
                };

            // 
            // txtUserWebAdress
            // 
            txtUserWebAdress = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Web address:",
                    Location = new Point(363, 144),
                    Name = "txtUserWebAdress",
                    Size = new Size(301, 20),
                    TabIndex = 17
                };

            // 
            // txtUserSkype
            // 
            txtUserSkype = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Skype:",
                    Location = new Point(363, 118),
                    Name = "txtUserSkype",
                    Size = new Size(301, 20),
                    TabIndex = 16
                };
            // 
            // txtUserEmail
            // 
            txtUserEmail = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "email:",
                    Location = new Point(363, 92),
                    Name = "txtUserEmail",
                    Size = new Size(301, 20),
                    TabIndex = 15
                };

            // 
            // txtUserMobile
            // 
            txtUserMobile = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Mobile:",
                    Location = new Point(363, 66),
                    Name = "txtUserMobile",
                    Size = new Size(301, 20),
                    TabIndex = 14
                };

            // 
            // txtUserPhone
            // 
            txtUserPhone = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Phone:",
                    Location = new Point(363, 40),
                    Name = "txtUserPhone",
                    Size = new Size(301, 20),
                    TabIndex = 13
                };

            // 
            // txtUserCountry
            // 
            txtUserCountry = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Country:",
                    Location = new Point(5, 247),
                    Name = "txtUserCountry",
                    Size = new Size(301, 20),
                    TabIndex = 12
                };

            // 
            // txtUserZip
            // 
            txtUserZip = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Zip/Postal Code:",
                    Location = new Point(5, 221),
                    Name = "txtUserZip",
                    Size = new Size(301, 20),
                    TabIndex = 11
                };

            // 
            // txtUserState
            // 
            txtUserState = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "State/Province:",
                    Location = new Point(5, 195),
                    Name = "txtUserState",
                    Size = new Size(301, 20),
                    TabIndex = 10
                };

            // 
            // txtUserCity
            // 
            txtUserCity = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "City:",
                    Location = new Point(5, 169),
                    Name = "txtUserCity",
                    Size = new Size(301, 20),
                    TabIndex = 9
                };

            // 
            // txtUserStreet
            // 
            txtUserStreet = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "Street:",
                    Location = new Point(5, 66),
                    Multiline = true,
                    Name = "txtUserStreet",
                    Size = new Size(301, 97),
                    TabIndex = 8
                };

            // 
            // txtUserName
            // 
            txtUserName = new NoLabelTextBox
                {
                    Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic),
                    ForeColor = SystemColors.InactiveCaption,
                    LabelText = "User Name:",
                    Location = new Point(6, 40),
                    Name = "txtUserName",
                    Size = new Size(301, 20),
                    TabIndex = 7
                };

            this.groupCustomer.Controls.Add(this.txtCustomerCountry);
            this.groupCustomer.Controls.Add(this.txtCustomerZip);
            this.groupCustomer.Controls.Add(this.txtCustomerState);
            this.groupCustomer.Controls.Add(this.txtCustomerCity);
            this.groupCustomer.Controls.Add(this.txtCustomerStreet);
            this.groupCustomer.Controls.Add(this.txtCustomerName);

            this.groupUserDetails.Controls.Add(this.txtUserTwitter);
            this.groupUserDetails.Controls.Add(this.txtUserWebAdress);
            this.groupUserDetails.Controls.Add(this.txtUserSkype);
            this.groupUserDetails.Controls.Add(this.txtUserEmail);
            this.groupUserDetails.Controls.Add(this.txtUserMobile);
            this.groupUserDetails.Controls.Add(this.txtUserPhone);
            this.groupUserDetails.Controls.Add(this.txtUserCountry);
            this.groupUserDetails.Controls.Add(this.txtUserZip);
            this.groupUserDetails.Controls.Add(this.txtUserState);
            this.groupUserDetails.Controls.Add(this.txtUserCity);
            this.groupUserDetails.Controls.Add(this.txtUserStreet);
            this.groupUserDetails.Controls.Add(this.txtUserName);

        }

        private bool handleCheckChanged = true;
        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (!handleCheckChanged)
                return;
            if (((RadioButton) sender).Equals(rbTemplateStandardLines))
                rbStandardLines.Checked = rbTemplateStandardLines.Checked;
            if (((RadioButton) sender).Equals(rbTemplateGroupedAnalysis))
                rbGroupedAnalysis.Checked = rbTemplateGroupedAnalysis.Checked;
            if (((RadioButton) sender).Equals(rbTemplateSimpleWordAnalysis))
                rbSimpleWordAnalysis.Checked = rbTemplateSimpleWordAnalysis.Checked;

            if (((RadioButton)sender).Equals(rbStandardLines))
            {
                _rates.BindRates(new StandardLinesTemplate());
                handleCheckChanged = false;
                rbTemplateStandardLines.Checked = rbStandardLines.Checked;
                handleCheckChanged = true;
            }

            if (((RadioButton) sender).Equals(rbGroupedAnalysis))
            {
                _rates.BindRates(new GroupedAnalysisTemplate());
                handleCheckChanged = false;
                rbTemplateGroupedAnalysis.Checked = rbGroupedAnalysis.Checked;
                handleCheckChanged = true;
            }
            if (((RadioButton) sender).Equals(rbSimpleWordAnalysis))
            {
                _rates.BindRates(new SimpleWordTemplate());
                handleCheckChanged = false;
                rbTemplateSimpleWordAnalysis.Checked = rbSimpleWordAnalysis.Checked;
                handleCheckChanged = true;
            }
        }

        private void ck_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Equals(ckWord))
                ddlWord.Visible = ckWord.Checked;
            if (((RadioButton)sender).Equals(ckExcel))
                ddlExcel.Visible = ckExcel.Checked;
        }

        private bool _saveCultrue = true;
        private void StudioInQuote_Load(object sender, EventArgs e)
        {
            txtProjectsXML.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Studio 2019\Projects");
            projectsList.SelectedIndexChanged += projectsList_SelectedIndexChanged;
            BindDDL(ddlWord, (new WordTemplates()).GetAllTemplates());
            BindDDL(ddlExcel, (new ExcelTemplates()).GetAllTemplates());
            _saveCultrue = false;
            BindDDL(ddlLanguages, GetAllAvailableLanguages());
            _saveCultrue = true;
            ddlLanguages.SelectedValue = Settings.GetSavedCulture();

            List<KeyValuePair<String, String>> projects = Projects.Projects.GetAllProjects(txtProjectsXML.Text).OrderBy(project => project.Value).ToList();
            projectsList.DisplayMember = "Value";
            projectsList.ValueMember = "Key";
            projectsList.DataSource = projects;

            cmbReportType.SelectedIndex = 0;
            FillLanguagePairDDL();
            FillReportTypeDDL();

            FillDefaultValues();
        }

        private void FillDefaultValues()
        {
            FillDefaultUser();
            LoadCurrentRatesState();
            //FillDefaultCustomer();
        }

        private void LoadCurrentRatesState()
        {
            List<TemplateRatesBase> rates = Settings.GetSavedRatesState();
            _rates.State = rates;
        }

        private void FillDefaultCustomer()
        {
            Customer customer = Settings.GetCurrentCustomer();
            if (customer != null)
            {
                txtCustomerName.SDLText = customer.Name;
                txtCustomerStreet.SDLText = customer.Street;
                txtCustomerCity.SDLText = customer.City;
                txtCustomerState.SDLText = customer.State;
                txtCustomerZip.SDLText = customer.Zip;
                txtCustomerCountry.SDLText = customer.Country;
            }
        }

        private void FillDefaultUser()
        {
            User user = Settings.GetCurrentUser();
            if (user != null)
            {
                txtUserName.SDLText = user.Name;
                txtUserStreet.SDLText =user.Street;
                txtUserCity.SDLText = user.City;
                txtUserState.SDLText = user.State;
                txtUserZip.SDLText = user.Zip;
                txtUserCountry.SDLText = user.Country;
                txtUserTwitter.SDLText = user.Twitter;
                txtUserWebAdress.SDLText = user.WebAddress;
                txtUserSkype.SDLText = user.Skype;
                txtUserEmail.SDLText = user.Email;
                txtUserMobile.SDLText = user.Mobile;
                txtUserPhone.SDLText = user.Phone;
            }
        }

        private void FillReportTypeDDL()
        {
            UIResources resources = new UIResources(Settings.GetSavedCulture());
            cmbReportType.Items.Clear();
            cmbReportType.Items.Add(resources.BreakdownAndSummary);
            cmbReportType.Items.Add(resources.Summary);
            cmbReportType.SelectedIndex = 0;
        }

        private void FillLanguagePairDDL()
        {
            if (projectsList.SelectedIndex < 0)
                return;
            String path = Projects.Projects.GetProjectFilePath(txtProjectsXML.Text, projectsList.SelectedValue.ToString());
            if (String.IsNullOrEmpty(path))
                return;

            String folder = String.Format("{0}\\Reports", Path.GetDirectoryName(path));
            if (!Directory.Exists(folder))
            {
                folder = String.Format("{0}\\{1}.ProjectFiles\\Reports", Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
                if (!Directory.Exists(folder)) return;
            }
            String[] analyseFiles = Directory.GetFiles(folder, "Analyze Files *.xml");

            var languagePairs = new List<KeyValuePair<string, string>>();
            var languagePairsInfo = new List<LanguagePairInfo>();
            foreach (var analyseFile in analyseFiles)
            {
                var fileName = Path.GetFileName(analyseFile);
                if (fileName != null)
                {
                    var creationTime = GetCreationTime(analyseFile);

                    String[] languages = fileName.Replace("Analyze Files ", "").Replace(".xml", "").Split('_');

                    var sourceCulture = GetCulture(languages[0]);
                    var destinationCulture = GetCulture(languages[1]);

                    languagePairsInfo.Add(new LanguagePairInfo()
                    {
                        AnalyseFile = analyseFile,
                        CreationDateAsDateTime = File.GetLastWriteTime(analyseFile),
                        Name = String.Format("{0} -> {1}", (new CultureInfo(sourceCulture)).Name, (new CultureInfo(destinationCulture)).Name),
                        CreationDate = creationTime
                    });
                }
            }
            var orderedLanguagePairsInfo = languagePairsInfo.OrderBy(pair => pair.Name).ThenBy(pair=>pair.CreationDateAsDateTime);
            foreach (var languagePairInfo in orderedLanguagePairsInfo)
            {
                String languagePair = String.Format("{0} [{1}]", languagePairInfo.Name , languagePairInfo.CreationDate);
                languagePairs.Add(new KeyValuePair<string, string>(languagePairInfo.AnalyseFile, languagePair));
            }

            BindDDL(cmbLanguagePair, languagePairs);
        }

        private string GetCreationTime(string analyseFile)
        {
            String creationTime = File.GetCreationTime(analyseFile).ToString(CultureInfo.InvariantCulture);
            var projectsFile = new XPathDocument(analyseFile);
            var nav = projectsFile.CreateNavigator();
            const string expression = "task/taskInfo";
            var taskInfoNodes = nav.Select(expression);
            while (taskInfoNodes.MoveNext())
            {
                var creationDate = taskInfoNodes.Current.SelectSingleNode("@runAt");
                if (creationDate != null)
                {
                    creationTime = creationDate.Value;
                    break;
                }
            }
            return creationTime;
        }
        private int GetLCID(string analyseFile)
        {
            string lcidValue = "1033";
            var projectsFile = new XPathDocument(analyseFile);
            var nav = projectsFile.CreateNavigator();
            const string expression = "task/taskInfo/language";
            var taskInfoNodes = nav.Select(expression);
            while (taskInfoNodes.MoveNext())
            {
                var lcid = taskInfoNodes.Current.SelectSingleNode("@lcid");
                if (lcid != null)
                {
                    lcidValue = lcid.Value;
                    break;
                }
            }
            return Convert.ToInt32(lcidValue);
        }
        private string GetCulture(string cultureName)
        {
            int indexOfOpenedBracket = cultureName.IndexOf('(');
            return indexOfOpenedBracket > 0 ? cultureName.Substring(0, indexOfOpenedBracket) : cultureName;
        }

        private List<KeyValuePair<String, String>> GetAllAvailableLanguages()
        {
            var assembly = Assembly.GetExecutingAssembly();
            String dir = Path.GetDirectoryName(assembly.Location);
            if (String.IsNullOrEmpty(dir))
                return new List<KeyValuePair<String, string>>();
            String[] languageFiles = Directory.GetFiles(dir, "*.resx");
            List<KeyValuePair<String, String>> cultures = new List<KeyValuePair<string, string>>();
            foreach (var languageFile in languageFiles)
            {
                string[] langs = languageFile.Split('.');
                String cultureID = langs[langs.Count() - 2];
                String cultureName = new CultureInfo(cultureID).NativeName.Split(' ')[0];
                cultures.Add(new KeyValuePair<string, string>(cultureID, cultureName));
            }
            return cultures;
        }

        private void BindDDL(ComboBox comboBox, List<KeyValuePair<String, String>> items)
        {
            comboBox.ValueMember = "Key";
            comboBox.DisplayMember = "Value";
            comboBox.DataSource = items;
        }

        private void ddlLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLanguages.SelectedValue != null)
            {
                String culture = ddlLanguages.SelectedValue.ToString();
                if (_saveCultrue)
                    Settings.SaveSelectedCulture(culture);
                EnsureUIBasedOnCultureInfo(culture);
                FillReportTypeDDL();

                _rates.UpdateStateBasedOnUICulture();
                if (rbStandardLines.Checked)
                    _rates.BindRates(new StandardLinesTemplate());
                if (rbGroupedAnalysis.Checked)
                    _rates.BindRates(new GroupedAnalysisTemplate());
                if (rbSimpleWordAnalysis.Checked)
                    _rates.BindRates(new SimpleWordTemplate());
            }
        }

        private void EnsureUIBasedOnCultureInfo(String culture)
        {
            var resources = new UIResources(culture);

            pageCreateQuote.Text = resources.CreateQuote;
            pageTemplates.Text = resources.Rates;
            pageUserDetails.Text = resources.UserDetails;
            groupCustomer.Text = resources.SelectCustomer;
            groupProjects.Text = resources.SelectProject;
            btnGenerate.Text = resources.GenerateQuote;
            groupReportType.Text = resources.ReportType;
            ckExcel.Text = resources.MSExcel;
            ckClipboard.Text = resources.Clipboard;
            ckWord.Text = resources.MSWord;
            txtCustomerName.LabelText = resources.CustomerName;
            txtCustomerStreet.LabelText = resources.Street;
            txtCustomerCountry.LabelText = resources.Country;
            txtCustomerZip.LabelText = resources.Zip;
            txtCustomerState.LabelText = resources.State;
            txtCustomerCity.LabelText = resources.City;
            groupStudioAnalysisBands.Text = resources.StudioAnalysisBands;
            rbGroupedAnalysis.Text = resources.GroupedAnalysis;
            rbStandardLines.Text = resources.StandardLines;
            rbSimpleWordAnalysis.Text = resources.SimpleWordAnalysis;
            rbTemplateGroupedAnalysis.Text = resources.GroupedAnalysis;
            rbTemplateStandardLines.Text = resources.StandardLines;
            rbTemplateSimpleWordAnalysis.Text = resources.SimpleWordAnalysis;
            groupUserDetails.Text = resources.UserDetails;
            txtUserTwitter.LabelText = resources.Twitter;
            txtUserWebAdress.LabelText = resources.WebAddress;
            txtUserSkype.LabelText = resources.Skype;
            txtUserEmail.LabelText = resources.Email;
            txtUserMobile.LabelText = resources.Mobile;
            txtUserPhone.LabelText = resources.Phone;
            txtUserCountry.LabelText = resources.Country;
            txtUserZip.LabelText = resources.Zip;
            txtUserState.LabelText = resources.State;
            txtUserCity.LabelText = resources.City;
            txtUserStreet.LabelText = resources.Street;
            txtUserName.LabelText = resources.UserName;
            btnCustomer.Text = resources.Customers;
            lblLanguagePair.Text = resources.LanguagePair;
            lblSummaryReportType.Text = resources.SummaryReportType;
            groupTemplateTemplates.Text = grpTemplateType.Text = resources.Templates;
            lblProjectsXML.Text = resources.ProjectsXMLPath;
        }

        void projectsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //String file = projectsList.SelectedValue.ToString();
            //txtCustomerName.SDLText = Projects.Projects.GetProjectCustomer(file);
            FillLanguagePairDDL();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            var customers = new CustomersDialog(ddlLanguages.SelectedValue.ToString());
            if (customers.ShowDialog() == DialogResult.OK && customers.SelectedCustomer != null)
            {
                txtCustomerName.SDLText = customers.SelectedCustomer.Name;
                txtCustomerStreet.SDLText = customers.SelectedCustomer.Street;
                txtCustomerCity.SDLText = customers.SelectedCustomer.City;
                txtCustomerState.SDLText = customers.SelectedCustomer.State;
                txtCustomerZip.SDLText = customers.SelectedCustomer.Zip;
                txtCustomerCountry.SDLText = customers.SelectedCustomer.Country;

                _rates.State = customers.SelectedCustomer.Rates;
                if (rbStandardLines.Checked)
                    _rates.BindRates(new StandardLinesTemplate(), false);
                if (rbGroupedAnalysis.Checked)
                    _rates.BindRates(new GroupedAnalysisTemplate(), false);
                if (rbSimpleWordAnalysis.Checked)
                    _rates.BindRates(new SimpleWordTemplate(), false);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!ckWord.Checked && !ckExcel.Checked && !ckClipboard.Checked)
                return;

            Settings.SaveRatesState(_rates.State);

            ITemplateRates currentSelectedTemplate = _rates.CurrentRatesTemplate;
            if (currentSelectedTemplate == null)
            {
                if (_rates == null || _rates.State == null)
                    currentSelectedTemplate = new SimpleWordTemplate();
                else
                {
                    currentSelectedTemplate = _rates.GetSimpleWordTemplateFromState();
                }
            }
            Project project = new Project(cmbLanguagePair.SelectedValue.ToString(),
                currentSelectedTemplate, 
                cmbReportType.SelectedIndex == 0 ? ReportType.Detailed : ReportType.Summary);

            Customer customer = new Customer(
                txtCustomerName.SDLText,
                txtCustomerStreet.SDLText,
                txtCustomerCity.SDLText,
                txtCustomerState.SDLText,
                txtCustomerZip.SDLText,
                txtCustomerCountry.SDLText);

            //Settings.SaveCurrentCustomer(customer);
            bool emptyUser = String.IsNullOrEmpty(txtUserName.SDLText);
            User user = new User()
                {
                    Name = emptyUser ? String.Empty : txtUserName.SDLText,
                    Street = emptyUser ? String.Empty : txtUserStreet.SDLText,
                    City = emptyUser ? String.Empty : txtUserCity.SDLText,
                    State = emptyUser ? String.Empty : txtUserState.SDLText,
                    Zip = emptyUser ? String.Empty : txtUserZip.SDLText,
                    Country = emptyUser ? String.Empty : txtUserCountry.SDLText,
                    Phone = emptyUser ? String.Empty : txtUserPhone.SDLText,
                    Mobile = emptyUser ? String.Empty : txtUserMobile.SDLText,
                    Email = emptyUser ? String.Empty : txtUserEmail.SDLText,
                    Skype = emptyUser ? String.Empty : txtUserSkype.SDLText,
                    WebAddress = emptyUser ? String.Empty : txtUserWebAdress.SDLText,
                    Twitter = emptyUser ? String.Empty : txtUserTwitter.SDLText
                };
            Settings.SaveCurrentUser(user);
            UIResources resources = new UIResources(ddlLanguages.SelectedValue.ToString());
            if (ckClipboard.Checked)
            {
                project.GenerateClipboardData(customer, user);
                MessageBox.Show(resources.QuoteGeneratedClipboard, resources.Information, MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            if (ckExcel.Checked)
            {
                saveFileDialog.FileName = String.Empty;
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = @"Excel Document|*.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    String fileToSave = saveFileDialog.FileName;
                    String template = ddlExcel.SelectedValue.ToString();

                    project.GenerateExcelData(customer, user, fileToSave, template);
                    MessageBox.Show(resources.QuoteGeneratedExcel, resources.Information, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            if (ckWord.Checked)
            {
                saveFileDialog.FileName = String.Empty;
                saveFileDialog.DefaultExt = "docx";
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = @"Word Document|*.docx";
                saveFileDialog.SupportMultiDottedExtensions = false;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    String fileToSave = saveFileDialog.FileName;
                    String template = ddlWord.SelectedValue.ToString();

                    project.GenerateWordData(customer, user, fileToSave, template);
                    MessageBox.Show(resources.QuoteGeneratedWord, resources.Information, MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }

        private void StudioInQuote_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_rates != null && _rates.State != null)
                Settings.SaveRatesState(_rates.State);
            bool emptyUser = String.IsNullOrEmpty(txtUserName.SDLText);
            User user = new User()
            {
                Name = emptyUser ? String.Empty : txtUserName.SDLText,
                Street = emptyUser ? String.Empty : txtUserStreet.SDLText,
                City = emptyUser ? String.Empty : txtUserCity.SDLText,
                State = emptyUser ? String.Empty : txtUserState.SDLText,
                Zip = emptyUser ? String.Empty : txtUserZip.SDLText,
                Country = emptyUser ? String.Empty : txtUserCountry.SDLText,
                Phone = emptyUser ? String.Empty : txtUserPhone.SDLText,
                Mobile = emptyUser ? String.Empty : txtUserMobile.SDLText,
                Email = emptyUser ? String.Empty : txtUserEmail.SDLText,
                Skype = emptyUser ? String.Empty : txtUserSkype.SDLText,
                WebAddress = emptyUser ? String.Empty : txtUserWebAdress.SDLText,
                Twitter = emptyUser ? String.Empty : txtUserTwitter.SDLText
            };
            Settings.SaveCurrentUser(user);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == pageTemplates)
                _rates.EnsureUI();
        }

        private void btnBrowseProjectsXML_Click(object sender, EventArgs e)
        {
            FolderSelectDialog selectFolder = new FolderSelectDialog();
            selectFolder.InitialDirectory = txtProjectsXML.Text;
            if (selectFolder.ShowDialog())
            {
                txtProjectsXML.Text = selectFolder.FileName;

                List<KeyValuePair<String, String>> projects = Projects.Projects.GetAllProjects(txtProjectsXML.Text).OrderBy(project => project.Value).ToList();
                projectsList.DisplayMember = "Value";
                projectsList.ValueMember = "Key";
                projectsList.DataSource = projects;
            }
        }
    }

    public class LanguagePairInfo
    {
        public String AnalyseFile { get; set; }
        public String Name { get; set; }
        public String CreationDate { get; set; }

        public DateTime CreationDateAsDateTime
        {
            get;
            set;
        }
    }
}
