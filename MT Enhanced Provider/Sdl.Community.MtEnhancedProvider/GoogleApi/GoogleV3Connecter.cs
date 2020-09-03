using NLog;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.GoogleApi
{
	public class GoogleV3Connecter: IGoogleV3Connecter
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _jsonFilePath;
		private readonly string _projectName;

		public GoogleV3Connecter(string projectName,string jsonFilePath)
		{
			_projectName = projectName;
			_jsonFilePath = jsonFilePath;
		}

		public void GetAvailableLanguages()
		{
			
		}
	}
}
