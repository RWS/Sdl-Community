using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Multilingual.Excel.FileType.Models;

namespace Multilingual.Excel.FileType.FileType.ViewModels
{
	public class AppendPlaceablePatternViewModel : INotifyPropertyChanged
	{
		private readonly List<PlaceholderPattern> _placeablePatterns;
		private readonly PlaceholderPattern _selectedPlaceholderPattern;
		private List<SegmentationHintItem> _segmentationHintItems;
		private SegmentationHintItem _segmentationHintItem;
		private string _windowTitle;
		private string _pattern;
		private string _description;
		private bool _isEditMode;

		public AppendPlaceablePatternViewModel(PlaceholderPattern selectedPlaceholderPattern,
			List<PlaceholderPattern> placeablePatterns, bool isEditMode)
		{
			_segmentationHintItems = SegmentationHintItem.GetSegmentationHintItems();
		
			_selectedPlaceholderPattern = selectedPlaceholderPattern ?? new PlaceholderPattern();
			_placeablePatterns = placeablePatterns ?? new List<PlaceholderPattern>();

			IsEditMode = isEditMode;

			Pattern = selectedPlaceholderPattern?.Pattern;
			Description = selectedPlaceholderPattern?.Description;
			SegmentationHintItem = _segmentationHintItems.FirstOrDefault(a =>
				a.Key == selectedPlaceholderPattern?.SegmentationHint.ToString());

			WindowTitle = IsEditMode ? PluginResources.WindowTitle_Edit_PlaceholderPattern : PluginResources.WindowTitle_Add_PlaceholderPattern;
		}

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				if (_isEditMode == value)
				{
					return;
				}

				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}

		public string Pattern
		{
			get => _pattern;
			set
			{
				if (_pattern == value)
				{
					return;
				}

				_pattern = value;
				OnPropertyChanged(nameof(Pattern));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<SegmentationHintItem> SegmentationHintItems
		{
			get => _segmentationHintItems;
			set
			{
				if (_segmentationHintItems == value)
				{
					return;
				}

				_segmentationHintItems = value;
				OnPropertyChanged(nameof(SegmentationHintItems));
				OnPropertyChanged(nameof(IsValid));
			}
		}
		
		public SegmentationHintItem SegmentationHintItem
		{
			get => _segmentationHintItem;
			set
			{
				if (_segmentationHintItem == value)
				{
					return;
				}

				_segmentationHintItem = value;
				OnPropertyChanged(nameof(SegmentationHintItem));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				if (_description == value)
				{
					return;
				}

				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		public bool IsValid
		{
			get
			{
				if (IsEditMode)
				{
					if (_selectedPlaceholderPattern.Pattern != Pattern &&
						_placeablePatterns.Exists(a => a.Pattern == Pattern))
					{
						return false;
					}
				}
				else if (_placeablePatterns.Exists(a => a.Pattern == Pattern))
				{
					return false;
				}

				return true;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
