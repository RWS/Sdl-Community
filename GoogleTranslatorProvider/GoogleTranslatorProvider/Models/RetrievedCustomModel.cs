using System.Data;
using Google.Cloud.AutoML.V1;

namespace GoogleTranslatorProvider.Models
{
	public class RetrievedCustomModel
	{
		public RetrievedCustomModel(Model model)
		{
			Model = model;
			if (model is null)
			{
				DisplayName = "No custom models available.";
				return;
			}

			if (string.IsNullOrEmpty(model?.DatasetId))
			{
				DisplayName = "No custom model selected.";
				return;
			}

			DatasetId = model.DatasetId;
			DisplayName = model.DisplayName;
			ModelPath = model.ModelName.ToString();
			SourceLanguage = model.TranslationModelMetadata.SourceLanguageCode;
			TargetLanguage = model.TranslationModelMetadata.TargetLanguageCode;
		}

		public Model Model { get; set; }
		public string DatasetId { get; set; }
		public string DisplayName { get; set; }
		public string ModelPath { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
	}
}