using System.Collections.ObjectModel;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	interface ILanguageMappingSettings
	{
		ObservableCollection<LanguageMappingModel> LanguageMappings { get; set; }
	}
}