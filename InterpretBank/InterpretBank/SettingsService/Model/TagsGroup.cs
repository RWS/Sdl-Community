using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.MultiSelectComboBox.API;

namespace InterpretBank.SettingsService.Model
{
	public class TagsGroup : IItemGroup
	{
		public int Order { get; set; }
		public string Name { get; }

		public TagsGroup(int index, string name)
		{
			Order = index;
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
