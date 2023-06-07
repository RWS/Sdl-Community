using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModelsListBoxItem
	{
		public List<LanguageModel> LanguageModels { get; set; }
		public int SelectedIndex { get; set; }
	}
}
