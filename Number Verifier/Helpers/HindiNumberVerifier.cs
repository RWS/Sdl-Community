using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Helpers
{
	public class HindiNumberVerifier : IHindiNumberVerifier
	{
		public CurrentProjectAdaptee GetProjectInfo()
		{
			CurrentProjectAdaptee model = new CurrentProjectAdaptee();
			return model.GetProjectInformation();
		}
	}
}
