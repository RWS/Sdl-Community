using System.Collections.Generic;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.Interface
{
	public interface ILanguageMappingViewModel
	{
		BaseModel ViewModel { get; set; }

		List<TradosToMTEdgeLanguagePair> LanguageMapping { get; set; }
	}
}