using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.NumberVerifier
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
        public static NumberVerifierSettings NumberVerifierSettings { get; set; }

		public static bool IsNumberVerifierView { get; set; } = false;

        public void Execute()
		{
			Log.Setup();
		}
	}
}