using System.Collections.Generic;
using System.Reflection;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.XLIFF.Manager.Service
{
	public class CustomerProvider
	{
		public CustomerModel GetProjectCustomer(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				var internalProjectCustomer = internalDynamicProject?.Customer;
				if (internalProjectCustomer != null)
				{
					return new CustomerModel
					{
						Name = internalProjectCustomer.Name,
						Email = internalProjectCustomer.Email,
						Id = internalProjectCustomer.Guid.ToString()
					};
				}
			}

			return null;
		}

		public List<CustomerModel> GetProjectCustomers(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				dynamic studioCustomers = internalDynamicProject?.ProjectsProvider?.CustomerProvider?.Customers;
				if (studioCustomers == null)
				{
					return null;
				}

				var customersModels = new List<CustomerModel>();
				foreach (var studioCustomer in studioCustomers)
				{
					var customer = new CustomerModel
					{
						Name = studioCustomer.Name,
						Email = studioCustomer.Email,
						Id = studioCustomer.Guid.ToString()
					};

					customersModels.Add(customer);
				}

				return customersModels;
			}

			return null;
		}
	}
}
