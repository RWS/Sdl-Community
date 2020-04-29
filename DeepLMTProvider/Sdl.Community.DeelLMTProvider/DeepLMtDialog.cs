using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider
{
    public partial class DeepLMtDialog : Form
    {
		private ITranslationProviderCredentialStore _credentialStore;
		public DeepLTranslationOptions Options { get; set; }
		public DeepLMtDialog(DeepLTranslationOptions options, ITranslationProviderCredentialStore  credentialStore)
		{

			InitializeComponent();
			mainTableLayout.CellPaint += MainTableLayout_CellPaint;
			_credentialStore = credentialStore;
			contentInformationLabl.Text = @"DeepL API is a paid automated translation service. To use this service, set up a DeepL account and create a API Key.";

			//programatically merge columns 
			foreach (Control control in contentLayoutPanel.Controls)
			{

				var index = mainTableLayout.GetRow(control);
				if (index != 1)
				{
					mainTableLayout.SetColumnSpan(control, 2);
					
				}
			}

			
			//read logo from resource and add it to image box
			using (var imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Sdl.Community.DeepLMTProvider.Resources.deepLResized.png"))
			{
				if (imgStream != null)
				{
					var image = new Bitmap(imgStream);
					logoPicture.Image = image;
					logoPicture.Height = image.Height;
					logoPicture.Width = image.Width;
				}
				
			}

			Options = options;
			if (options != null)
			{
				apiKey.Text = options.ApiKey;
			}
		}

		private void MainTableLayout_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			//fill first row with white
			if (e.Row == 0)
			{
				e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
			}
			//draw top line for rows starting from index 1
			if (e.Row > 0)
			{
				e.Graphics.DrawLine(Pens.LightGray, e.CellBounds.Location, new Point(e.CellBounds.Right, e.CellBounds.Top));
			}
			
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			if (!ValidateForm())
			{
				return;
			}
			Options.ApiKey = apiKey.Text.Trim();
		
		}

		private bool ValidateForm()
		{
			
			if (string.IsNullOrWhiteSpace(apiKey.Text))
			{
				MessageBox.Show(@"Api Key is required");
				DialogResult = DialogResult.None;
				return false;
			}
			return true;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://www.deepl.com/api-contact.html");
		}
	}
}
