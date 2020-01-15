using System.Collections.ObjectModel;
using Sdl.Community.BeGlobalV4.Provider.Model;

namespace Sdl.Community.BeGlobalV4.Provider.Interfaces
{
	interface ILanguageMappingSettings
	{
		ObservableCollection<LanguageMappingModel> LanguageMappings { get; set; }
	}
}