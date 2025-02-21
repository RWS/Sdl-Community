using System;

namespace Sdl.Community.StarTransit.Shared.Models
{
	public class ReturnFileDetails : BaseViewModel
	{
		private bool _isChecked;
		public string FileName { get; set; }
		public string Path { get; set; }
		public Guid Id { get; set; }

		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				if (_isChecked == value) return;
				_isChecked = value;
				OnPropertyChanged(nameof(IsChecked));
			}
		}
	}
}
