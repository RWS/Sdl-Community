using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.Interface
{
	public interface ILanguageMappingViewModel
	{
		BaseModel ViewModel { get; set; }

		ObservableCollection<TradosToMTEdgeLanguagePair> LanguageMapping { get; set; }
	}
}