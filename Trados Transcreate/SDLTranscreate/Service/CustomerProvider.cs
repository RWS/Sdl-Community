using System.Collections.Generic;
using System.Reflection;
using Sdl.ProjectAutomation.FileBased;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Service
{
	public class CustomerProvider
	{
		public Customer GetProjectCustomer(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				var internalProjectCustomer = internalDynamicProject?.Customer;
				if (internalProjectCustomer != null)
				{
					return new Customer
					{
						Name = internalProjectCustomer.Name,
						Email = internalProjectCustomer.Email,
						Id = internalProjectCustomer.Guid.ToString()
					};
				}
			}

			return null;
		}

		public List<Customer> GetProjectCustomers(FileBasedProject project)
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

				var customersModels = new List<Customer>();
				foreach (var studioCustomer in studioCustomers)
				{
					var customer = new Customer
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
