using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GroupShareKitFieldTemplateExample.TranslationMemories;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;

namespace GroupShareKitFieldTemplateExample
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= MainWindow_Loaded;

			URI.Text = "http://gs2017dev.sdl.com";
		}

		private async void RunTests()
		{
			try
			{
				IsEnabled = false;

				var millisecondsDelay = 2000;

				var userName = UserName.Text;
				var password = Password.Text;
				var uri = URI.Text;
				var templateOwnerId = TemplateOwnerId.Text;
				var templateLocation = TemplateLocation.Text;
				var templateName = TemplateName.Text;

				if (!IsValidParameters(userName, password, uri, templateOwnerId, templateLocation, templateName))
				{
					IsEnabled = true;
					return;
				}

				ClearTextBlock();

				var fieldTemplatesProvider = new FieldTemplatesProvider(userName, password, uri);

				var templateId = await CreateFieldTempalate(fieldTemplatesProvider);
				UpdateTextBlock("CreateFieldTempalate");
				UpdateTextBlock($"Added Template: {templateId}");
				UpdateTextBlock(string.Empty);
				await Task.Delay(millisecondsDelay);

				var fieldTemplate01 = await GetFieldTemplateById(fieldTemplatesProvider, templateId);
				UpdateTextBlock("GetFieldsForTemplate");
				UpdateTextBlock(fieldTemplate01 != null
					? $"Found Template: {templateId}"
					: $"Unable to locate Template: {templateId}");
				UpdateTextBlock(string.Empty);

				var fieldsTest01 = await GetFieldsForTemplate(fieldTemplatesProvider, templateId);
				UpdateTextBlock("GetFieldsForTemplate");
				UpdateTextBlock($"Found {fieldsTest01.Count} fields in Template: {templateId}");
				UpdateTextBlock(string.Empty);

				var fieldId = await CreateFieldForTemplate(fieldTemplatesProvider, templateId);
				UpdateTextBlock("CreateFieldForTemplate");
				UpdateTextBlock($"Added Field: {fieldId} in Template {templateId}");
				UpdateTextBlock(string.Empty);

				await Task.Delay(millisecondsDelay);

				var fieldsTest02 = await GetFieldsForTemplate(fieldTemplatesProvider, templateId);
				UpdateTextBlock("GetFieldsForTemplate");
				UpdateTextBlock($"Found {fieldsTest02.Count} fields in Template: {templateId}");
				UpdateTextBlock(string.Empty);

				var fieldTest01 = await GetFieldForTemplate(fieldTemplatesProvider, templateId, fieldId);
				UpdateTextBlock("GetFieldForTemplate");
				UpdateTextBlock(fieldTest01 != null
					? $"Found Field: {fieldId}"
					: $"Unable to locate Field: {fieldId}");
				UpdateTextBlock(string.Empty);

				await Task.Delay(millisecondsDelay);

				await DeleteFieldForTemplate(fieldTemplatesProvider, templateId, fieldId);
				UpdateTextBlock("DeleteFieldForTemplate");
				UpdateTextBlock($"Removed Field: {fieldId} from Template: {templateId}");
				UpdateTextBlock(string.Empty);

				await Task.Delay(millisecondsDelay);

				var fieldsTest03 = await GetFieldsForTemplate(fieldTemplatesProvider, templateId);
				UpdateTextBlock("GetFieldsForTemplate");
				UpdateTextBlock($"Found {fieldsTest03.Count} fields in Template: {templateId}");
				UpdateTextBlock(string.Empty);

				var fieldTest02 = await GetFieldForTemplate(fieldTemplatesProvider, templateId, fieldId);
				UpdateTextBlock("GetFieldForTemplate");
				UpdateTextBlock(fieldTest02 != null
					? $"Found Field: {fieldId}"
					: $"Unable to locate Field: {fieldId}");
				UpdateTextBlock(string.Empty);

				await DeleteFieldTempalate(fieldTemplatesProvider, templateId);
				UpdateTextBlock("DeleteFieldTempalate");
				UpdateTextBlock($"Removed Template: {templateId}");
				UpdateTextBlock(string.Empty);

				await Task.Delay(millisecondsDelay);

				var fieldTemplate02 = await GetFieldTemplateById(fieldTemplatesProvider, templateId);
				UpdateTextBlock("GetFieldTemplateById");
				UpdateTextBlock(fieldTemplate02 != null
					? $"Found Template: {templateId}"
					: $"Unable to locate Template: {templateId}");
				UpdateTextBlock(string.Empty);

				UpdateTextBlock("Done");

			}
			catch (Exception e)
			{
				UpdateTextBlock("Exception: " + e.Message);
			}
			finally
			{
				IsEnabled = true;
			}
		}

		private void UpdateTextBlock(string text)
		{
			TextBlock.Text += "\r\n" + text;
			ScrollViewer.ScrollToEnd();
		}

		private void ClearTextBlock()
		{
			TextBlock.Text = string.Empty;
		}

		private bool IsValidParameters(string userName, string password, string uri, 
			string templateOwnerId, string templateLocation, string templateName)
		{
			if (!TestIsValid("User Name", userName))
			{
				return false;
			}

			if (!TestIsValid("Password", password))
			{
				return false;
			}

			if (!TestIsValid("GroupShare URI", uri))
			{
				return false;
			}

			if (!TestIsValid("FieldTemplate Owner ID", templateOwnerId))
			{
				return false;
			}

			if (!TestIsValid("FieldTemplate Location", templateLocation))
			{
				return false;
			}

			if (!TestIsValid("FieldTemplate Name", templateName))
			{
				return false;
			}
			return true;
		}

		private bool TestIsValid(string lableName, string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				MessageBox.Show($"The {lableName} value cannot be null!");
				return false;
			}

			return true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			RunTests();
		}

		private async Task<string> CreateFieldTempalate(FieldTemplatesProvider fieldTemplatesProvider)
		{	
			var newFieldTemplate = new FieldTemplate
			{
				OwnerId = TemplateOwnerId.Text,
				Location = TemplateLocation.Text,
				Name = TemplateName.Text,
				Description = "Dev. Testing",
				IsTmSpecific = false
			};

			var fieldTemplateId = await fieldTemplatesProvider.CreateFieldTempalate(newFieldTemplate);

			return fieldTemplateId;
		}

		private async Task<FieldTemplate> GetFieldTemplateById(FieldTemplatesProvider fieldTemplatesProvider, string templateId)
		{
			var fieldTemplate = await fieldTemplatesProvider.GetFieldTemplateById(templateId);
			return fieldTemplate;
		}

		private async Task DeleteFieldTempalate(FieldTemplatesProvider fieldTemplatesProvider, string templateId)
		{
			await fieldTemplatesProvider.DeleteFieldTempalate(templateId);
		}

		private async Task<string> CreateFieldForTemplate(FieldTemplatesProvider fieldTemplatesProvider, string templateId)
		{
			var fieldRequest = new FieldRequest
			{
				Name = "IntegerFieldTest",
				Type = FieldRequest.TypeEnum.Integer,
				Values = new List<string>()
			};

			var fieldId = await fieldTemplatesProvider.CreateFieldForTemplate(templateId, fieldRequest);

			return fieldId;
		}

		private async Task<IReadOnlyList<Field>> GetFieldsForTemplate(FieldTemplatesProvider fieldTemplatesProvider, string templateId)
		{
			var fields = await fieldTemplatesProvider.GetFieldsForTemplate(templateId);
			return fields;
		}

		private async Task<Field> GetFieldForTemplate(FieldTemplatesProvider fieldTemplatesProvider, string templateId, string fieldId)
		{
			var field = await fieldTemplatesProvider.GetFieldForTemplate(templateId, fieldId);
			return field;
		}

		private async Task DeleteFieldForTemplate(FieldTemplatesProvider fieldTemplatesProvider, string templateId, string fieldId)
		{
			await fieldTemplatesProvider.DeleteFieldForTemplate(templateId, fieldId);
		}
	}
}
