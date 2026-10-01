using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.ProjectAutomation.Core;
using TradosStudio.API.TranslationResources.Terminology;


namespace Sdl.Community.IATETerminologyProvider.Interface
{
	public interface IEUProvider
	{
		bool IsEULanguages(ILanguage source, ILanguage target);
	}
}
