using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ILanguageMappingSettings
	{
		List<LanguageMappingModel> LanguageMappings { get; set; }
	}
}