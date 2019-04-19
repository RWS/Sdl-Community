using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Resource
	{
		private IResource _resourceType;

		public void SetResourceType(IResource resourceType)
		{
			_resourceType = resourceType;
		}

		public void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle)
		{
			_resourceType.AddLanguageResourceToBundle(langResBundle);
		}
	}
}