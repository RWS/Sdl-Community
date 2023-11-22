using Google.Cloud.AutoML.V1;

namespace GoogleCloudTranslationProvider.Models
{
	public class RetrievedCustomModel
	{
		public RetrievedCustomModel(Model model)
		{
			Model = model;
			if (Model is null || string.IsNullOrEmpty(Model?.DatasetId))
			{
				DisplayName = Model is null ? PluginResources.RetrievedResources_NotAvailable
											: PluginResources.RetrievedResources_CustomModels_Unselected;
				return;
			}

			DatasetId = Model.DatasetId;
			DisplayName = Model.DisplayName;
			ModelPath = Model.ModelName.ToString();
			SourceLanguage = Model.TranslationModelMetadata?.SourceLanguageCode;
			TargetLanguage = Model.TranslationModelMetadata?.TargetLanguageCode;
		}

		public Model Model { get; set;  }
		public string DatasetId { get; set; }
		public string DisplayName { get; set; }
		public string ModelPath { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
	}
}