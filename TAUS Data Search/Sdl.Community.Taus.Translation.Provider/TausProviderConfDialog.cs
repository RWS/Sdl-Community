using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider;
using Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider.Settings;

namespace Sdl.Community.Taus.Translation.Provider
{
	public partial class TausProviderConfDialog : Form
	{
		private Dictionary<string, string> _providers = new Dictionary<string, string>();
		private Dictionary<string, string> _products = new Dictionary<string, string>();
		private Dictionary<string, string> _owners = new Dictionary<string, string>();

		public TausProviderConfDialog(TausTranslationOptions options)
		{
			Options = options;
			InitializeComponent();

			UpdateDialog();
		}

		public TausTranslationOptions Options
		{
			get;
			set;
		}

		private void UpdateDialog()
		{
			try
			{
				comboBox_industry.Items.Clear();
				foreach (var item in TausTranslationProvider.AttributesIndustry)
				{
					comboBox_industry.Items.Add(item.Value);
				}
				comboBox_industry.Sorted = true;

				comboBox_contentType.Items.Clear();
				foreach (var item in TausTranslationProvider.AttributesContentType)
				{
					comboBox_contentType.Items.Add(item.Value);
				}
				comboBox_contentType.Sorted = true;

				_providers = Processor.GetAttributeListings(Options.ConnectionAuthKey, "provider", "segment", Options.ConnectionAppKey);
				foreach (var item in _providers)
				{
					comboBox_provider.Items.Add(item.Value);
				}
				comboBox_provider.Sorted = true;

				_products = Processor.GetAttributeListings(Options.ConnectionAuthKey, "product", "segment", Options.ConnectionAppKey);
				foreach (var item in _products)
				{
					comboBox_product.Items.Add(item.Value);
				}
				comboBox_product.Sorted = true;

				_owners = Processor.GetAttributeListings(Options.ConnectionAuthKey, "owner", "segment", Options.ConnectionAppKey);
				foreach (var item in _owners)
				{
					comboBox_owner.Items.Add(item.Value);
				}
				comboBox_owner.Sorted = true;

				if (Options.ConnectionAppKey == null)
					Options.ConnectionAppKey = string.Empty;
				if (Options.ConnectionUserName == null)
					Options.ConnectionUserName = string.Empty;
				if (Options.ConnectionUserPassword == null)
					Options.ConnectionUserPassword = string.Empty;
				if (Options.ConnectionAuthKey == null)
					Options.ConnectionAuthKey = string.Empty;

				if (Options.SearchCriteriaContentTypeId == null)
					Options.SearchCriteriaContentTypeId = string.Empty;
				if (Options.SearchCriteriaContentTypeName == null)
					Options.SearchCriteriaContentTypeName = string.Empty;

				if (Options.SearchCriteriaIndustryId == null)
					Options.SearchCriteriaIndustryId = string.Empty;
				if (Options.SearchCriteriaIndustryName == null)
					Options.SearchCriteriaIndustryName = string.Empty;

				if (Options.SearchCriteriaOwnerId == null)
					Options.SearchCriteriaOwnerId = string.Empty;
				if (Options.SearchCriteriaOwnerName == null)
					Options.SearchCriteriaOwnerName = string.Empty;

				if (Options.SearchCriteriaProviderId == null)
					Options.SearchCriteriaProviderId = string.Empty;
				if (Options.SearchCriteriaProviderName == null)
					Options.SearchCriteriaProviderName = string.Empty;

				if (Options.SearchCriteriaProductId == null)
					Options.SearchCriteriaProductId = string.Empty;
				if (Options.SearchCriteriaProductName == null)
					Options.SearchCriteriaProductName = string.Empty;

				if (Options.SearchTimeout == null)
					Options.SearchTimeout = string.Empty;

				if (Options.IgnoreTranslatedSegments == null)
					Options.IgnoreTranslatedSegments = "True";

				if (Options.ConnectionAppKey.Trim() == string.Empty)
					Options.ConnectionAppKey = TausTranslationProvider.TausApplicationProviderId;


				if (Options.ConnectionUserName.Trim() == string.Empty
					&& Options.ConnectionUserPassword.Trim() == string.Empty
					&& Options.ConnectionAuthKey.Trim() == string.Empty)
				{
					#region  |  read the plugin default settings  |

					var tausTmProvider = new Processor();
					var searchSettings = tausTmProvider.ReadSettings();

					Options.ConnectionUserName = searchSettings.UserName;
					Options.ConnectionUserPassword = searchSettings.Password;
					Options.ConnectionAuthKey = searchSettings.AuthKey;

					#endregion
				}

				textBox_applicationKey.Text = Options.ConnectionAppKey;
				textBox_userName.Text = Options.ConnectionUserName;
				textBox_password.Text = Options.ConnectionUserPassword;
				textBox_authKey.Text = Options.ConnectionAuthKey;

				if (textBox_userName.Text.Trim() != string.Empty
					&& textBox_password.Text.Trim() != string.Empty
					&& textBox_authKey.Text.Trim() != string.Empty)
				{
					pictureBox_authorizationImage_01.Visible = false;
					pictureBox_authorizationImage_02.Visible = true;

					linkLabel_viewAuthorizationKey.Enabled = true;
					label_authorization_message.Text = @"authorization key exists...";
				}
				else
				{
					pictureBox_authorizationImage_01.Visible = true;
					pictureBox_authorizationImage_02.Visible = false;

					linkLabel_viewAuthorizationKey.Enabled = false;
					label_authorization_message.Text = @"create an authorization key...";
				}

				// Content Type Id update
				if (string.IsNullOrEmpty(Options.SearchCriteriaContentTypeId.Trim()))
				{
					numericUpDown_contentTypeId.Value = 0;
					comboBox_contentType.SelectedIndex = 0;
				}
				else
				{
					numericUpDown_contentTypeId.Value = int.Parse(Options.SearchCriteriaContentTypeId);
					comboBox_contentType.SelectedItem = Options.SearchCriteriaContentTypeName;
				}

				// Industry Id update
				if (string.IsNullOrEmpty(Options.SearchCriteriaIndustryId.Trim()))
				{
					numericUpDown_industryId.Value = 0;
					comboBox_industry.SelectedIndex = 0;
				}
				else
				{
					numericUpDown_industryId.Value = int.Parse(Options.SearchCriteriaIndustryId);
					comboBox_industry.SelectedItem = Options.SearchCriteriaIndustryName;
				}

				// Provider Id update
				if (string.IsNullOrEmpty(Options.SearchCriteriaProviderId.Trim()))
				{
					numericUpDown_providerId.Value = 0;
					comboBox_provider.SelectedIndex = 0;
				}
				else
				{
					numericUpDown_providerId.Value = int.Parse(Options.SearchCriteriaProviderId);
					comboBox_provider.SelectedItem = Options.SearchCriteriaProviderName;
				}

				// Product Id update
				if (string.IsNullOrEmpty(Options.SearchCriteriaProductId.Trim()))
				{
					numericUpDown_productId.Value = 0;
					comboBox_product.SelectedIndex = 0;
				}
				else
				{
					numericUpDown_productId.Value = int.Parse(Options.SearchCriteriaProductId);
					comboBox_product.SelectedItem = Options.SearchCriteriaProductName;
				}

				// Owner Id update
				if (string.IsNullOrEmpty(Options.SearchCriteriaOwnerId.Trim()))
				{
					numericUpDown_ownerId.Value = 0;
					comboBox_owner.SelectedIndex = 0;
				}
				else
				{
					numericUpDown_ownerId.Value = int.Parse(Options.SearchCriteriaOwnerId);
					comboBox_owner.SelectedItem = Options.SearchCriteriaOwnerName;
				}

				numericUpDown_ownerId.Value = Options.SearchCriteriaOwnerId.Trim() != string.Empty ? Convert.ToInt64(Options.SearchCriteriaOwnerId) : 0;
				numericUpDown_providerId.Value = Options.SearchCriteriaProviderId.Trim() != string.Empty ? Convert.ToInt64(Options.SearchCriteriaProviderId) : 0;
				numericUpDown_productId.Value = Options.SearchCriteriaProductId.Trim() != string.Empty ? Convert.ToInt64(Options.SearchCriteriaProductId) : 0;

				comboBox_provider.Text = Options.SearchCriteriaProviderName;
				comboBox_owner.Text = Options.SearchCriteriaOwnerName;
				comboBox_product.Text = Options.SearchCriteriaProductName;

				numericUpDown_searchTimeout.Value = Options.SearchTimeout.Trim() != string.Empty ? Convert.ToInt64(Options.SearchTimeout) : 8;

				checkBox_ignoreTranslatedSegments.Checked = Options.IgnoreTranslatedSegments.Trim() == "True";

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void bnt_OK_Click(object sender, EventArgs e)
		{
			Options.ConnectionAppKey = textBox_applicationKey.Text;
			Options.ConnectionUserName = textBox_userName.Text;
			Options.ConnectionUserPassword = textBox_password.Text;
			Options.ConnectionAuthKey = textBox_authKey.Text;

			Options.SearchCriteriaContentTypeId = numericUpDown_contentTypeId.Value.ToString(CultureInfo.InvariantCulture);
			Options.SearchCriteriaContentTypeName = comboBox_contentType.SelectedItem != null ? comboBox_contentType.SelectedItem.ToString() : string.Empty;

			Options.SearchCriteriaIndustryId = numericUpDown_industryId.Value.ToString(CultureInfo.InvariantCulture);
			Options.SearchCriteriaIndustryName = comboBox_industry.SelectedItem != null ? comboBox_industry.SelectedItem.ToString() : string.Empty;

			Options.SearchCriteriaOwnerId = numericUpDown_ownerId.Value.ToString(CultureInfo.InvariantCulture);
			Options.SearchCriteriaOwnerName = comboBox_owner.SelectedItem != null ? comboBox_owner.SelectedItem.ToString() : string.Empty;

			Options.SearchCriteriaProviderId = numericUpDown_providerId.Value.ToString(CultureInfo.InvariantCulture);
			Options.SearchCriteriaProviderName = comboBox_provider.SelectedItem != null ? comboBox_provider.SelectedItem.ToString() : string.Empty;

			Options.SearchCriteriaProductId = numericUpDown_productId.Value.ToString(CultureInfo.InvariantCulture);
			Options.SearchCriteriaProductName = comboBox_product.SelectedItem != null ? comboBox_product.SelectedItem.ToString() : string.Empty;

			Options.SearchTimeout = numericUpDown_searchTimeout.Value.ToString(CultureInfo.InvariantCulture);
			Options.IgnoreTranslatedSegments = checkBox_ignoreTranslatedSegments.Checked ? "True" : "False";
		}

		private void button_saveGlobalSettings_Click(object sender, EventArgs e)
		{

			var f = new FormPasswordCheck();

			f.ShowDialog();
			if (!f.Saved) return;
			if (f.textBox_password.Text == textBox_password.Text)
			{

				#region  |  save information to the plugin default settings  |

				Options.ConnectionAppKey = textBox_applicationKey.Text;
				Options.ConnectionUserName = textBox_userName.Text;
				Options.ConnectionUserPassword = textBox_password.Text;
				Options.ConnectionAuthKey = textBox_authKey.Text;

				Options.SearchCriteriaContentTypeId = numericUpDown_contentTypeId.Value.ToString(CultureInfo.InvariantCulture);
				Options.SearchCriteriaContentTypeName = comboBox_contentType.SelectedItem.ToString();

				Options.SearchCriteriaIndustryId = numericUpDown_industryId.Value.ToString(CultureInfo.InvariantCulture);
				Options.SearchCriteriaIndustryName = comboBox_industry.SelectedItem.ToString();

				Options.SearchCriteriaOwnerId = numericUpDown_ownerId.Value.ToString(CultureInfo.InvariantCulture);
				Options.SearchCriteriaOwnerName = comboBox_owner.SelectedItem != null ? comboBox_owner.SelectedItem.ToString() : string.Empty;

				Options.SearchCriteriaProviderId = numericUpDown_providerId.Value.ToString(CultureInfo.InvariantCulture);
				Options.SearchCriteriaProviderName = comboBox_provider.SelectedItem != null ? comboBox_provider.SelectedItem.ToString() : string.Empty;


				Options.SearchCriteriaProductId = numericUpDown_productId.Value.ToString(CultureInfo.InvariantCulture);
				Options.SearchCriteriaProductName = comboBox_product.SelectedItem != null ? comboBox_product.SelectedItem.ToString() : string.Empty;

				Options.SearchTimeout = numericUpDown_searchTimeout.Value.ToString(CultureInfo.InvariantCulture);
				Options.IgnoreTranslatedSegments = checkBox_ignoreTranslatedSegments.Checked ? "True" : "False";

				var tausTmProvider = new Processor();
				var searchSettings = new SearchSettings();

				searchSettings.Timeout = Convert.ToInt32(Options.SearchTimeout);
				searchSettings.IgnoreTranslatedSegments = Options.IgnoreTranslatedSegments;
				searchSettings.AppKey = Options.ConnectionAppKey;
				searchSettings.UserName = Options.ConnectionUserName;
				searchSettings.Password = Options.ConnectionUserPassword;
				searchSettings.AuthKey = Options.ConnectionAuthKey;

				searchSettings.IndustryId = Convert.ToInt64(Options.SearchCriteriaIndustryId) > 0 ? Options.SearchCriteriaIndustryId : string.Empty;
				searchSettings.ContentTypeId = Convert.ToInt64(Options.SearchCriteriaContentTypeId) > 0 ? Options.SearchCriteriaContentTypeId : string.Empty;
				searchSettings.ProviderId = Convert.ToInt64(Options.SearchCriteriaProviderId) > 0 ? Options.SearchCriteriaProviderId.ToString() : string.Empty;
				searchSettings.OwnerId = Convert.ToInt64(Options.SearchCriteriaOwnerId) > 0 ? Options.SearchCriteriaOwnerId : string.Empty;
				searchSettings.ProductId = Convert.ToInt64(Options.SearchCriteriaProductId) > 0 ? Options.SearchCriteriaProductId : string.Empty;

				searchSettings.IndustryName = comboBox_industry.SelectedItem.ToString();
				searchSettings.ContentTypeId = comboBox_contentType.SelectedItem.ToString();
				searchSettings.ProviderId = comboBox_provider.SelectedItem != null ? comboBox_provider.SelectedItem.ToString() : string.Empty;
				searchSettings.OwnerId = comboBox_owner.SelectedItem != null ? comboBox_owner.SelectedItem.ToString() : string.Empty;
				searchSettings.ProductId = comboBox_product.SelectedItem != null ? comboBox_product.SelectedItem.ToString() : string.Empty;

				tausTmProvider.SaveSettings(searchSettings);
				#endregion
			}
			else
			{
				MessageBox.Show(this, @"Unable to save the Global Settings; the password does not match the one specified in the ‘Connection Credentials’", "TAUS Search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			//ignore
		}

		private void TausProviderConfDialog_Load(object sender, EventArgs e)
		{
			Text = @"TAUS Search Plugin for Studio";

			CheckEnableOkButton();
		}

		private void NewAuthorizationkey()
		{
			try
			{
				textBox_authKey.Text = string.Empty;
				Options.ConnectionAuthKey = textBox_authKey.Text;

				textBox_authKey.Text = Processor.GetAuthorizationKey(textBox_userName.Text, textBox_password.Text, textBox_applicationKey.Text);


				if (textBox_userName.Text.Trim() != string.Empty
						 && textBox_password.Text.Trim() != string.Empty
						 && textBox_authKey.Text.Trim() != string.Empty)
				{
					pictureBox_authorizationImage_01.Visible = false;
					pictureBox_authorizationImage_02.Visible = true;

					linkLabel_viewAuthorizationKey.Enabled = true;
					label_authorization_message.Text = @"authorization key exists...";
				}
				else
				{
					pictureBox_authorizationImage_01.Visible = true;
					pictureBox_authorizationImage_02.Visible = false;

					linkLabel_viewAuthorizationKey.Enabled = false;

					label_authorization_message.Text = @"create an authorization key...";
				}

			}
			catch (Exception ex)
			{
				Options.ConnectionAuthKey = textBox_authKey.Text;
				textBox_authKey.Text = string.Empty;

				pictureBox_authorizationImage_01.Visible = true;
				pictureBox_authorizationImage_02.Visible = false;
				linkLabel_viewAuthorizationKey.Enabled = false;
				label_authorization_message.Text = @"create an authorization key...";


				MessageBox.Show(this, ex.Message, @"Taus Translation Provider", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			finally
			{
				Options.ConnectionAuthKey = textBox_authKey.Text;
				CheckEnableOkButton();
			}
		}

		private void CheckEnableOkButton()
		{
			bnt_OK.Enabled = textBox_authKey.Text.Trim() != string.Empty;
		}
		private void textBox_userName_TextChanged(object sender, EventArgs e)
		{
			CheckEnableOkButton();
		}

		private void textBox_password_TextChanged(object sender, EventArgs e)
		{
			CheckEnableOkButton();
		}

		private void linkLabel_createNewLoginCredentials_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.tausdata.org/index.php/component/users/?view=registration");
		}

		private void linkLabel_createNewAuthorizationKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			NewAuthorizationkey();
		}


		private void button_Help_Click(object sender, EventArgs e)
		{
			try
			{
				const string helpFileOut = @"TAUS.Search.html";
				const string helpFileIn = @"Taus.Translation.Provider.TAUS.Search.html";

				var helpFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Taus.Translation.Provider");

				if (!Directory.Exists(helpFolder))
					Directory.CreateDirectory(helpFolder);

				var helpFilePath = Path.Combine(helpFolder, helpFileOut);
				if (File.Exists(helpFilePath))
					File.Delete(helpFilePath);

				var asb = Assembly.GetExecutingAssembly();
				using (var inputStream = asb.GetManifestResourceStream(helpFileIn))
				{

					Stream outputStream = File.Open(helpFilePath, FileMode.Create);

					if (inputStream != null)
					{
						var bsInput = new BufferedStream(inputStream);
						var bsOutput = new BufferedStream(outputStream);

						var buffer = new byte[1024];
						int bytesRead;

						while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
						{
							bsOutput.Write(buffer, 0, bytesRead);
						}

						bsInput.Flush();
						bsOutput.Flush();
						bsInput.Close();
						bsOutput.Close();
					}
				}

				System.Diagnostics.Process.Start(helpFilePath);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private void linkLabel_clearConnectionSettings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{

			textBox_userName.Text = string.Empty;
			textBox_password.Text = string.Empty;
			textBox_authKey.Text = string.Empty;

			Options.ConnectionUserName = string.Empty;
			Options.ConnectionUserPassword = string.Empty;
			Options.ConnectionAuthKey = string.Empty;

			Processor.DeleteSettingsFile();

			bnt_OK_Click(new object(), new EventArgs());
			DialogResult = DialogResult.OK;

			Close();
		}

		private void linkLabel_viewAuthorizationKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var f = new FormPasswordCheck
			{
				label_message =
				{
					Text =
						@"It is required to retype the password that is present in the ‘Connection Credentials’ to view the Authorization key"
				}
			};

			f.ShowDialog();

			if (f.Saved)
			{
				if (f.textBox_password.Text == textBox_password.Text)
				{
					MessageBox.Show(this, @"Authorization key: " + textBox_authKey.Text, @"TAUS Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(this, @"Unable to view Authorization key; the password does not match the one specified in the ‘Connection Credentials’", "TAUS Search", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		private void button_about_Click(object sender, EventArgs e)
		{
			var f = new TausSearchAbout();
			f.ShowDialog();
		}

		private void comboBox_industry_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var value in TausTranslationProvider.AttributesIndustry)
			{
				if (string.Compare(value.Value, comboBox_industry.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				numericUpDown_industryId.Value = int.Parse(value.Key);
				break;
			}
		}

		private void comboBox_contentType_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var value in TausTranslationProvider.AttributesContentType)
			{
				if (string.Compare(value.Value, comboBox_contentType.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				numericUpDown_contentTypeId.Value = int.Parse(value.Key);
				break;
			}
		}

		private void comboBox_provider_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var provider in _providers)
			{
				if (string.Compare(provider.Value, comboBox_provider.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				numericUpDown_providerId.Value = int.Parse(provider.Key);
				break;
			}
		}

		private void comboBox_product_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var product in _products)
			{
				if (string.Compare(product.Value, comboBox_product.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				numericUpDown_productId.Value = int.Parse(product.Key);
				break;
			}
		}

		private void comboBox_owner_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var owner in _owners)
			{
				if (string.Compare(owner.Value, comboBox_owner.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
				{
					continue;
				}
				numericUpDown_ownerId.Value = int.Parse(owner.Key);
				break;
			}
		}

		private void numericUpDown_providerId_KeyUp(object sender, KeyEventArgs e)
		{
			if (string.Compare(numericUpDown_providerId.Value.ToString(CultureInfo.InvariantCulture), Options.SearchCriteriaProviderId, StringComparison.OrdinalIgnoreCase) != 0)
			{
				comboBox_provider.SelectedItem = string.Empty;
			}
		}

		private void numericUpDown_productId_KeyUp(object sender, KeyEventArgs e)
		{
			if (string.Compare(numericUpDown_productId.Value.ToString(CultureInfo.InvariantCulture), Options.SearchCriteriaProductId, StringComparison.OrdinalIgnoreCase) != 0)
			{
				comboBox_product.SelectedItem = string.Empty;
			}
		}

		private void numericUpDown_ownerId_KeyUp(object sender, KeyEventArgs e)
		{
			if (string.Compare(numericUpDown_ownerId.Value.ToString(CultureInfo.InvariantCulture), Options.SearchCriteriaOwnerId, StringComparison.OrdinalIgnoreCase) != 0)
			{
				comboBox_owner.SelectedItem = string.Empty;
			}
		}
	}
}