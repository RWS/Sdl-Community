using Google.Cloud.AutoML.V1;

namespace GoogleCloudTranslationProvider.Models
{
	public class RetrievedCustomModel
	{
		public RetrievedCustomModel(CombinedModel model)
		{
			if (model is null || string.IsNullOrEmpty(model?.Dataset))
			{
				DisplayName = model is null ? PluginResources.RetrievedResources_NotAvailable
											: PluginResources.RetrievedResources_CustomModels_Unselected;
				return;
			}

			DatasetId = model.Dataset;
			DisplayName = model.DisplayName;
			ModelPath = model.Name;
			SourceLanguage = model.SourceLanguageCode;
			TargetLanguage = model.TargetLanguageCode;
		}

		public string DatasetId { get; set; }
		public string DisplayName { get; set; }
		public string ModelPath { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
	}
}