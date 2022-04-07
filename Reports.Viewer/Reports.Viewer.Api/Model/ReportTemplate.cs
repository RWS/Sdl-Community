using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Reports.Viewer.Api.Model
{
	public class ReportTemplate : INotifyPropertyChanged, ICloneable
	{
		public enum TemplateScope
		{
			All,
			StudioOnly,
			NonStudioOnly
		}

		private string _id;
		private string _path;
		private string _group;
		private string _language;
		private bool _isAvailable;
		private TemplateScope _scope;

		public ReportTemplate()
		{
			_id = Guid.NewGuid().ToString();
		}

		public string Id
		{
			get => _id;
			set
			{
				if (_id == value)
				{
					return;
				}

				_id = value;
				OnPropertyChanged(nameof(Id));
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

		public TemplateScope Scope
		{
			get => _scope;
			set
			{
				if (_scope == value)
				{
					return;
				}

				_scope = value;
				OnPropertyChanged(nameof(Scope));
			}
		}

		public bool IsAvailable
		{
			get => _isAvailable;
			set
			{
				if (_isAvailable == value)
				{
					return;
				}

				_isAvailable = value;
				OnPropertyChanged(nameof(IsAvailable));
			}
		}

		public object Clone()
		{
			var success = Enum.TryParse(Scope.ToString(), true, out TemplateScope scope);

			var reportTemplate = new ReportTemplate
			{
				Id = Id,
				Path = Path,
				Group = Group,
				Language = Language,
				Scope = success ? scope : TemplateScope.All,
				IsAvailable = IsAvailable
			};
			return reportTemplate;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
