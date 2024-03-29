﻿using InterpretBank.Model;
using InterpretBank.Studio;
using Sdl.MultiSelectComboBox.API;
using System.ComponentModel;
using System.Drawing;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModel : IItemGroupAware
    {
		public IItemGroup Group { get; set; }
		public int Index { get; set; }
		public string Name { get; set; }

		public override bool Equals(object obj)
        {
            if (obj is not LanguageModel model) return false;
			return model.Name == Name;
		}



        public Image Flag => StudioContext.GetLanguageFlag(Name);

        public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}