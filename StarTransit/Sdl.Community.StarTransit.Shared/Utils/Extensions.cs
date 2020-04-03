using System;
using System.Reflection;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Utils
{
	public static class Extensions
	{
		public static readonly Log Log = Log.Instance;

		public static void SetCustomer(this FileBasedProject project, Customer customerModel)
		{
			try
			{
				//Get internal project
				var projectType = project.GetType();
				var projectMemberInfos = projectType.GetMember("_project", BindingFlags.NonPublic | BindingFlags.Instance);
				var projectFieldInfo = (FieldInfo)projectMemberInfos[0];
				dynamic internalProject = projectFieldInfo.GetValue(project);
				var internalProjectType = projectFieldInfo.FieldType;

				//Create customer instance
				var xmlCustomerType = Type.GetType(String.Format("{0},{1}", "Sdl.ProjectApi.Implementation.Xml.Customer", "Sdl.ProjectApi.Implementation"), true);
				var xmlCustomer = Activator.CreateInstance(xmlCustomerType);
				xmlCustomerType.GetProperty("Guid").SetValue(xmlCustomer, customerModel.Guid);
				xmlCustomerType.GetProperty("Name").SetValue(xmlCustomer, customerModel.Name);
				xmlCustomerType.GetProperty("Email").SetValue(xmlCustomer, customerModel.Email);

				var customerType = Type.GetType(String.Format("{0}, {1}", "Sdl.ProjectApi.Implementation.Customer", "Sdl.ProjectApi.Implementation"), true);

				var customer = Activator.CreateInstance(customerType,
					BindingFlags.Instance | BindingFlags.NonPublic,
					null,
					new object[]
					{
						internalProject.ProjectServer
						, xmlCustomer
					},
					null,
					null);
				internalProjectType.GetProperty("Customer").SetValue(internalProject, customer);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetCustomer method: {ex.Message}\n {ex.StackTrace}");
			}
		}
	}
}