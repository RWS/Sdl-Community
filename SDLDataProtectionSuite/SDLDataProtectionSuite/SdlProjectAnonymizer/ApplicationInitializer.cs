using System.IO;
using Newtonsoft.Json;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			CreateAcceptFile();

			var acceptWindow = new AcceptWindow();
			if (!AgreementMethods.UserAgreed())
			{
				acceptWindow.ShowDialog();
			}

			ActiveViewService.Instance.Initialize();
		}

		private static void CreateAcceptFile()
		{
			if (!Directory.Exists(Constants.AcceptFolderPath))
			{
				Directory.CreateDirectory(Constants.AcceptFolderPath);
			}

			if (File.Exists(Constants.AcceptFilePath))
			{
				return;
			}

			var file = File.Create(Constants.AcceptFilePath);
			file.Close();

			var accept = new Agreement
			{
				Accept = false
			};

			File.WriteAllText(Constants.AcceptFilePath, JsonConvert.SerializeObject(accept));
		}
	}
}
