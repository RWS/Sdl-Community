using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;

namespace GroupShareKitFieldTemplateExample.TranslationMemories
{
	public class FieldTemplatesProvider
	{
		private readonly string _userName;
		private readonly string _password;
		private readonly string _uri;

		public FieldTemplatesProvider(string userName, string password, string uri)
		{
			_userName = userName;
			_password = password;
			_uri = uri;
		}

		private async Task<GroupShareClient> GetGroupShareClient()
		{			
			var authentication = new Authentication();
			var client = await authentication.Login(_userName, _password, _uri);
			return client;
		}

		public async Task<string> CreateFieldTempalate(FieldTemplate fieldTemplate)
		{
			var client = await GetGroupShareClient();
			var templateId = await client.TranslationMemories.CreateFieldTemplate(fieldTemplate);
			return templateId;
		}

		public async Task<FieldTemplate> GetFieldTemplateById(string templateId)
		{			
			try
			{
				var client = await GetGroupShareClient();
				var fieldTemplate = await client.TranslationMemories.GetFieldTemplateById(templateId);
				return fieldTemplate;
			}
			catch
			{
				//ignore; catch all
			}

			return null;
		}

		public async Task DeleteFieldTempalate(string templateId)
		{
			try
			{
				var client = await GetGroupShareClient();
				await client.TranslationMemories.DeleteFieldTemplate(templateId);
			}
			catch
			{
				//ignore; catch all
			}			
		}

		public async Task<string> CreateFieldForTemplate(string templateId, FieldRequest fieldRequest)
		{
			var client = await GetGroupShareClient();
			var fieldId = await client.TranslationMemories.CreateFieldForTemplate(templateId, fieldRequest);
			return fieldId;
		}

		public async Task<IReadOnlyList<Field>> GetFieldsForTemplate(string templateId)
		{
			var client = await GetGroupShareClient();
			var fields = await client.TranslationMemories.GetFieldsForTemplate(templateId);
			return fields;
		}

		public async Task<Field> GetFieldForTemplate(string templateId, string fieldId)
		{			
			try
			{
				var client = await GetGroupShareClient();
				var field = await client.TranslationMemories.GetFieldForTemplate(templateId, fieldId);
				return field;
			}
			catch
			{
				//ignore; catch all
			}

			return null;
		}

		public async Task DeleteFieldForTemplate(string templateId, string fieldId)
		{			
			try
			{
				var client = await GetGroupShareClient();
				await client.TranslationMemories.DeleteFieldForTemplate(templateId, fieldId);
			}
			catch
			{
				//ignore; catch all
			}
		}
	}
}
