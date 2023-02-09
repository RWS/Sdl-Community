using System.Collections.Generic;
using System.Collections.ObjectModel;
using InterpretBank.SettingsService.Model;

namespace InterpretBank.TerminologyService
{
	public interface ISettingsService
	{
		List<string> GlossaryNames { get; set; }
		List<TagModel> Tags { get; set; }
	}
}