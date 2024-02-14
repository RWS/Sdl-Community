using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using TMProvider;

namespace TradosPlugin
{
    public partial class SettingsForm : Form
    {

        /// <summary>
        /// The provider comes as a plain parameter from Trados -> no use to change the reference
        /// </summary>
        public MemoQTranslationProvider TranslationProvider { get; private set; }

        private List<TMListItem> filteredList;
        private List<TMListItem> tmGridSourceList;
        /// <summary>
        /// memoQ codes of project languages
        /// </summary>
        private List<string> projectLanguages;
        private string ltAccountCommand;

        private TMProviderBase currentTMProvider;
        /// <summary>
        /// Copy of the TM providers. The listbox always reflects this list.
        /// </summary>
        private List<TMProviderBase> localTMProviders;

        private bool changingAccount = false;
        private bool addingServer = false;

        private string savedUser;
        /// <summary>
        /// Saved password in plain (NON-encrypted) form.
        /// </summary>
        private string savedPwd;
        private LoginTypes savedLoginType;

        private GeneralSettingsControl generalSettingsControl;
        private StartingPanelControl startingPanel;
        private ProviderListPanel providerListPanel;

        private int currentIndexUnderContextMenu = -1;

        private Size largeSize;
        private Size smallSize = new Size(500, 240);
        private Color lineColor = Color.FromArgb(221, 221, 221);

        public SettingsForm()
        {
	        InitializeComponent();
            CenterToScreen();

            largeSize = Size;

            tmGridSourceList = new List<TMListItem>();

            Text = PluginResources.SettingsFormName;
            lblServerAccessDetails.Text = PluginResources.lblServerAccessDetails;
            lblTMs.Text = PluginResources.lblTMs;
            lblServers.Text = PluginResources.lblServers;
            lblServerName.Text = PluginResources.lblServerName;
            lblUserName.Text = PluginResources.lblUsername;
            lblPassword.Text = PluginResources.lblPassword;
            //lnkChangeAccount.Text = PluginResources.lnkChangeAccount;
            btnChangeAccount.Text = PluginResources.lnkChangeAccount;
            btnLogin.Text = PluginResources.lnkLogin;
            btnOpenLTAccount.Text = PluginResources.lnkOpenLT;
            colName.HeaderText = PluginResources.colName;
            colOwner.HeaderText = PluginResources.colOwner;
            colSource.HeaderText = PluginResources.colSource;
            colTarget.HeaderText = PluginResources.colTarget;
            colPermissions.HeaderText = PluginResources.colLTPermission;
            colSDLLookup.HeaderText = PluginResources.colSDLLookup;
            colSDLUpdate.HeaderText = PluginResources.colSDLUpdate;
            btnOK.Text = PluginResources.btnOK;
            btnCancel.Text = PluginResources.btnCancel;
            btnHelp.Text = PluginResources.btnHelp;
            rbLanguageTerminal.Text = PluginResources.rbLanguageTerminal;
            rbMemoQServer.Text = PluginResources.rbMemoQServer;
            rbWindows.Text = PluginResources.rbWindows;
            rbSSO.Text = PluginResources.rbSSO;
            lblFilter.Text = PluginResources.lblFilter;
            lblTargetLang.Text = PluginResources.lblTargetLang;
            lnkAddServer.Text = PluginResources.lnkAddServer;
            toolTip1.SetToolTip(pbAddMemoQServer, PluginResources.pbRemoveServerTooltip);
            //lblAuthenticationMethod.Text = PluginResources.lblAuthenticationMethod;
            lblGeneralSettings.Text = PluginResources.grpGeneralSettings + "  ▲";  // " ▲"; // U+25B2 BLACK UP-POINTING TRIANGLE
            // ▴ - U+25B4 SMALL BLACK UP-POINTING TRIANGLE
            lblNoTMs.Text = PluginResources.lblNoTMs;

            ltAccountCommand = PluginResources.LTAccountCommand;

            // fill the sorting methods for the columns
            // !!! if the columns change, change the sorting!
            sortData = new Sorting[7];
            sortData[0] = new Sorting(true, nameComparer);
            sortData[1] = new Sorting(true, ownerComparer);
            sortData[2] = new Sorting(true, sourceComparer);
            sortData[3] = new Sorting(true, targetComparer);
            sortData[4] = new Sorting(true, permissionComparer);
            sortData[5] = new Sorting(true, lookupComparer);
            sortData[6] = new Sorting(true, updateComparer);

            dgrTMs.AutoGenerateColumns = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="translationProvider">Null if there's no provider yet (new provider).</param>
        public SettingsForm(MemoQTranslationProvider translationProvider, List<string> prLangs)
            : this()
        {
            try
            {
                bool noProvidersYet = false;
                projectLanguages = prLangs;
                TranslationProvider = translationProvider;
                fillLanguageComboList();

                // when there's no data yet
                localTMProviders = new List<TMProviderBase>();

                // when user data comes from the credential store, and there's no provider yet
                if (TranslationProvider == null)
                {
                    TranslationProvider = new MemoQTranslationProvider(new GeneralTMSettings(true, false, false, false), new List<TMProviderBase>());
                    noProvidersYet = true;
                }
                else if (TranslationProvider.MemoQTMProviders != null && TranslationProvider.MemoQTMProviders.Count > 0)
                {
                    // copy providers
                    // if there's no LT provider in the list, add a fake one without data
                    foreach (TMProviderBase p in TranslationProvider.MemoQTMProviders) localTMProviders.Add(p.Clone());
                }
                else
                {
                    // provider list is empty
                    noProvidersYet = true;
                }


                generalSettingsControl = new GeneralSettingsControl(TranslationProvider.GeneralTMSettings.ConcordanceCaseSensitive,
                    TranslationProvider.GeneralTMSettings.ConcordancNumericEquivalence);
                pnlLeftSide.Controls.Add(generalSettingsControl);
                generalSettingsControl.Visible = false;
                generalSettingsControl.HeaderClicked += generalSettingsControl_HeaderClicked;

                startingPanel = new StartingPanelControl();
                Controls.Add(startingPanel);
                startingPanel.MemoQServerClicked += startingPanel_MemoQServerClicked;

                providerListPanel = new ProviderListPanel(localTMProviders, 100, gplAccessDetails.Height);
                pnlLeftSide.Controls.Add(providerListPanel);
                providerListPanel.BringToFront();
                providerListPanel.ProviderChosen += providerListPanel_ProviderChosen;
                providerListPanel.ProviderDeleted += providerListPanel_ProviderDeleted;
                providerListPanel.SizeChanged += providerListPanel_SizeChanged;
                resizeLstServers();

                int chosenItemIx = getItemToShow(localTMProviders);
                selectItemInList(chosenItemIx);

                // we don't know the name of the user
                myName = "My Name";

                if (noProvidersYet) showStartingPage();
            }

            catch (Exception e)
            {
                ExceptionHelper.ShowMessage(e); // + e.ToString());  
                Log.WriteToLog(e.ToString());
                clearOnException();
            }
        }

        void providerListPanel_SizeChanged(ProviderListPanel sender)
        {
            positionProviderList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            providerListPanel.Width = 100;
            providerListPanel.Top = 10 + 4;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (providerListPanel == null) return;
            if (gplTMServers == null) return;
            resizeLstServers();
        }

        private void providerListPanel_ProviderDeleted(TMProviderBase provider)
        {
            removeServer(provider);
        }

        private void providerListPanel_ProviderChosen(TMProviderBase provider)
        {
            changingAccount = false;
            savedPwd = savedUser = "";
            savedLoginType = LoginTypes.Undefined;
            txtFilter.Text = "";

            // save previous item
            if (currentTMProvider != null) saveProviderData(currentTMProvider);
            currentTMProvider = provider;
            // select item, fill data
            if (provider == null)
            {
                return;
            }

            if (!String.IsNullOrEmpty(provider.Settings.UserName) && !currentTMProvider.IsLoggedIn) login(currentTMProvider.Settings.URL, currentTMProvider.Settings.UserName,
                     StringEncoder.DecryptText(currentTMProvider.Settings.Password), currentTMProvider.Settings.LoginType, currentTMProvider);

            if (currentTMProvider != null) displayProvider(currentTMProvider, true);
            showAllExceptions(currentTMProvider);
        }


 

        private void startingPanel_MemoQServerClicked()
        {
            hideStartingPage();
            CenterToScreen();
            lnkAddServer_LinkClicked(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>True if the user accepted the certificate or there was nothing to accept.</returns>
        private bool askUserToAcceptCertificate(TMProviderMQServer provider)
        {
            if (provider == null) return true;
            if (provider.CertificateIsSelfSigned && !provider.CertificateAccepted)
            {
                DialogResult res = MessageBox.Show(String.Format(PluginResources.NoValidCertificate, provider.ProviderName, provider.CertificateData.IssuerName.Name), PluginResources.Error_Warning, MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    provider.AcceptCertificate(provider.CertificateData.Thumbprint);
                    return true;
                }
                else return false;
            }
            return true;
        }

        private int getItemToShow(List<TMProviderBase> providers)
        {
            if (providers == null || providers.Count == 0) return -1;

            // if it's not set up but there are no more items
            if (providers.Count == 1) return -1;
            // else return first memoQ server
            return 1;
        }

        /// <summary>
        /// Tries to log the user into the chosen server. If succeeded, the provider is added to the local list of providers, or updated in the list 
        /// if it's already there.
        /// </summary>
        /// <param name="url">Null or empty for LT.</param>
        /// <param name="username"></param>
        /// <param name="password">In the form it is need for login (NOT encrypted).</param>
        /// <param name="provider">The provider to log in to. If null, there's no provider yet.</param>
        private void login(string url, string username, string password, LoginTypes loginType, TMProviderBase provider, bool showErrors = true)
        {
            bool isLT = false;
            bool acceptedCert = false;
            if (String.IsNullOrEmpty(url))
            {
                isLT = true;
            }
            try
            {
                string displTitle;

                // memoQ server

                if (loginType == LoginTypes.WindowsIntegrated)
                {
                    username = password = "";
                }
                if (provider != null) provider.Login(username, StringEncoder.EncryptText(password), loginType);
                else
                {
                    provider = new TMProviderMQServer(url, username, StringEncoder.EncryptText(password), loginType);
                    provider.Login(provider.Settings.UserName, provider.Settings.Password, provider.Settings.LoginType);
                }
                displTitle = url + "/" + username;

                lblLTAccountData.Text = displTitle;

                if (provider.IsLoggedIn)
                {
                    // if everything is OK, add provider
                    hideLoginError(isLT);
                    if (addingServer)
                    {
                        int ix = addProviderToList(provider);
                        selectItemInList(ix);
                    }
                    else
                    {
                        currentTMProvider = provider;
                        // this not a good idea here, display it after login
                        //displayProvider(provider);
                        providerListPanel.RefreshProvider(provider);
                    }
                    changingAccount = false;
                    return;
                }

                // if login was not successful, it might have been because the server has a self-signed certificate
                // if this is the case, ask them: if they reject it, then don't add the server / leave it there without logging in
                // if they accept: try to log in again
                if (provider is TMProviderMQServer && (provider as TMProviderMQServer).CertificateIsSelfSigned && !(provider as TMProviderMQServer).CertificateAccepted)
                {
                    acceptedCert = askUserToAcceptCertificate(provider as TMProviderMQServer);
                    if (!acceptedCert && addingServer)
                    {
                        int ix = getItemToShow(localTMProviders);
                        selectItemInList(ix);
                        return;
                    }
                    else
                    {
                        // try to log in again if needed - recursive call
                        login(url, username, password, loginType, provider);
                        return;
                    }
                }

                // now we are at the stage where login failed, or the certificate was rejected
                if (addingServer)
                {

                    showLoginPage(txtServerName.Text, txtUsername.Text, txtPassword.Text);
                    if (provider.ExceptionList.Any(e => e is AuthenticationException)) showLoginError();
                    if (showErrors && provider.ExceptionList.Any(e => !(e is AuthenticationException))) showAllExceptions(provider);
                    //if (showErrors) showAllExceptions(provider);
                    provider.ClearExceptions();
                }
                // simple login failed
                else
                {
                    showLogin(provider, false, isLT, false);
                    int c = 0;
                    // if there were any login errors
                    if (showErrors && (c = provider.ExceptionList.Count(e => e is AuthenticationException)) != 0) showLoginError();
                    // if there are any other errors than login errors
                    if (showErrors && c != provider.ExceptionList.Count) showAllExceptions(provider);
                    else provider.ClearExceptions();
                    clearTMs();
                    //providerListPanel.RefreshProvider(provider);
                }
            }

            catch (Exception e)
            {
                // other exceptions: only clear TMs list & settings
                clearTMs();
                if (e is AuthenticationException)
                {
                    if (addingServer) showLoginPage(txtServerName.Text, txtUsername.Text, txtPassword.Text);
                    else showLogin(provider, false, isLT, false);
                    txtPassword.Text = password;
                    txtUsername.Text = username;
                    txtServerName.Text = url;
                    if (showErrors) showLoginError();
                }
                if (showErrors) ExceptionHelper.ShowMessage(e, currentTMProvider != null ? currentTMProvider.ProviderName : "");
            }
        }

        private bool checkPasswordField(bool checkServerURL)
        {
            if (checkServerURL && String.IsNullOrEmpty(txtServerName.Text))
            {
                MessageBox.Show(this, PluginResources.Error_NoServername, PluginResources.Error_Warning);
                txtServerName.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show(this, PluginResources.Error_NoUsername, PluginResources.Error_Warning);
                txtUsername.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show(this, PluginResources.Error_NoPassword, PluginResources.Error_Warning);
                txtPassword.Focus();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks the URL for protocol. Adds https if missing, corrects http to https, and adds 8080 port number if missing.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="correctedURL"></param>
        /// <returns>False URL is still not OK.</returns>
        private bool checkAndCorrectURL(string url, out string correctedURL)
        {
            correctedURL = url;
            // url is empty for LT
            if (String.IsNullOrEmpty(url)) return true;
            string newURL = correctServerURL(url);
            Uri uriResult;
            bool result = Uri.TryCreate(newURL, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!result)
            {
                string message = ExceptionHelper.GetExceptionMessage(new UriFormatException());
                errorProvider.SetError(txtServerName, message);
                return false;
            }

            // url doesn't contain port number - Uri.Port returns default port if not defined
            if (!newURL.Contains(uriResult.Port.ToString()))
            {
                correctedURL = uriResult.Scheme + Uri.SchemeDelimiter + uriResult.Host + ":8080"
                    + (uriResult.PathAndQuery != "/" ? uriResult.PathAndQuery : "");
            }
            else correctedURL = newURL;

            return true;
        }

        /// <summary>
        /// Corrects the URL from http to https, or adds the https protocol if it was missing.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string correctServerURL(string url)
        {
            string newURL = url;
            // for LT the URL is empty
            if (String.IsNullOrEmpty(url)) return newURL;

            if (url.StartsWith("http:/"))
            {
                newURL = url.Replace("http", "https");
            }
            else if (!url.StartsWith("https://"))
            {
                newURL = "https://" + url;
            }
            return newURL;
        }

        /// <summary>
        /// Adds the provider to the list, or updates it if there's already one on the list with the same URL. Returns the index where it was inserted.
        /// </summary>
        /// <param name="p"></param>
        private int addProviderToList(TMProviderBase p)
        {
            addingServer = false;

            // if the provider (url) is already on the list -> replace
            int ix;
            if ((ix = localTMProviders.FindIndex(prov => prov.Settings.URL == p.Settings.URL)) != -1)
            {
                localTMProviders[ix] = p;
                //lstServers.SelectedIndex = lstServers.Items.Count - 1;
            }
            // if not, add in alphabetical order
            else
            {
                // find place: the first one where p's name is before the item
                ix = localTMProviders.FindIndex(1, prov => p.ProviderName.CompareTo(prov.ProviderName) < 0);
                if (ix == -1)
                {
                    localTMProviders.Add(p);
                    // last one (this current one) will be selected)
                    ix = localTMProviders.Count - 1;
                }
                else localTMProviders.Insert(ix, p);

                // refresh list
                providerListPanel.RefreshProviderList(localTMProviders);
            }
            // current one should be selected
            selectItemInList(ix);
            providerListChanged(localTMProviders);
            return ix;

        }

        private void showTMs(TMProviderBase provider, List<string> prLangs)
        {
            try
            {
                TMInfo[] ltTMs = provider.ListTMs(TranslationProvider.GeneralTMSettings.StrictSublanguageMatching, prLangs);
                // filter for only project languages
                lblNoTMs.Visible = false;
                fillTMGridSource(ltTMs.ToList());
                filteredList = tmGridSourceList;
                dgrTMs.DataSource = null;
                //dgrTMs.Refresh();
                dgrTMs.DataSource = tmGridSourceList;

                // try to refresh currencymanager not to have index -1 does not have a value exception (currencymanager.get_current - indexoutofrange)
                CurrencyManager cm = (CurrencyManager)dgrTMs.BindingContext[tmGridSourceList];
                if (cm != null) cm.Refresh();
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowMessage(e);
            }
        }

        private void clearTMs()
        {
            tmGridSourceList.Clear();
            dgrTMs.DataSource = null;
            dgrTMs.Refresh();
            dgrTMs.DataSource = tmGridSourceList;
        }

        private void clearOnException()
        {
            clearTMs();
            hideStartingPage();
            if (currentTMProvider == null)
            {
                //showEmptyPage();
                return;
            }
            showLogin(currentTMProvider, false, false, false);
        }

        private void changeAccount()
        {
            if (currentTMProvider == null)
            {
                ExceptionHelper.ShowMessage(new InvalidOperationException("The current TMProvider is empty. Please, click on a server on the list or add a new server."));
                //showStartingPage();
                return;
            }

            saveProviderData(currentTMProvider);

            bool isLT = false;
            changingAccount = !changingAccount;

            // user wants to change account
            if (changingAccount)
            {
                // save user data to be restored on undo
                savedUser = currentTMProvider.Settings.UserName;
                savedPwd = StringEncoder.DecryptText(currentTMProvider.Settings.Password);
                savedLoginType = currentTMProvider.Settings.LoginType;

                hideLoginError(isLT);
                showLogin(currentTMProvider, false, isLT, true);
            }
            else
            {
                // user clicked on "undo change account"
                // the old data for the current provider is saved at login - now only if it's successful
                if (!String.IsNullOrEmpty(savedUser))
                {
                    login(isLT ? "" : currentTMProvider.Settings.URL, savedUser, savedPwd, savedLoginType, currentTMProvider, false);

                    currentTMProvider.Settings.UserName = savedUser;
                    currentTMProvider.Settings.Password = StringEncoder.EncryptText(savedPwd);
                    currentTMProvider.Settings.LoginType = savedLoginType;

                    displayProvider(currentTMProvider, true);
                }
                // if the user wants to undo before login, there's no data, and we only have to hide login
                else
                {
                    string title;
                    if (isLT) title = currentTMProvider.Settings.UserName;
                    else title = currentTMProvider.Settings.URL + "/" + currentTMProvider.Settings.UserName;
                    hideLogin();
                }
                savedUser = savedPwd = "";
                savedLoginType = LoginTypes.Undefined;
            }
        }

        private void btnChangeAccount_Click(object sender, EventArgs e)
        {
            changeAccount();
        }

        /// <summary>
        /// Only save data if provider is logged in. Otherwise the user is not able to change anything, and we have to keep her settings.
        /// See #27096
        /// </summary>
        /// <param name="provider"></param>
        private void saveProviderData(TMProviderBase provider)
        {
            // #27096 - don't change data if not logged in
            if (!provider.IsLoggedIn) return;

            provider.ClearUsedTMs();
            TMInfo[] usedTMs = provider.GetUsedTMs();
            foreach (TMListItem item in tmGridSourceList)
            {
                TMPurposes p = TMPurposes.Undefined;
                if (item.TMInfo.AccessLevel != ResourceAccessLevel.ReadOnly && item.SDLLookup && item.SDLUpdate) p = TMPurposes.LookupUpdate;
                else if (item.SDLLookup) p = TMPurposes.Lookup;
                else if (item.TMInfo.AccessLevel != ResourceAccessLevel.ReadOnly && item.SDLUpdate) p = TMPurposes.Update;
                // don't add: unnecessarily long list and also #27108
                else p = TMPurposes.None;
                item.TMInfo.Purpose = p;

                if (p != TMPurposes.None && p != TMPurposes.Undefined) provider.AddUsedTM(item.TMInfo);
                providerListPanel.RefreshProvider(provider);
            }
            // try to create URI just to make sure Trados won't complain
            try
            {
                List<MemoQTMSettings> settings = localTMProviders.Select(tm => tm.Settings).ToList();
                GeneralTMSettings generalSettings = new GeneralTMSettings(true, false, generalSettingsControl.ConcordanceCaseSensitive, generalSettingsControl.ConcordanceNumericEquiv);
                Uri wholeURI = SettingsURICreator.CreateURIFromSettings(generalSettings, settings);
            }
            catch
            {
                ExceptionHelper.ShowMessage(String.Format("There are too many TMs checked for {0}. Please, select your TMs again.", provider.ProviderName));
                provider.ClearUsedTMs();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // only some controls exist for general settings !!! create default
            GeneralTMSettings generalSettings = new GeneralTMSettings(true, false, generalSettingsControl.ConcordanceCaseSensitive, generalSettingsControl.ConcordanceNumericEquiv);
            // save the currently open provider
            if (currentTMProvider != null) saveProviderData(currentTMProvider);
            // don't change the reference for the translation provider, as it comes as a plain parameter from Trados for editing
            TranslationProvider.GeneralTMSettings = generalSettings;
            // copy local provider list
            TranslationProvider.MemoQTMProviders = new List<TMProviderBase>();
            int i = 0;
            
            while (i < localTMProviders.Count)
            {
                TranslationProvider.AddMemoQProvider(localTMProviders[i]);
                i++;
            }
            // display exceptions if there are any
            showAllExceptions(currentTMProvider);

            DialogResult = DialogResult.OK;
            Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tryLogin()
        {
            if (!checkPasswordField(addingServer)) return;

            string newURL;
            if (!checkAndCorrectURL(txtServerName.Text, out newURL))
            {
                txtServerName.Focus();
                return;
            }

            txtServerName.Text = newURL;

            login(txtServerName.Text, txtUsername.Text, txtPassword.Text, getLoginType(), currentTMProvider);
            if (currentTMProvider != null) displayProvider(currentTMProvider, false);
            //showAllExceptions(currentTMProvider);
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tryLogin();
        }

        private void fillTMGridSource(List<TMInfo> allTMs)
        {
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("TradosPlugin.Languages", typeof(Languages).Assembly);
            tmGridSourceList.Clear();
            if (currentTMProvider == null) throw new InvalidOperationException("The currentTMProvider is null");

            // need a whole list of TMs
            foreach (TMInfo tm in allTMs)
            {
                string s = resourceManager.GetString(tm.SourceLanguageCode);
                string t = resourceManager.GetString(tm.TargetLanguageCode);
                string operation = tm.AccessLevel <= ResourceAccessLevel.ReadOnly ? PluginResources.Lookup : PluginResources.Update;
                // find matching TM in the used TMs
                TMInfo usedTM = currentTMProvider.GetUsedTMs().FirstOrDefault(tminfo => tminfo.TMGuid == tm.TMGuid);
                if (usedTM == null) usedTM = tm;
                bool lookup = usedTM.Purpose == TMPurposes.Undefined || usedTM.Purpose == TMPurposes.None || usedTM.Purpose == TMPurposes.Update ? false : true;
                bool update = usedTM.Purpose == TMPurposes.Undefined || usedTM.Purpose == TMPurposes.None || usedTM.Purpose == TMPurposes.Lookup ? false : true;
                tmGridSourceList.Add(new TMListItem(usedTM, usedTM.FriendlyName, usedTM.TMOwner, s, t, operation, lookup, update));
            }
            tmGridSourceList.Sort((x, y) => ownerComparer(x, y));
        }

        private void fillLanguageComboList()
        {
            System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("TradosPlugin.Languages", typeof(Languages).Assembly);
            resourceManager.IgnoreCase = true;
            cmbTargetLang.Items.Add("");
            foreach (string code in projectLanguages)
            {
                string t = resourceManager.GetString(code);
                if (!String.IsNullOrEmpty(t)) cmbTargetLang.Items.Add(t);
                // add main language as well
                if (code.Length > 3) t = resourceManager.GetString(code.Substring(0, 3));
                if (!String.IsNullOrEmpty(t) && !cmbTargetLang.Items.Contains(t)) cmbTargetLang.Items.Add(t);
            }
            cmbTargetLang.SelectedIndex = 0;
        }

        /// <summary>
        /// Hides the login controls and displays the name instead.
        /// </summary>
        /// <param name="accountTitle">The name that will be displayed.</param>
        private void hideLogin()
        {
            //lnkChangeAccount.Text = PluginResources.lnkChangeAccount;
            btnChangeAccount.Text = PluginResources.lnkChangeAccount;
            btnChangeAccount.Visible = true;
            pnlServer.Visible = false;
            pnlLoginFields.Visible = false;
            pnlLogin.Height = lblLTAccountData.Bottom + 4;
            pnlTMs.Visible = true;
            pnlLogin.Visible = true;
            pnlTMs.Top = pnlLogin.Bottom + 4;
            pnlTMs.Height = btnOK.Top - 16 - pnlTMs.Top;
        }

        /// <summary>
        /// Shows the login controls.
        /// </summary>
        /// <param name="showServerName"></param>
        /// <param name="isLT"></param>
        private void showLogin(TMProviderBase provider, bool showServerName, bool isLT, bool resetTxtFields)
        {
            if (changingAccount) btnChangeAccount.Text = PluginResources.lnkUndoChangeAccount;
            else btnChangeAccount.Visible = false;

            pnlServer.Visible = showServerName;
            pnlLoginFields.Visible = true;
            pnlLogin.Visible = true;
            gplAccessDetails.Visible = true;

            if(resetTxtFields) txtServerName.Text = txtUsername.Text = txtPassword.Text = "";
            if (isLT)
            {
                pnlLoginType.Visible = false;
                //lblAuthenticationMethod.Visible = false;
                pnlLoginFields.Height = txtPassword.Bottom + 4;
                pnlLogin.Height = txtPassword.Bottom + 4; // pnlLoginFields

            }
            else
            {
                pnlLoginType.Visible = true;
                pnlLoginFields.Height = pnlLoginType.Bottom + 4;
                pnlLogin.Height = pnlLoginFields.Bottom + 4;
            }

            pnlLogin.Height = pnlLoginFields.Bottom + 4;
            if (showServerName)
            {
                pnlServer.Top = lblLTAccountData.Bottom + 4;
                pnlLoginFields.Top = pnlServer.Bottom;
            }
            else
            {
                pnlLoginFields.Top = lblLTAccountData.Bottom + 4;
            }
            pnlTMs.Top = pnlLogin.Bottom + 4;
            pnlTMs.Height = btnOK.Top - 16 - pnlTMs.Top;
            //hideLoginError(isLT);
            if (resetTxtFields && provider != null && !String.IsNullOrEmpty(provider.Settings.UserName))
            {
                txtUsername.Text = provider.Settings.UserName;
                txtPassword.Text = StringEncoder.DecryptText(provider.Settings.Password);
                if (!isLT) txtServerName.Text = provider.Settings.URL;
                else if (provider.Settings.LoginType == LoginTypes.Windows) rbWindows.Checked = true;
                else rbMemoQServer.Checked = true;
            }
            txtUsername.Focus();
        }

        private void showLoginError()
        {
            string serverName;
            if (currentTMProvider == null) serverName = txtServerName.Text;
            else serverName = currentTMProvider.ProviderName;
            errorProvider.SetError(txtPassword, String.Format(PluginResources.Error_InvalidCredentialsText, serverName));
        }

        private void hideLoginError(bool showOpenLT)
        {
            errorProvider.SetError(txtPassword, String.Empty);
        }

        /// <summary>
        /// Displays the provider's data: TMs, etc. 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>True if login succeeded, false if there was a problem.</returns>
        private bool displayProvider(TMProviderBase provider, bool overwriteTextfields)
        {
            try
            {
                // if the user could not log in, we display the login fields, and still keep the undo changing account button
                if(provider.IsLoggedIn) changingAccount = false;
                addingServer = false;
                bool tryLogin = !String.IsNullOrEmpty(provider.Settings.UserName);
                TMProviderMQServer mqserver = provider as TMProviderMQServer;
                // only login if the user has just accepted the certificate
                // if she was not able to log in, it is surely not self-signed
                if (mqserver != null && mqserver.CertificateIsSelfSigned && mqserver.CertificateAccepted && !provider.IsLoggedIn && tryLogin)
                {
                    //askUserToAcceptCertificate(provider as TMProviderMQServer);
                    provider.Login(provider.Settings.UserName, provider.Settings.Password, provider.Settings.LoginType);
                }
                // display TMs
                if (provider.IsLoggedIn && !String.IsNullOrEmpty(provider.Settings.UserName)) showTMs(provider, projectLanguages);
                else clearOnException();
                // hide the little page with the 2 buttons
                hideStartingPage();
                bool isLT = false;
                // hide the label with the Your translation memories... text
                hideNoTMsForServer();
                // show all controls
                showDetailsPage(isLT, overwriteTextfields);
                if (provider.IsLoggedIn) hideLogin();
                else if (isLT)
                {
                    txtUsername.Focus();
                    showNoTMsLabelInDgr();
                }
                lblServerAccessDetails.Text = provider.ProviderName + " - " + PluginResources.lblServerAccessDetails;

                lblTMs.Text = provider.ProviderName + " - " + PluginResources.lblTMs;
                string displName;
                if (isLT) displName = provider.Settings.UserName;
                else displName = provider.Settings.URL + "/" + provider.Settings.UserName;
                lblLTAccountData.Text = displName;
                if (!isLT && (provider as TMProviderMQServer).CertificateIsSelfSigned) lblWarning.Text = PluginResources.ConnectionNotSecure;
                else lblWarning.Text = "";
                // show/hide Open my LT page
                showOrHideOpenLT();
                providerListPanel.RefreshProvider(provider);
                return true;
            }
            catch (Exception e)
            {
                clearOnException();
                ExceptionHelper.ShowMessage(e);
                return false;
            }
        }

        private void selectItemInList(int index)
        {
            providerListPanel.SelectItem(index);
            if (providerListPanel.SelectedItem != null)
            {
                currentTMProvider = providerListPanel.SelectedItem.Provider;
                if (!currentTMProvider.IsLoggedIn && !String.IsNullOrEmpty(currentTMProvider.Settings.UserName))
                    login(currentTMProvider.Settings.URL, currentTMProvider.Settings.UserName,
                     StringEncoder.DecryptText(currentTMProvider.Settings.Password), currentTMProvider.Settings.LoginType, currentTMProvider);

                displayProvider(providerListPanel.SelectedItem.Provider, true);
            }
        }

        private void openLTPage()
        {
            return;
        }

        private void showLoginPage(string server, string user, string pwd)
        {
            txtServerName.Text = server;
            txtUsername.Text = user;
            txtPassword.Text = pwd;
            hideStartingPage();
            lblServerAccessDetails.Text = PluginResources.lblServerAccessDetails_AddServer;
            hideLoginError(false);
            lblLTAccountData.Text = "";
            lblLTAccountData.Visible = false;
            pnlTMs.Visible = false;
            pnlServer.Visible = true;
            pnlLoginFields.Visible = true;
            pnlLogin.Visible = true;
            //lnkChangeAccount.Visible = false;
            btnChangeAccount.Visible = false;
            pnlLoginType.Visible = true;
            pnlServer.Top = gplAccessDetails.Bottom + 4;
            pnlLoginFields.Top = pnlServer.Bottom;
            pnlTMs.Top = pnlLogin.Bottom;
            pnlTMs.Height = btnOK.Top - 16 - pnlTMs.Top;
            pnlTMs.Width = pnlLogin.Width;
            pnlLoginFields.Height = pnlLoginType.Bottom + 4;
            pnlLogin.Height = pnlLoginFields.Bottom + 4;
        }

        private void showStartingPage()
        {
            Size = smallSize;
            CenterToScreen();
            startingPanel.Location = new Point(0, 0);
            startingPanel.Width = Width;
            startingPanel.Height = Height;
            startingPanel.Visible = true;
            startingPanel.BringToFront();
        }

        private void hideStartingPage()
        {
            Size = largeSize;
            //this.Location = new Point(200, 100);
            startingPanel.Width = 0;
            startingPanel.Height = 0;
            startingPanel.Visible = false;
            startingPanel.SendToBack();
        }

        private void showDetailsPage(bool isLT, bool overwriteTextFields)
        {
            lblServerAccessDetails.Text = PluginResources.lblServerAccessDetails;
            lblLTAccountData.Visible = true;
            if(currentTMProvider != null && currentTMProvider.IsLoggedIn) hideLoginError(isLT);
            showLogin(currentTMProvider, false, isLT, overwriteTextFields);
            pnlLoginFields.Height = isLT ? txtPassword.Bottom + 4 : pnlLoginType.Bottom;
            pnlLogin.Height = pnlLoginFields.Bottom + 4;
            pnlTMs.Visible = true;
            pnlTMs.Top = pnlLogin.Bottom + 4;
            pnlTMs.Height = btnOK.Top - 16 - pnlTMs.Top;
        }

        private void showNoTMsLabelInDgr()
        {
            lblNoTMs.Visible = true;
            lblNoTMs.BringToFront();
            lblNoTMs.Location = new Point(dgrTMs.Left + 4, dgrTMs.Top + 20);
            lblNoTMs.Height = dgrTMs.Height - 40;
            lblNoTMs.Width = dgrTMs.Width - 8;
            lblNoTMs.BorderStyle = BorderStyle.None;
        }

        private void showNoTMsForServer()
        {
            pnlTMs.Visible = true;
            foreach (Control c in pnlTMs.Controls)
            {
                c.Visible = false;
            }
            lblNoTMs.Visible = true;
            lblNoTMs.Location = new Point(0, 0);
            lblNoTMs.Height = pnlTMs.Height - 20;
            lblNoTMs.Width = pnlTMs.Width - 20;
            lblNoTMs.BorderStyle = BorderStyle.None;
            pnlVerticalLine.Visible = true;
        }

        private void hideNoTMsForServer()
        {
            pnlTMs.Visible = true;
            foreach (Control c in pnlTMs.Controls)
            {
                c.Visible = true;
            }
            lblNoTMs.Visible = false;

            lblNoTMs.Size = new Size(dgrTMs.Width - 30, dgrTMs.Height - 40);
            lblNoTMs.Left = dgrTMs.Left + (dgrTMs.Width - lblNoTMs.Width) / 2;
            lblNoTMs.Top = dgrTMs.Top + (dgrTMs.Height - lblNoTMs.Height) / 2;
            lblNoTMs.BorderStyle = BorderStyle.None;
            pnlVerticalLine.Visible = false;
        }

        private void showOrHideOpenLT()
        {
            btnOpenLTAccount.Visible = false;
        }

        private void positionProviderList()
        {
            providerListPanel.Top = 10 + 4;
            providerListPanel.Left = 15;
            pbAddMemoQServer.Top = providerListPanel.Bottom + 8;
            lnkAddServer.Top = pbAddMemoQServer.Bottom - lnkAddServer.Height;
        }

        private LoginTypes getLoginType()
        {
            if (rbMemoQServer.Checked) return LoginTypes.MemoQServer;
            if (rbLanguageTerminal.Checked) return LoginTypes.MemoQServerLT;
            if (rbWindows.Checked) return LoginTypes.Windows;
            if (rbSSO.Checked) return LoginTypes.WindowsIntegrated;
            return LoginTypes.Undefined;
        }

        #region Server list things

        private void lstServers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            // is the current item LT and not configured
            bool isLTAndNotConfigured = false;
            if (e.Index == 0)
            {
                if (localTMProviders == null || localTMProviders[0] == null || String.IsNullOrEmpty(localTMProviders[0].Settings.UserName))
                    isLTAndNotConfigured = true;
            }
            //if the item state is selected them change the back color 
            // white background, blue letters
            Color foreColor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if (isLTAndNotConfigured) foreColor = Color.FromArgb(20, 99, 130);
                else foreColor = Color.Black; // Color.DarkSlateBlue;
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          Color.White, // Color.FromArgb(100, 130, 155), // Color.White,DarkSlateBlue
                                          Color.FromArgb(221, 221, 221));
                // Color.FromArgb(100, 130, 155)); // dark blue-grayish
                // Color.FromArgb(175, 180, 210)); // - dark greyish, but lighter
                // TextRenderer.DrawText(e.Graphics, lstServers.Items[e.Index].ToString(), this.Font, e.Bounds, Color.DarkSlateBlue, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }
            else
            {
                // if it's not selected: white background, black letters
                // if not logged in: gray
                // if not logged in and LT: bluish
                if (isLTAndNotConfigured) foreColor = Color.FromArgb(20, 99, 130);
                else foreColor = localTMProviders[e.Index].IsLoggedIn ? Color.Black : Color.Gray;
                e = new DrawItemEventArgs(e.Graphics,
                                             e.Font,
                                             e.Bounds,
                                             e.Index,
                                             e.State,
                                             foreColor, // Color.White,
                                             Color.White); // Color.FromArgb(175, 180, 210));
                // TextRenderer.DrawText(e.Graphics, lstServers.Items[e.Index].ToString(), this.Font, e.Bounds, Color.Black, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            }

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            //Font currentFont = e.Font;
            if (isLTAndNotConfigured) //currentFont = new System.Drawing.Font(e.Font.FontFamily, e.Font.Size, FontStyle.Bold);
            {
                // for LT we show the item in bold
                e.Graphics.DrawString(ltAccountCommand, new Font(e.Font.FontFamily, e.Font.Size, FontStyle.Bold), new SolidBrush(foreColor),
                    new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + 5), e.Bounds.Size), StringFormat.GenericDefault);
            }
            else
            {
                e.Graphics.DrawString((sender as ListBox).Items[e.Index].ToString(), e.Font, new SolidBrush(foreColor),
                        new Rectangle(new Point(e.Bounds.X, e.Bounds.Y + 5), e.Bounds.Size), StringFormat.GenericDefault);

            }
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();

            //currentFont.Dispose();
        }

        private void resizeLstServers()
        {
            // resize listbox
            providerListPanel.SetSize(100);
            positionProviderList();
        }

        private void providerListChanged(List<TMProviderBase> providers)
        {
            providerListPanel.RefreshProviderList(providers);

            resizeLstServers();
        }


        private void lnkAddServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            addingServer = true;
            // save previous item
            if (currentTMProvider != null) saveProviderData(currentTMProvider);
            changingAccount = false;
            currentTMProvider = null;
            showOrHideOpenLT();
            providerListPanel.SelectItem(-1);
            showLoginPage("", "", "");
            txtServerName.Focus();
            showNoTMsForServer();
        }

        #endregion



        #region Classes for list items and sorting

        private class TMListItem
        {
            public string TMName { get; private set; }
            public string Owner { get; private set; }
            public string SourceLang { get; private set; }
            public string TargetLang { get; private set; }
            public string LTPermission { get; private set; }
            public bool SDLLookup { get; set; }
            public bool SDLUpdate { get; set; }
            [Browsable(false)]
            public TMInfo TMInfo { get; private set; }

            public TMListItem(TMInfo tmInfo, string tmName, string owner, string sourceLang, string targetLang, string ltPermission, bool sdlLookup, bool sdlUpdate)
            {
                TMInfo = tmInfo;
                TMName = tmName;
                Owner = owner;
                SourceLang = sourceLang;
                TargetLang = targetLang;
                LTPermission = ltPermission;
                SDLLookup = sdlLookup;
                SDLUpdate = sdlUpdate;
            }
        }

        private class Sorting
        {
            public bool Ascending;
            public SortMethod SortMethod;

            public Sorting(bool asc, SortMethod sortMethod)
            {
                Ascending = asc;
                SortMethod = sortMethod;
            }
        }


        #endregion

        #region Sorting the datagridview

        private void dgrTMs_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (tmGridSourceList.Count == 0) return;
            if (filteredList == null || filteredList.Count == 0) filteredList = tmGridSourceList;

            if (sortData[e.ColumnIndex].Ascending) filteredList.Sort((x, y) => sortData[e.ColumnIndex].SortMethod(x, y));
            else filteredList.Sort((x, y) => sortData[e.ColumnIndex].SortMethod(y, x));
            // change sort order
            sortData[e.ColumnIndex].Ascending = !sortData[e.ColumnIndex].Ascending;
            dgrTMs.DataSource = null;
            dgrTMs.Refresh();
            dgrTMs.DataSource = filteredList;
        }


        private delegate int SortMethod(TMListItem x, TMListItem y);
        private Sorting[] sortData;
        private string myName = "";

        private int ownerComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            string o1 = x.Owner;
            string o2 = y.Owner;
            if (o1 == null && o2 != null) return 1;
            if (o1 != null && o2 == null) return -1;
            // my name comes first
            if (String.Compare(o1, myName, StringComparison.InvariantCulture) == 0) return -1;
            if (String.Compare(o2, myName, StringComparison.InvariantCulture) == 0) return 1;

            int c = String.Compare(o1, o2);
            if (c != 0) return c;
            return String.Compare(x.TMName, y.TMName);
        }

        private int nameComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            int c = String.Compare(x.TMName, y.TMName);
            return c;
        }

        private int sourceComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            int c = String.Compare(x.SourceLang, y.SourceLang);
            return c;
        }

        private int targetComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            int c = String.Compare(x.TargetLang, y.TargetLang);
            return c;
        }

        private int permissionComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            int c = String.Compare(x.LTPermission, y.LTPermission);
            return c;
        }

        private int lookupComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            if (x.SDLLookup && !y.SDLLookup) return -1;
            if (!x.SDLLookup && y.SDLLookup) return 1;
            return 0;
        }

        private int updateComparer(TMListItem x, TMListItem y)
        {
            if (x == null && y == null) return 0;
            if (x == null && y != null) return 1;
            if (x != null && y == null) return -1;
            if (x.SDLUpdate && !y.SDLUpdate) return -1;
            if (!x.SDLUpdate && y.SDLUpdate) return 1;
            return 0;
        }

        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && (txtUsername.Focused || txtPassword.Focused || txtServerName.Focused ||
                rbLanguageTerminal.Focused || rbMemoQServer.Focused || rbWindows.Focused))
            {
                tryLogin();
            }
            //else if (keyData == Keys.Enter)
            //{
            //    btnOK_Click(btnOK, null);
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            filterDatagrid();
        }

        private void cmbTargetLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterDatagrid();
        }

        private void filterDatagrid()
        {
            if (tmGridSourceList.Count == 0) return;

            string lang;
            if (cmbTargetLang.SelectedIndex < 1) lang = "";
            else lang = (string)cmbTargetLang.SelectedItem;

            string name = txtFilter.Text;

            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(lang)) filteredList = tmGridSourceList;
            // only language
            else if (String.IsNullOrEmpty(name)) filteredList = tmGridSourceList.Where(tm => tm.TargetLang.IndexOf(lang, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            // only name
            else if (String.IsNullOrEmpty(lang)) filteredList = tmGridSourceList.Where(tm => tm.TMName.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            // both
            else filteredList = tmGridSourceList.Where(tm =>
                tm.TMName.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1 &&
                tm.TargetLang.IndexOf(lang, StringComparison.OrdinalIgnoreCase) != -1).ToList();

            dgrTMs.DataSource = filteredList;
        }


        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            txtUsername.Enabled = txtPassword.Enabled = true;

            if (rbWindows.Checked)
            {
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                txtUsername.Text = identity.Name;
                txtPassword.Text = "";
                //txtUsername.Text = txtPassword.Text = "";
                //txtUsername.ReadOnly = txtPassword.ReadOnly = true;
            }
            else if (rbLanguageTerminal.Checked)
            {
                // first one on the list is surely an LT provider: fill in the fields
                if (localTMProviders[0] != null)
                {
                    txtUsername.Text = localTMProviders[0].Settings.UserName;
                    txtPassword.Text = String.IsNullOrEmpty(localTMProviders[0].Settings.Password) ? "" : StringEncoder.DecryptText(localTMProviders[0].Settings.Password);
                    txtUsername.ReadOnly = txtPassword.ReadOnly = false;
                }
            }
            else if (rbSSO.Checked)
            {
                txtUsername.Text = txtPassword.Text = "";
                txtUsername.Enabled = txtPassword.Enabled = false;
            }
            else
            {
                // memoQ server
                if (currentTMProvider != null && currentTMProvider.Settings.LoginType == LoginTypes.MemoQServer)
                {
                    txtUsername.Text = currentTMProvider.Settings.UserName;
                    txtPassword.Text = StringEncoder.DecryptText(currentTMProvider.Settings.Password);
                }
                else
                {
                    txtUsername.Text = txtPassword.Text = "";
                }
                txtUsername.ReadOnly = txtPassword.ReadOnly = false;
            }
        }

        private void showAllExceptions(TMProviderBase provider)
        {
            if (provider == null) return;
            string m = ExceptionHelper.GetAllExceptionsMessage(provider.ExceptionList, provider.ProviderName);
            if (!String.IsNullOrEmpty(m)) ExceptionHelper.ShowMessage(m);
            provider.ClearExceptions();
        }

        private void removeServer(TMProviderBase provider)
        {
            int index = localTMProviders.FindIndex(item => (String.Compare(item.ProviderName, provider.ProviderName) == 0));
            if (index == -1) return;

            // we can't delete Language Terminal
            if (index < 0) return;
            if (index == 0)
            {
                TMProviderBase p = localTMProviders[0];

                p.Settings.UserName = "";
                p.Settings.Password = "";
                p.IsLoggedIn = false;

                displayProvider(p, true);
                providerListPanel.RefreshProvider(p);
                return;
            }

            localTMProviders.RemoveAt(index);
            providerListChanged(localTMProviders);
            if (index != 0) selectItemInList(index - 1);
            else selectItemInList(0);

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                if(System.IO.File.Exists(AppData.HelpFilePath))  System.Diagnostics.Process.Start(AppData.HelpFilePath);
                else System.Diagnostics.Process.Start("http://kilgray.com/memoq/StudioPlugin/help-en/index.html");

            }
            catch
            {
                MessageBox.Show(this, PluginResources.Error_NoHelp, PluginResources.Error_Error);
            }
        }

        private void pbRemoveServer_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(pbAddMemoQServer, PluginResources.pbRemoveServerTooltip);
        }

        private void pbRemoveServer_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(pbAddMemoQServer, PluginResources.pbRemoveServerTooltip);
        }

        private void showGeneralSettings()
        {
            generalSettingsControl.Location = new Point(0, pnlLeftSide.Height - generalSettingsControl.Height);
            generalSettingsControl.Width = pnlLeftSide.Width;
            generalSettingsControl.Visible = true;
            gplGeneralSettings.Visible = false;
            generalSettingsControl.BringToFront();
        }

        private void hideGeneralSettings()
        {
            generalSettingsControl.Visible = false;
            gplGeneralSettings.Visible = true;
            generalSettingsControl.SendToBack();
        }

        private void gplGeneralSettings_Click(object sender, EventArgs e)
        {
            showGeneralSettings();
        }

        private void lblGeneralSettings_Click(object sender, EventArgs e)
        {
            showGeneralSettings();
        }

        private void generalSettingsControl_HeaderClicked()
        {
            hideGeneralSettings();
        }

        private void gplHeader_Click(object sender, EventArgs e)
        {
            showGeneralSettings();
        }

        private void pnlVerticalLine_Paint(object sender, PaintEventArgs e)
        {
            //ControlPaint.DrawBorder(e.Graphics, pnlVerticalLine.DisplayRectangle, lineColor, ButtonBorderStyle.Solid);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            tryLogin();
        }

        private void dgrTMs_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgrTMs.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
        }

        private void dgrTMs_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgrTMs.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void btnOpenLTAccount_Click(object sender, EventArgs e)
        {
            openLTPage();
        }

        private void txtServerName_Leave(object sender, EventArgs e)
        {
            string newURL;
            // first try to correct URL
            if (checkAndCorrectURL(txtServerName.Text, out newURL)) txtServerName.Text = newURL;
            else return;

            int ix = localTMProviders.FindIndex(p => String.Compare(p.ProviderName, txtServerName.Text, true) == 0);
            if (ix != -1) errorProvider.SetError(txtServerName, PluginResources.Error_ProviderAlreadyOnList);
            else errorProvider.SetError(txtServerName, String.Empty);
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            if (!(sender is TextBox)) return;
            errorProvider.SetError(sender as TextBox, String.Empty);
            errorProvider.SetError(txtPassword, String.Empty);
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            //errorProvider.SetError(txtUsername, String.Empty);
            errorProvider.SetError(txtPassword, String.Empty);

        }
    }
}
