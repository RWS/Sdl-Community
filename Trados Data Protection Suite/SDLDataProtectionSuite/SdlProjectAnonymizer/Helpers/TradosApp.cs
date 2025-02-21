using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	// we can be launched from a non-UI context (like, from a console app, using the public API to run an automated task
	// if that is the case, the SdlTradosStudio.Application will always throw an exception
	internal class TradosApp
	{
		private static SdlTradosStudioApplication _tradosApp;
		private static bool _isNonUIContext = false;

		private static SdlTradosStudioApplication? App()
		{
			try
			{
				if (_isNonUIContext)
					return null;
				_tradosApp = SdlTradosStudio.Application;
				return _tradosApp;
			}
			catch (Exception exception)
			{
				_isNonUIContext = true;
				return null;
			}
		}

		public static T? GetController<T>() where T : AbstractController
		{
			var app = App();
			return app?.GetController<T>();
		}
	}
}
