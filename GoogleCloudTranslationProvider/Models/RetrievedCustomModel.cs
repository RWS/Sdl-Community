using Google.Cloud.AutoML.V1;

namespace GoogleCloudTranslationProvider.Models
{
	public class RetrievedCustomModel
	{
		private readonly Model _model;
		private readonly string _datasetId;
		private readonly string _displayName;
		private readonly string _modelPath;
		private readonly string _sourceLanguage;
		private readonly string _targetLanguage;

		public RetrievedCustomModel(Model model)
		{
			_model = model;
			if (_model is null || string.IsNullOrEmpty(_model?.DatasetId))
			{
				_displayName = _model is null ? PluginResources.RetrievedResources_CustomModels_Unavailable
											  : PluginResources.RetrievedResources_CustomModels_Unselected;
				return;
			}

			_datasetId = _model.DatasetId;
			_displayName = _model.DisplayName;
			_modelPath = _model.ModelName.ToString();
			_sourceLanguage = _model.TranslationModelMetadata.SourceLanguageCode;
			_targetLanguage = _model.TranslationModelMetadata.TargetLanguageCode;
		}

		public Model Model => _model;
		public string DatasetId => _datasetId;
		public string DisplayName => _displayName;
		public string ModelPath => _modelPath;
		public string SourceLanguage => _sourceLanguage;
		public string TargetLanguage => _targetLanguage;
	}
}