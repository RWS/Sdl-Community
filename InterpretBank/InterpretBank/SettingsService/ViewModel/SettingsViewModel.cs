using System.Collections.Generic;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;

namespace InterpretBank.SettingsService.ViewModel;

public class SettingsViewModel : ViewModel, ISubViewModel
{
	private List<GlossaryModel> _glossaries;

	public List<GlossaryModel> Glossaries
	{
		get => _glossaries;
		set => SetField(ref _glossaries, value);
	}

	public List<DbTag> SelectedTags { get; set; }
	public List<TagModel> Tags { get; set; }

	public string Name => "Settings";

	public void Setup(List<GlossaryModel> glossaries, List<TagModel> tags)
	{
		Tags = tags;
		Glossaries = glossaries;
	}
}