using System.Collections.Generic;
using InterpretBank.GlossaryService.DAL;

namespace InterpretBank.SettingsService.ViewModel.Interface
{
	public interface ISettingsViewModel : ISubViewModel
	{
		List<DbTag> SelectedTags { get; set; }
		List<DbGlossary> SelectedGlossaries { get; set; }
	}
}