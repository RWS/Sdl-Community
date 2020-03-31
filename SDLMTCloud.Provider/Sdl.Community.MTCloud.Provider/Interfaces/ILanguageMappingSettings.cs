using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	interface ILanguageMappingSettings
	{
		List<LanguageMappingModel> LanguageMappings { get; set; }
	}
}