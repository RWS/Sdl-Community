﻿using System.IO;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class User : ModelBase
	{
		private bool _isSelected;
		private string _name;
		private string _alias;

		public string TmFilePath { get; set; }

		public string TmName => TmFilePath != null ? Path.GetFileName(TmFilePath) : string.Empty;

		public bool IsSelected
		{

			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public string UserName
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(UserName));
			}
		}
		public string Alias
		{
			get => _alias;
			set
			{
				_alias = value;
				OnPropertyChanged(nameof(Alias));
			}
		}
	}
}
