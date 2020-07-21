using System.Collections.Generic;

namespace Sdl.Community.Transcreate.Model
{
	public class CustomerGroup : BaseModel
	{
		private Customer _customer;
		private List<Project> _projects;	

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

		public List<Project> Projects
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
