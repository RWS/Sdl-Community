using System.Collections.ObjectModel;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.ViewModel.Interface
{
	public interface IPairMappingViewModel
	{
		ObservableCollection<PairMapping> PairMappings { get; set; }
	}
}