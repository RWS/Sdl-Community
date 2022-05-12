using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reports.Viewer.Api.Model
{
	public class Report : Sdl.ProjectAutomation.FileBased.Reports.Models.Report, INotifyPropertyChanged, ICloneable
	{
		private bool _isSelected;
		private string _xsltPath;
		private bool _isStudioReport;

		public Report()
		{
			Id = Guid.NewGuid();
			Date = DateTime.Now;
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

		public virtual string XsltPath
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

		public virtual bool IsStudioReport
		{
			get => _isStudioReport;
			set
			{
				if (_isStudioReport == value)
				{
					return;
				}

				_isStudioReport = value;
				OnPropertyChanged(nameof(IsStudioReport));
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


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public object Clone()
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
				XsltPath = XsltPath,
				IsStudioReport = IsStudioReport,
				IsCustomReport = IsCustomReport,
				IsSelected = IsSelected,
				TemplateId = TemplateId
			};
		}
	}
}
