using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TemplateModel
	{
		public TemplateModel(List<LanguageResourceBundle> resourceBundle)
		{
			ResourceBundles = resourceBundle;
		}

		public List<LanguageResourceBundle> ResourceBundles { get; set; }

		public void ApplyTmTemplate(List<TranslationMemory> translationMemories)
		{
			foreach (var languageResourceBundle in ResourceBundles)
			{
				foreach (var translationMemory in translationMemories)
				{
					var langDirOfTm = translationMemory.Tm.LanguageDirection;

					if (langDirOfTm.SourceLanguage.Equals(languageResourceBundle.Language) ||
					    langDirOfTm.TargetLanguage.Equals(languageResourceBundle.Language))
					{
						translationMemory.MarkTmApplied();
						translationMemory.Tm.LanguageResourceBundles.Add(languageResourceBundle);
						translationMemory.Tm.Save();
					}
					else
					{
						translationMemory.MarkTmNotApplied();
					}
				}
			}
		}
	}
}
