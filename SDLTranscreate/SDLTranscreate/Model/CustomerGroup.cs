using System.Collections.Generic;

namespace Trados.Transcreate.Model
{
	public class CustomerGroup : BaseModel
	{
		private Customer _customer;

		private List<Interfaces.IProject> _projects;	

		public Customer Customer
		{
			get => _customer;
			set
			{
				_customer = value;
				OnPropertyChanged(nameof(Customer));
			}
		}

		public bool IsSelected { get; set; }

		public bool IsExpanded { get; set; }

		public List<Interfaces.IProject> Projects
		{
			get => _projects;
			set
			{
				_projects = value;
				OnPropertyChanged(nameof(Projects));
			}
		}
	}
}
