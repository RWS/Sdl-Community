using System.Collections.Generic;
using InterpretBank.SettingsService.Model;

namespace InterpretBank.SettingsService.ViewModel.Interface;

public interface ISubViewModel : IViewModel
{
	string Name { get; }
	void Setup(List<GlossaryModel> glossaries, List<TagModel> tags);
}