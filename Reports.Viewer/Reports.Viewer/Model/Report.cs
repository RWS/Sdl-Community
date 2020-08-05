using System;

namespace Sdl.Community.Reports.Viewer.Model
{
	public class Report : BaseModel
	{
		private string _name;
		private string _group;
		private string _language;
		private string _description;
		private string _path;
		private DateTime _date;
		private string _xsltPath;
		private bool _isSelected;
		private bool _isExtended;

		public Report()
		{
			Date = DateTime.Now;
		}

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Group
		{
			get => _group;
			set
			{
				if (_group == value)
				{
					return;
				}

				_group = value;
				OnPropertyChanged(nameof(Group));
			}
		}

		public string Language
		{
			get => _language;
			set
			{
				if (_language == value)
				{
					return;
				}

				_language = value;
				OnPropertyChanged(nameof(Language));
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

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public string XsltPath
		{
			get => _xsltPath;
			set
			{
				if (_xsltPath == value)
				{
					return;
				}

				_xsltPath = value;
				OnPropertyChanged(nameof(XsltPath));
			}
		}

		public DateTime Date
		{
			get => _date;
			set
			{
				_date = value;

				OnPropertyChanged(nameof(Date));
				OnPropertyChanged(nameof(DateToString));
			}
		}

		public string DateToString
		{
			get
			{
				var value = (Date != DateTime.MinValue && Date != DateTime.MaxValue)
					? Date.Year
					  + "-" + Date.Month.ToString().PadLeft(2, '0')
					  + "-" + Date.Day.ToString().PadLeft(2, '0')
					  + " " + Date.Hour.ToString().PadLeft(2, '0')
					  + ":" + Date.Minute.ToString().PadLeft(2, '0')
					  + ":" + Date.Second.ToString().PadLeft(2, '0')
					: "[none]";

				return value;
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected == value)
				{
					return;
				}

				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public bool IsExpanded
		{
			get => _isExtended;
			set
			{
				if (_isExtended == value)
				{
					return;
				}

				_isExtended = value;
				OnPropertyChanged(nameof(IsExpanded));
			}
		}
	}
}
