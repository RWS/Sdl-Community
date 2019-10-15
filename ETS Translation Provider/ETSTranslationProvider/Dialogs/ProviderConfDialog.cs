using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using ETSTranslationProvider.Helpers;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	public partial class ProviderConfDialog : Form
    {
		private ITranslationProviderCredentialStore credentialStore;
		private LanguagePair[] LanguagePairs;
		private System.Timers.Timer lpPopulationTimer = new System.Timers.Timer(500);
		public TranslationOptions Options { get; set; }
		public static readonly Log Log = Log.Instance;

		public ProviderConfDialog(TranslationOptions options, ITranslationProviderCredentialStore store, LanguagePair[] languagePairs)
		{
			credentialStore = store;
			Options = options;
			LanguagePairs = languagePairs;
			InitializeComponent();

			if (PluginConfiguration.CurrentInstance.DefaultConnection.HasValue &&
				Options.Host == null)
			{
				Options.Host = PluginConfiguration.CurrentInstance.DefaultConnection.Value.Host;
				Options.Port = PluginConfiguration.CurrentInstance.DefaultConnection.Value.Port;
			}
			// Set this outside the if statement in case Options Host & Port were already set to the default connection
			setDefaultTM.Checked = PluginConfiguration.CurrentInstance.DefaultConnection.HasValue
				&& PluginConfiguration.CurrentInstance.DefaultConnection.Value.Host == Options.Host
				&& PluginConfiguration.CurrentInstance.DefaultConnection.Value.Port == Options.Port;

			UpdateDialog();
			Text = @"SDL ETS Translation";

			// .5 seconds after certain events, run the populate command. This prevents us from authenticating each
			// keypress (as that was causing massive lag).
			lpPopulationTimer.AutoReset = false;
			lpPopulationTimer.Elapsed += lpPopulationTimer_Elapsed;
		}

		public void DisplayForCredentialsOnly()
		{
			// Disable hostname and port
			HostNameField.Enabled = false;
			PortField.Enabled = false;
		}

		private void UpdateDialog()
		{
			// Populate the fields with the pertinent information
			HostNameField.Text = Options.Host;
			PortField.Text = Options.Port.ToString();
			SaveCredentialsOption.Checked = Options.PersistCredentials;
			BasicAuthenticationOption.Checked = Options.UseBasicAuthentication;
			PopulateCredentials();
		}

		private int? GetPort()
		{
			int portNumber;
			if (!int.TryParse(PortField.Text, out portNumber) || portNumber > IPEndPoint.MaxPort || portNumber < IPEndPoint.MinPort)
			{
				return null;
			}
			return portNumber;
		}

		private void PopulateCredentials()
		{
			var uri = GetUri();
			if (uri == null)
			{
				return;
			}

			// Populate the login field if they've opted to persist the credentials
			var credentials = credentialStore.GetCredential(uri);
			if (credentials != null && credentials.Persist)
			{
				// Keep credentials persisting checked
				SaveCredentialsOption.Checked = true;

				// Populate the related textboxes with the saved credentials
				var gCredentials = new GenericCredentials(credentials.Credential);
				UsernameField.Text = gCredentials.UserName;
				PasswordField.Text = gCredentials.Password;
				APIKeyField.Text = gCredentials["API-Key"];
				BasicAuthenticationOption.Checked = gCredentials["UseApiKey"] != "true";
				APIKeyOption.Checked = !BasicAuthenticationOption.Checked;

				DelayedLPPopulation();
			}
		}

		private Uri GetUri()
		{
            //try to take the credentials from the project if there are any
			try
			{
				return Options.Uri;
			}
			catch (Exception e)
			{
				Log.Logger.Error(e);
			}

			// If the port is not a number or out of port range, don't build the URI
			int? port = GetPort();
			if (!port.HasValue)
			{
				return null;
			}
			try
			{
				var uriBuilder = new UriBuilder(TranslationProvider.TranslationProviderScheme, HostNameField.Text, port.Value);
				return uriBuilder.Uri;
			}
			catch (UriFormatException e)
			{
				Log.Logger.Error($"{Constants.BuildUri}: {e.Message}\n {e.StackTrace}");
				return null;
			}
		}

		private void PopulateLanguagePairs()
		{
			if (LanguagePairs == null || LanguagePairs.Length == 0)
			{
				return;
			}
			var credentials = GetCredentials();
			if (!AuthenticateCredentials(credentials, false))
			{
				return;
			}

			if (Options.ApiVersion != ETSApi.APIVersion.v2)
			{
				return;
			}
			var languagePairChoices = Options.SetPreferredLanguages(LanguagePairs);

			// Since this is run on a separate thread, use invoke to communicate with the master thread.
			var lpChoicesColumn = new DataGridViewComboBoxColumn();
			TradosLPs.Invoke(new Action(() =>
			{
				// This gets called multiple times, so let's clear out the old contents
				TradosLPs.Columns.Clear();
				TradosLPs.AutoGenerateColumns = false;

				var targetColumn = new DataGridViewTextBoxColumn
				{
					Name = "Target Language",
					DataPropertyName = "TradosCulture",
					ReadOnly = true
				};

				lpChoicesColumn.Name = "SDL ETS Language Pair";
				lpChoicesColumn.FlatStyle = FlatStyle.Flat;

				TradosLPs.Columns.AddRange(targetColumn, lpChoicesColumn);
				// Handler for populating combobox
				TradosLPs.DataBindingComplete += TradosLPs_DataBindingComplete;

				TradosLPs.DataSource = languagePairChoices;

				// Handlers for when the combobox changes
				TradosLPs.CellValueChanged += TradosLPs_CellValueChanged;
				TradosLPs.CurrentCellDirtyStateChanged += TradosLPs_CurrentCellDirtyStateChanged;
				TradosLPs.DataError += TradosLPs_DataError;
				try
				{
					if (Options?.LPPreferences != null)
					{
						if (Options.LPPreferences.Count > 0)
						{
							SetPreferedLanguageFlavours();
						}//set the default value in the case which users add the translation provider in project creation step
						else
						{
							for (var i = 0; i < languagePairChoices.Length; i++)
							{
								Options.LPPreferences[languagePairChoices[i].TradosCulture] = languagePairChoices[i].ETSLPs?.FirstOrDefault();
								var comboBox = (DataGridViewComboBoxCell)TradosLPs?.Rows[i].Cells[1];
								comboBox.Value = languagePairChoices[i].ETSLPs?.FirstOrDefault()?.LanguagePairId;
							}
						}
					}
					FinishButton.Enabled = true;
				}
				catch (Exception e)
				{
					Log.Logger.Error($"{e.Message}\n {e.StackTrace}");
				}
			}));
		}

		/// <summary>
		/// This errror handle is for DataGridViewCombobox cell value is not valid error
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TradosLPs_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message.Contains("value is not valid"))
			{
				var lp = TradosLPs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
				Log.Logger.Info($"lp:{lp}");

				foreach (var langP in Options.LPPreferences)
				{
					Log.Logger.Info($"LPPreferences foreach {langP.Value.LanguagePairId}");
				}

				Log.Logger.Error($"{e.Exception.Message}\n {e.Exception.StackTrace}");
				if (!((DataGridViewComboBoxColumn)TradosLPs.Columns[e.ColumnIndex]).Items.Contains(lp))
				{
					((DataGridViewComboBoxColumn)TradosLPs.Columns[e.ColumnIndex]).Items.Add(lp);
					e.ThrowException = false;
				}
			}
		}

		// This event handler manually raises the CellValueChanged event
		// by calling the CommitEdit method.
		void TradosLPs_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (TradosLPs.IsCurrentCellDirty)
			{
				// This fires the cell value changed handler below
				TradosLPs.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

	    private void SetPreferedLanguageFlavours()
	    {
		    const int comboboxColumnIndex = 1;

		    for (var i = 0; i < Options.LPPreferences.Count; i++)
		    {
			    var comboBox = (DataGridViewComboBoxCell) TradosLPs?.Rows[i].Cells[comboboxColumnIndex];
			    if (comboBox != null)
			    {
					comboBox.Value = Options.LPPreferences.ToList()[i].Value?.LanguagePairId;
				}
			}
		}
	    private void TradosLPs_CellEnter(object sender, DataGridViewCellEventArgs e)
	    {
		    var tradosLPs = sender as DataGridView;

		    // Check to make sure the cell clicked is the cell containing the combobox
		    if (e.RowIndex != -1 && e.ColumnIndex != -1 && tradosLPs.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
		    {
			    tradosLPs.BeginEdit(true);
			    ((ComboBox)tradosLPs.EditingControl).DroppedDown = true;
		    }
	    }
		private void TradosLPs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			const int comboboxColumnIndex = 1;
			var comboBox = (DataGridViewComboBoxCell)TradosLPs.Rows[e.RowIndex].Cells[comboboxColumnIndex];
			if (comboBox.Value != null)
			{
				var newLp = TradosLPs[e.ColumnIndex, e.RowIndex].Value as string;

				var lpPairing = TradosLPs[e.ColumnIndex, e.RowIndex].Tag as ETSApi.TradosToETSLP;
				if (lpPairing != null)
				{
					Options.LPPreferences[lpPairing.TradosCulture] = lpPairing.ETSLPs.First(lp => lp.LanguagePairId == newLp);
				}
			}
		}

		void TradosLPs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			for (int i = 0; i < TradosLPs.Rows.Count; i++)
			{
				var comboCell = (DataGridViewComboBoxCell)TradosLPs.Rows[i].Cells["SDL ETS Language Pair"];
				var entry = TradosLPs.Rows[i].DataBoundItem as ETSApi.TradosToETSLP;

				comboCell.Tag = entry;
				comboCell.DataSource = entry.ETSLPs.Select(lp => lp.LanguagePairId).ToList();
				if (Options.LPPreferences.ContainsKey(entry.TradosCulture))
					comboCell.Value = Options.LPPreferences[entry.TradosCulture].LanguagePairId;
			}
		}

		private void OKClicked(object sender, EventArgs e)
		{
			int? port = GetPort();
			if (!port.HasValue)
			{
				DialogResult = DialogResult.None;
				var error = $"The port must be a valid port between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.";
				MessageBox.Show(error, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var credentials = GetCredentials();
			if (!AuthenticateCredentials(credentials))
				return;

			var creds = new TranslationProviderCredential(credentials.ToCredentialString(), Options.PersistCredentials);
			credentialStore.AddCredential(Options.Uri, creds);

			if (setDefaultTM.Checked)
			{
				PluginConfiguration.CurrentInstance.DefaultConnection = new Connection(
					host: Options.Host,
					port: Options.Port
				);
				PluginConfiguration.CurrentInstance.SaveToFile();
			}
			else if (!setDefaultTM.Checked
			  && PluginConfiguration.CurrentInstance.DefaultConnection.HasValue
			  && PluginConfiguration.CurrentInstance.DefaultConnection.Value.Host == Options.Host
			  && PluginConfiguration.CurrentInstance.DefaultConnection.Value.Port == Options.Port)
			{
				PluginConfiguration.CurrentInstance.DefaultConnection = null;
				PluginConfiguration.CurrentInstance.SaveToFile();
			}
		}

		private GenericCredentials GetCredentials()
		{
			var userName = UsernameField.Text;
			var password = PasswordField.Text;
			var credentials = new GenericCredentials(userName, password);
			credentials["API-Key"] = APIKeyField.Text;
			credentials["UseApiKey"] = BasicAuthenticationOption.Checked ? "false" : "true";
			return credentials;
		}

		private bool AuthenticateCredentials(GenericCredentials credentials, bool showAlertOnFailure = true)
		{
			// Expire the old LPs in case we've changed the host and reset the API version
			ETSApi.ETSTranslatorHelper.ExpireLanguagePairs();
			Options.ApiVersion = ETSApi.APIVersion.Unknown;

			string token;
			try
			{
				if (Options.UseBasicAuthentication)
				{
					token = ETSApi.ETSTranslatorHelper.GetAuthToken(Options, credentials);
				}
				else
				{
					token = APIKeyField.Text;
					ETSApi.ETSTranslatorHelper.VerifyBasicAPIToken(Options, credentials);
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.AuthenticateCredentials}: {e.Message}\n {e.StackTrace}");
				if (showAlertOnFailure)
				{
					DialogResult = DialogResult.None;
					if (Environment.UserInteractive)
					{
						MessageBox.Show(e.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				return false;
			}
			Options.ApiToken = token;

			if (tabControl != null)
			{
				tabControl.Controls.RemoveByKey("LPTab");
				if (Options?.ApiVersion == ETSApi.APIVersion.v2)
				{
					tabControl.Invoke(new Action(() =>
					{
						if (LPTab != null)
						{
							tabControl.Controls.Add(LPTab);
						}
					}));
				}
			}

			return true;
		}

		private void PortKeyPressed(object sender, KeyPressEventArgs e)
		{
			// The port must have numbers only. This won't stop copy pasting non-digit
			// characters, so that'll be separate validation.
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
			}
		}

		private void DelayedLPPopulation()
		{
			lpPopulationTimer.Stop();
			lpPopulationTimer.Start();
		}

		private void lpPopulationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (Options.UseBasicAuthentication)
			{
				if (!string.IsNullOrEmpty(UsernameField.Text) && !string.IsNullOrEmpty(PasswordField.Text) &&
				    !string.IsNullOrEmpty(HostNameField.Text))
				{
					PopulateLanguagePairs();
				}
			}
			else
			{
				PopulateLanguagePairs();
			}
		}

		private void HostNameChanged(object sender, EventArgs e)
		{
			Uri parsedUri;
			if (Uri.TryCreate(HostNameField.Text, UriKind.Absolute, out parsedUri))
				// If they enter in a URI instead of a hostname, "politely" correct their entry to just the hostname
				// portion of the URI
				HostNameField.Text = parsedUri.Host;
			Options.Host = HostNameField.Text;
			PopulateCredentials();
		}

		private void PortChanged(object sender, EventArgs e)
		{
			int? port = GetPort();
			if (port == null)
				return;
			Options.Port = port.Value;
			PopulateCredentials();
		}

		private void APIKeyChanged(object sender, EventArgs e)
		{
			DelayedLPPopulation();
		}

		private void PersistCredentialsOptionChanged(object sender, EventArgs e)
		{
			Options.PersistCredentials = SaveCredentialsOption.Checked;
		}

		private void BasicAuthenticationOptionChanged(object sender, EventArgs e)
		{
			ToggleAuthenticationChecked();
		}

		private void APIKeyOptionChanged(object sender, EventArgs e)
		{
			ToggleAuthenticationChecked();
		}

		private void ToggleAuthenticationChecked()
		{
			Options.UseBasicAuthentication = BasicAuthenticationOption.Checked;

			// Enable Basic authentication if we're using API keys
			UsernameField.Enabled = Options.UseBasicAuthentication;
			PasswordField.Enabled = Options.UseBasicAuthentication;

			// Enable API keys if we're using basic authentication
			APIKeyField.Enabled = !Options.UseBasicAuthentication;
		}

		private void PasswordField_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				TryToAuthenticate();
			}
		}

	    private void TryToAuthenticate()
	    {
			if (Options.UseBasicAuthentication)
			{
				if (!string.IsNullOrEmpty(UsernameField.Text) && !string.IsNullOrEmpty(PasswordField.Text) &&
				    !string.IsNullOrEmpty(HostNameField.Text))
				{
					PopulateLanguagePairs();
				}
			}
		}

		private void UsernameField_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				TryToAuthenticate();
			}
		}
	}
}
