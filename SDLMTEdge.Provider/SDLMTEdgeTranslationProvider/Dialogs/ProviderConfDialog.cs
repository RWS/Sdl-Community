﻿using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Windows.Threading;
using NLog;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.Dialogs
{
	public partial class ProviderConfDialog : Form
	{
		private ITranslationProviderCredentialStore credentialStore;
		private LanguagePair[] LanguagePairs;
		private System.Timers.Timer lpPopulationTimer = new System.Timers.Timer(500);
		public TranslationOptions Options { get; set; }
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
			Text = @"SDL Machine Translation Edge";

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
			ConnectionBox.Checked = Options.RequiresSecureProtocol;
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
				_logger.Error(e);
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
				_logger.Error($"{Constants.BuildUri}: {e.Message}\n {e.StackTrace}");
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
			if (AuthenticateCredentials(credentials, false) && Options.ApiVersion == APIVersion.v2)
			{
				var languagePairChoices = Options.SetPreferredLanguages(LanguagePairs);

				Options.SetDictionaries(languagePairChoices);

				// Since this is run on a separate thread, use invoke inside SetTradosLPs() to communicate with the master thread.
				var lpChoicesColumn = new DataGridViewComboBoxColumn();
				var lpDictionariesColumn = new DataGridViewComboBoxColumn();

				SetTradosLPs(lpChoicesColumn, lpDictionariesColumn, languagePairChoices);
			}
			else
			{
				MessageBox.Show(PluginResources.AuthenticationFailed, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Set the TradosLPs grid values
		/// </summary>
		/// <param name="lpChoicesColumn">lpChoicesColumn</param>
		/// <param name="lpDictionariesColumn">lpDictionariesColumn</param>
		/// <param name="languagePairChoices">languagePairChoicess</param>
		private void SetTradosLPs(
			DataGridViewComboBoxColumn lpChoicesColumn,
			DataGridViewComboBoxColumn lpDictionariesColumn,
			TradosToMTEdgeLP[] languagePairChoices)
		{
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

				lpChoicesColumn.Name = "SDL MT Edge Language Pair";
				lpChoicesColumn.FlatStyle = FlatStyle.Flat;

				lpDictionariesColumn.Name = "SDL MT Edge Dictionaries";
				lpDictionariesColumn.FlatStyle = FlatStyle.Flat;

				TradosLPs.Columns.AddRange(targetColumn, lpChoicesColumn, lpDictionariesColumn);

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
						RemoveIncorrectLanguageSet();
						if (Options.LPPreferences.Count > 0)
						{
							SetPreferedLanguageFlavours();
						}//set the default value in the case which users add the translation provider in project creation step
						else
						{
							for (var i = 0; i < languagePairChoices.Length; i++)
							{
								Options.LPPreferences[languagePairChoices[i].TradosCulture] = languagePairChoices[i].MtEdgeLPs?.FirstOrDefault();
								var comboBox = (DataGridViewComboBoxCell)TradosLPs?.Rows[i].Cells[1];
								comboBox.Value = languagePairChoices[i].MtEdgeLPs?.FirstOrDefault()?.LanguagePairId;
							}
						}
					}
					FinishButton.Enabled = true;
				}
				catch (Exception e)
				{
					FinishButton.Enabled = true;
					_logger.Error($"{e.Message}\n {e.StackTrace}");
				}
			}));
		}

		private void RemoveIncorrectLanguageSet()
		{
			foreach (var lpKey in Options.LPPreferences.ToList())
			{
				var languagePair = LanguagePairs.FirstOrDefault(lp =>
					lp.TargetCulture.ThreeLetterWindowsLanguageName.Equals(lpKey.Key.ThreeLetterWindowsLanguageName));
				if (languagePair == null)
				{
					Options.LPPreferences.Remove(lpKey.Key);
				}
			}
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
				_logger.Info($"lp:{lp}");

				foreach (var langP in Options.LPPreferences)
				{
					_logger.Info($"LPPreferences foreach {langP.Value.LanguagePairId}");
				}

				_logger.Error($"{e.Exception.Message}\n {e.Exception.StackTrace}");
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
				var comboBox = (DataGridViewComboBoxCell)TradosLPs?.Rows[i].Cells[comboboxColumnIndex];
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
			if (comboBox.Value == null) return;
			var newLp = TradosLPs[e.ColumnIndex, e.RowIndex].Value as string;
			var lpPairing = TradosLPs[e.ColumnIndex, e.RowIndex].Tag as SDLMTEdgeApi.TradosToMTEdgeLP;

			if (lpPairing != null)
			{
				var languagePair = lpPairing.MtEdgeLPs.FirstOrDefault(lp => lp.LanguagePairId == newLp);
				if (languagePair != null)
				{
					Options.LPPreferences[lpPairing.TradosCulture] = languagePair;
				}
			}

			if (TradosLPs[e.ColumnIndex, e.RowIndex].OwningColumn.Name.Equals("SDL MT Edge Dictionaries") && lpPairing != null)
			{
				lpPairing = TradosLPs[1, e.RowIndex].Tag as SDLMTEdgeApi.TradosToMTEdgeLP;
				newLp = TradosLPs[1, e.RowIndex].Value as string;
				var languagePair = lpPairing?.MtEdgeLPs.FirstOrDefault(lp => lp.LanguagePairId == newLp);
				if (languagePair != null)
				{
					Options.LPPreferences[lpPairing.TradosCulture] = languagePair;
				}
				Options.LPPreferences[lpPairing.TradosCulture].DictionaryId = TradosLPs[e.ColumnIndex, e.RowIndex].Value as string;
			}
		}

		void TradosLPs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < TradosLPs?.Rows.Count; i++)
            {
                var comboCell = (DataGridViewComboBoxCell)TradosLPs.Rows[i].Cells["SDL MT Edge Language Pair"];
                var dictionariesCombo = (DataGridViewComboBoxCell)TradosLPs.Rows[i].Cells["SDL MT Edge Dictionaries"];
                var entry = TradosLPs.Rows[i].DataBoundItem as TradosToMTEdgeLP;
                if (entry == null) continue;

 

                comboCell.Tag = entry;
                comboCell.DataSource = entry.MtEdgeLPs.Select(lp => lp.LanguagePairId).ToList();

 

                if (entry.Dictionaries != null)
                {
                    dictionariesCombo.DataSource = entry.Dictionaries.Select(d => d.DictionaryId).ToList();
                    dictionariesCombo.Value = entry.Dictionaries[0].DictionaryId; // set by default "No dictionary" value
                }
                if (Options?.LPPreferences == null || Options?.LPPreferences.Count == 0)
                {
                    continue;
                }
                
                if (Options.LPPreferences.ContainsKey(entry.TradosCulture))
                {
                    var currentDictionaryId = Options.LPPreferences[entry.TradosCulture].DictionaryId;
                    comboCell.Value = Options.LPPreferences[entry.TradosCulture].LanguagePairId;

 

                    ConfigureDictionary(currentDictionaryId, dictionariesCombo, entry);
                }
                else
                {
                    dictionariesCombo.Value = Constants.NoDictionary;
                }
            }
        }

		private void ConfigureDictionary(string currentDictionaryId, DataGridViewComboBoxCell dictionariesCombo, TradosToMTEdgeLP entry)
		{
			if (string.IsNullOrEmpty(currentDictionaryId))
			{
				Options.LPPreferences[entry.TradosCulture].DictionaryId = entry.Dictionaries[0].DictionaryId;
				dictionariesCombo.Value = entry.Dictionaries[0].DictionaryId;
			}
			else
			{
				dictionariesCombo.Value = currentDictionaryId;
				Options.LPPreferences[entry.TradosCulture].DictionaryId = currentDictionaryId;
			}
		}

		private void OKClicked(object sender, EventArgs e)
		{
			if (!Options.LPPreferences.Any(lp => lp.Value == null))
			{
				int? port = GetPort();
				if (!port.HasValue)
				{
					DialogResult = DialogResult.None;
					var error = $"The port must be a valid port between {IPEndPoint.MinPort} and {IPEndPoint.MaxPort}.";
					MessageBox.Show(error, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			else
			{
				MessageBox.Show(Constants.NoProviderSetup, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
			SDLMTEdgeTranslatorHelper.ExpireLanguagePairs();
			Options.ApiVersion = APIVersion.Unknown;

			string token;
			try
			{
				if (Options.UseBasicAuthentication)
				{
					token = SDLMTEdgeTranslatorHelper.GetAuthToken(Options, credentials);
				}
				else
				{
					token = APIKeyField.Text;
					SDLMTEdgeTranslatorHelper.VerifyBasicAPIToken(Options, credentials);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.AuthenticateCredentials}: {e.Message}\n {e.StackTrace}");
				if (showAlertOnFailure)
				{
					DialogResult = DialogResult.None;
					MessageBox.Show(e.Message, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				return false;
			}
			Options.ApiToken = token;

			if (tabControl != null)
			{
				tabControl.Controls.RemoveByKey("LPTab");
				if (Options?.ApiVersion == APIVersion.v2)
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
			var credentialsValid = !string.IsNullOrEmpty(UsernameField.Text) &&
								   !string.IsNullOrEmpty(PasswordField.Text) &&
								   !string.IsNullOrEmpty(HostNameField.Text);
			ToggleCursor();
			if (!Options.UseBasicAuthentication || credentialsValid)
			{
				try
				{
					PopulateLanguagePairs();
				}
				catch (Exception ex)
				{
					_logger.Error(ex.Message);
					MessageBox.Show(ex.Message, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			ToggleCursor();
		}

		private void ToggleCursor()
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() => Cursor = Cursor == Cursors.WaitCursor ? Cursors.Default : Cursors.WaitCursor));
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

		private void connectionBox_CheckedChanged(object sender, EventArgs e)
		{
			Options.RequiresSecureProtocol = ConnectionBox.Checked;
		}
	}
}
