using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class Worker
	{
		private AnonymizerSettings _anonymizerSettings;
		private DataAnonymizer _dataAnonymizer;

		public Worker(AnonymizerSettings anonymizerSettings)
		{
			_anonymizerSettings = anonymizerSettings;
		}
		internal DataAnonymizer DataAnonymizerObj
		{
			get
			{
				if (_dataAnonymizer == null)
				{
					_dataAnonymizer = new DataAnonymizer();
				}
				_dataAnonymizer.Settings = _dataAnonymizer;
				return _dataAnonymizer;
			}
			set => _dataAnonymizer = value;
		}
		public bool GeneratePreviewFiles(string filePath, IMultiFileConverter multiFileConverter)
		{

			multiFileConverter.SynchronizeDocumentProperties();
			multiFileConverter.AddBilingualProcessor(new FileReader(DataAnonymizerObj, _anonymizerSettings, filePath));
			multiFileConverter.Parse();

			return true;
		}
	}
}
