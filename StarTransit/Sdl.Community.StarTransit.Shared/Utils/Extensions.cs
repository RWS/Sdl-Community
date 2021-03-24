using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Utils
{
	public static class Extensions
	{
		public static List<Customer> GetStudioCustomers(FileBasedProject project)
		{
			var customers = new List<Customer>();

			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.Instance | BindingFlags.NonPublic);
			if (internalProjectField == null) return customers;
			dynamic internalDynamicaProject = internalProjectField.GetValue(project);
			dynamic customersList = internalDynamicaProject.ProjectsProvider?.CustomerProvider?.Customers;
			if (customersList == null) return customers;
			foreach (var customer in customersList)
			{
				customers.Add(new Customer {Guid = customer.Guid, Name = customer.Name, Email = customer.Email});
			}

			return customers;
		}

		public static void SetCustomer(this FileBasedProject project, Customer customerModel)
		{
			var allCustomers = GetStudioCustomers(project);
			var customer = allCustomers.FirstOrDefault(c =>
				c.Name.Equals(customerModel.Name) && c.Guid.Equals(customerModel.Guid));
			if (customer == null) return;
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.Instance | BindingFlags.NonPublic);
			if (internalProjectField == null) return;
			dynamic internalDynamicaProject = internalProjectField.GetValue(project);
			dynamic customersList = internalDynamicaProject.ProjectsProvider?.CustomerProvider?.Customers;
			if (customersList == null) return;
			foreach (var dynCustomer in customersList)
			{
				if (!dynCustomer.Guid.Equals(customer.Guid)) continue;
				internalDynamicaProject.ChangeCustomer(dynCustomer);
				break;
			}
		}
	}
}