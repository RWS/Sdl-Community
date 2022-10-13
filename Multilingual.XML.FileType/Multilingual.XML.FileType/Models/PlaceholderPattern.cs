using System;
using Multilingual.XML.FileType.Extensions;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Models
{
	public class PlaceholderPattern : BaseModel, ICloneable
	{
		private string _pattern;
		private string _description;
		private bool _selected;
		private int _order;
		private SegmentationHint _segmentationHint;
		
		public PlaceholderPattern()
		{
			
			_pattern = string.Empty;
			SegmentationHint = SegmentationHint.MayExclude;
			_description = string.Empty;
			_selected = false;
			_order = 0;
		}

		public SegmentationHint SegmentationHint
		{
			get => _segmentationHint;
			set
			{
				if (_segmentationHint == value)
				{
					return;
				}

				_segmentationHint = value;
				OnPropertyChanged(nameof(SegmentationHint));
				
			}
		}

		[JsonIgnore] 
		public string SegmentationHintName => SegmentationHint.ToString().SplitCapitalizedWords();


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

		public bool Selected
		{
			get => _selected;
			set
			{
				if (_selected == value)
				{
					return;
				}

				_selected = value;
				OnPropertyChanged(nameof(Selected));
			}
		}

		public int Order
		{
			get => _order;
			set
			{
				if (_order == value)
				{
					return;
				}

				_order = value;
				OnPropertyChanged(nameof(Order));
			}
		}

		public object Clone()
		{
			return new PlaceholderPattern
			{
				Pattern = Pattern,
				SegmentationHint = SegmentationHint,
				Description = Description,
				Selected = Selected,
				Order = Order
			};
		}
	}
}
