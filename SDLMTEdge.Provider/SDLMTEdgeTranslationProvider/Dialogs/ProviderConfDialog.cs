using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using NLog;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Properties;
using Sdl.Community.MTEdge.Provider.SDLMTEdgeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.Dialogs
{
	public partial class ProviderConfDialog : Form
	{
		private ITranslationProviderCredentialStore credentialStore;
		private bool inRemoveLanguagesPreferencesTab;
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
			Text = Properties.WeaverEdgeResource.WeaverEdge_OptionsWindowTitle;

			// .5 seconds after certain events, run the populate command. This prevents us from authenticating each
			// keypress (as that was causing massive lag).
			lpPopulationTimer.AutoReset = false;
			lpPopulationTimer.Elapsed += lpPopulationTimer_Elapsed;

			AddErrorIconsToTabControl();

			LPTab.VisibleChanged += LPTab_Click;
		}

		private void AddErrorIconsToTabControl()
		{
			var errorIcons = new ImageList();
			errorIcons.Images.Add(SystemIcons.Warning);
			errorIcons.ImageSize = new Size(13, 13);
			tabControl.ImageList = errorIcons;
		}

		private void LPTab_Click(object sender, EventArgs e)
		{
			ValidateDataGridLanguageMappings();
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
				ConnectionBox.Checked = gCredentials["RequiresSecureProtocol"] == "true";

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
				SetTradosLPs(languagePairChoices);
			}
			else
			{
				MessageBox.Show(PluginResources.AuthenticationFailed, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Set the TradosLPs grid values
		/// </summary>
		/// <param name="languagePairChoices">languagePairChoices</param>
		private void SetTradosLPs(
			TradosToMTEdgeLP[] languagePairChoices)
		{


			TradosLPs.Invoke(new Action(() =>
			{
				// This gets called multiple times, so let's clear out the old contents

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

		private void AddColumnsToDataGridView()
		{
			TradosLPs.Columns.Clear();
			TradosLPs.AutoGenerateColumns = false;

			var targetColumn = new DataGridViewTextBoxColumn
			{
				Name = "Target Language",
				DataPropertyName = nameof(TradosToMTEdgeLP.TradosCulture),
				ReadOnly = true
			};

			var lpChoicesColumn = new DataGridViewComboBoxColumn
			{
				Name = WeaverEdgeResource.WeaverEdge_LanguagePairColumnName,
				FlatStyle = FlatStyle.Flat,
			};
			var lpDictionariesColumn = new DataGridViewComboBoxColumn();

			lpDictionariesColumn.Name = WeaverEdgeResource.WeaverEdge_DictionariesColumnName;
			lpDictionariesColumn.FlatStyle = FlatStyle.Flat;

			TradosLPs.Columns.AddRange(targetColumn, lpChoicesColumn, lpDictionariesColumn);
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
		private void TradosLPs_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message.Contains("value is not valid"))
			{
				var lp = TradosLPs.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
				_logger.Info($"lp:{lp}");

				foreach (var langP in Options.LPPreferences)
				{
					_logger.Info($"LPPreferences foreach {langP.Value?.LanguagePairId}");
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
			const int languagePairColumnIndex = 1;
			var languagePairComboBox = (DataGridViewComboBoxCell)TradosLPs.Rows[e.RowIndex].Cells[languagePairColumnIndex];
			if (languagePairComboBox.Value == null)
				return;

			//Valentin -> Only in the comboboxColumnIndex column the Tag it's bind with a TradosToMTEdgeLP correspondent object for the current row . olumn 2 or 3 have no bind in its Tag...
			if (TradosLPs[languagePairColumnIndex, e.RowIndex].Tag is TradosToMTEdgeLP lpPairing)
				if (TradosLPs[e.ColumnIndex, e.RowIndex].OwningColumn.Name.Equals(Properties.WeaverEdgeResource.WeaverEdge_LanguagePairColumnName))
				{
					var newLp = TradosLPs[e.ColumnIndex, e.RowIndex].Value as string;
					var languagePair = lpPairing.MtEdgeLPs.FirstOrDefault(lp => lp.LanguagePairId == newLp);
					if (languagePair != null)
						Options.LPPreferences[lpPairing.TradosCulture] = languagePair;
				}
				else
				//Valentin - each column with its calls. Otherwise it's a mess. for each cell from the grid will execute the same for X times. Sometimes  it will ruin what the previous call set in Options, just because now it's on another column another cell etc. ....
				if (TradosLPs[e.ColumnIndex, e.RowIndex].OwningColumn.Name.Equals(Properties.WeaverEdgeResource.WeaverEdge_DictionariesColumnName))
				{

					Options.LPPreferences[lpPairing.TradosCulture].DictionaryId = TradosLPs[e.ColumnIndex, e.RowIndex].Value as string;
				}
		}

		void TradosLPs_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			AddColumnsToDataGridView();
			ValidateTabLanguageMappings();

			if (inRemoveLanguagesPreferencesTab)
				return;
			for (var i = 0; i < TradosLPs?.Rows.Count; i++)
			{
				var languagePairsComboCell = (DataGridViewComboBoxCell)TradosLPs.Rows[i].Cells[Properties.WeaverEdgeResource.WeaverEdge_LanguagePairColumnName];
				var dictionariesComboCell = (DataGridViewComboBoxCell)TradosLPs.Rows[i].Cells[Properties.WeaverEdgeResource.WeaverEdge_DictionariesColumnName];
				var entry = TradosLPs.Rows[i].DataBoundItem as TradosToMTEdgeLP;
				if (entry == null)
					continue;

				languagePairsComboCell.Tag = entry;
				languagePairsComboCell.DataSource = entry.MtEdgeLPs.Select(lp => lp.LanguagePairId).ToList();

				if (entry.Dictionaries != null)
				{
					dictionariesComboCell.DataSource = entry.Dictionaries.Select(d => d.DictionaryId).ToList();
					dictionariesComboCell.Value = entry.Dictionaries[0].DictionaryId; // set by default "No dictionary" value
				}
				if (Options?.LPPreferences == null || Options?.LPPreferences.Count == 0)
				{
					continue;
				}

				if (Options.LPPreferences.ContainsKey(entry.TradosCulture) && Options.LPPreferences[entry.TradosCulture] != null)
				{
					var currentDictionaryId = Options.LPPreferences[entry.TradosCulture].DictionaryId;
					languagePairsComboCell.Value = Options.LPPreferences[entry.TradosCulture].LanguagePairId;

					ConfigureDictionary(currentDictionaryId, dictionariesComboCell, entry);
				}
				else
				{
					dictionariesComboCell.Value = Constants.NoDictionary;
				}

			}
		}

		private void ConfigureDictionary(string currentDictionaryId, DataGridViewComboBoxCell dictionariesCombo, TradosToMTEdgeLP entry)
		{
			if (string.IsNullOrEmpty(currentDictionaryId))
			{
				if (!dictionariesCombo.Value.Equals(entry.Dictionaries[0].DictionaryId))
					dictionariesCombo.Value = entry.Dictionaries[0].DictionaryId;
				Options.LPPreferences[entry.TradosCulture].DictionaryId = entry.Dictionaries[0].DictionaryId;

			}
			else
			{
				//avoid to trigger again the grid cell changed event 
				if (!dictionariesCombo.Value.Equals(currentDictionaryId))
					dictionariesCombo.Value = currentDictionaryId;
				Options.LPPreferences[entry.TradosCulture].DictionaryId = currentDictionaryId;
			}
		}

		private void OKClicked(object sender, EventArgs e)
		{
			DialogResult = ValidateSettingsLanguageMappings();
			if (DialogResult == DialogResult.None)
				return;

			var port = GetPort();
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

			var cred = new TranslationProviderCredential(credentials.ToCredentialString(), Options.PersistCredentials);
			credentialStore.AddCredential(Options.Uri, cred);

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

		private DialogResult ValidateSettingsLanguageMappings()
		{
			var unmappedTargetLanguages = GetUnmappedTargetLanguages();
			var unmappedTargetLanguagesString =
				unmappedTargetLanguages.Aggregate("", (current, tl) => current + $"{Environment.NewLine}• {tl}");

			var dialogResult = MessageBox.Show(
				string.Format(PluginResources.MissingMappings_DialogMessage, Environment.NewLine,
					unmappedTargetLanguagesString, Environment.NewLine, Environment.NewLine),
				PluginResources.MissingMapings_DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			return dialogResult == DialogResult.Yes ? DialogResult.OK : DialogResult.None;
		}

		private void ValidateTabLanguageMappings()
		{
			var rows = TradosLPs.Rows;

			foreach (DataGridViewRow row in rows)
			{
				var columnIndex = TradosLPs.Columns[WeaverEdgeResource.WeaverEdge_LanguagePairColumnName].Index;
				var lpCell = row.Cells[columnIndex];
				if (!(lpCell.Value is null)) continue;

				LPTab.ImageIndex = 0;
				return;
			}
			LPTab.ImageIndex = -1;
		}

		private void ValidateDataGridLanguageMappings()
		{
			var rows = TradosLPs.Rows;

			foreach (DataGridViewRow row in rows)
			{
				var columnIndex = TradosLPs.Columns[WeaverEdgeResource.WeaverEdge_LanguagePairColumnName].Index;
				var lpCell = row.Cells[columnIndex];

				lpCell.ErrorText = lpCell.Value is null ? "Missing mapping" : "";
			}
		}


		private List<string> GetUnmappedTargetLanguages()
		{
			var rows = TradosLPs.Rows;
			var unmappedTargetLanguages = new List<string>();

			foreach (DataGridViewRow row in rows)
			{
				var item = TradosLPs.Rows[row.Index].DataBoundItem as TradosToMTEdgeLP;
				if (!item.MtEdgeLPs.Any())
				{
					var unmappedTargetLanguage = row.Cells[0].Value.ToString();
					if (!unmappedTargetLanguages.Contains(unmappedTargetLanguage))
						unmappedTargetLanguages.Add(unmappedTargetLanguage);
				}
			}


			return unmappedTargetLanguages;
		}

		private GenericCredentials GetCredentials()
		{
			var userName = UsernameField.Text;
			var password = PasswordField.Text;
			var credentials = new GenericCredentials(userName, password)
			{
				["API-Key"] = APIKeyField.Text,
				["UseApiKey"] = BasicAuthenticationOption.Checked ? "false" : "true",
				["RequiresSecureProtocol"] = ConnectionBox.Checked ? "true" : "false"
			};
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
				try
				{
					//avoiding to do binding when the window will be closed. Because removing and adding the tab will do the bind again.
					inRemoveLanguagesPreferencesTab = true;
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
				finally
				{
					inRemoveLanguagesPreferencesTab = false;
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
