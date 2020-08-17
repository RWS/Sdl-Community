using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sdl.Reports.Viewer.API.Model
{
	public class Report : INotifyPropertyChanged
	{
		private string _name;
		private string _group;
		private string _language;
		private string _description;
		private string _path;
		private DateTime _date;
		private string _xslt;
		private bool _isSelected;
		private bool _isExtended;

		public Report()
		{
			Id = Guid.NewGuid().ToString();
			Date = DateTime.Now;
			Language = string.Empty;
		}

		public virtual string Id { get; internal set; }

		public virtual string Name
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

		public virtual string Group
		{
			get => _group;
			set
			{
				if (_group == value)
				{
					return;
				}

				_group = value ?? string.Empty;
				OnPropertyChanged(nameof(Group));
			}
		}

		public virtual string Language
		{
			get => _language;
			set
			{
				if (_language == value)
				{
					return;
				}

				_language = value ?? string.Empty;
				OnPropertyChanged(nameof(Language));
			}
		}

		public virtual string Description
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

		public virtual string Path
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

		public virtual string Xslt
		{
			get => _xslt;
			set
			{
				if (_xslt == value)
				{
					return;
				}

				_xslt = value;
				OnPropertyChanged(nameof(Xslt));
			}
		}

		public virtual DateTime Date
		{
			get => _date;
			internal set
			{
				_date = value;

				OnPropertyChanged(nameof(Date));
				OnPropertyChanged(nameof(DateToString));
			}
		}

		public virtual string DateToString
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

		public virtual string DateToShortString
		{
			get
			{
				var value = (Date != DateTime.MinValue && Date != DateTime.MaxValue)
					? Date.Year
					  + "-" + Date.Month.ToString().PadLeft(2, '0')
					  + "-" + Date.Day.ToString().PadLeft(2, '0')
					: "[none]";

				return value;
			}
		}

		public virtual bool IsSelected
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

		public virtual bool IsExpanded
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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public virtual object Clone()
		{
			return new Report
			{
				Id = Id,
				Name = Name,
				Description = Description,
				Group = Group,
				Language = Language,
				Date = new DateTime(Date.Ticks),
				Path = Path,
				Xslt = Xslt,
				IsExpanded = IsExpanded,
				IsSelected = IsSelected
			};
		}
	}
}
