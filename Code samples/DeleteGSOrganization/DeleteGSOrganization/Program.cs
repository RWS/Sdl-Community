using System;
using System.Collections.Generic;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Models.Response;

namespace DeleteGSOrganization
{
	public class Program
	{
		static void Main(string[] args)
		{
			var token = GroupShareClient.GetRequestToken(
				"",
				"",
				new Uri("http://gs2017dev.sdl.com"),
				GroupShareClient.AllScopes).Result;

			var groupShareClient = GroupShareClient.AuthenticateClient(
				token,
				"",
				"",
				new Uri("http://gs2017dev.sdl.com"),
				GroupShareClient.AllScopes).Result;

			for (var i = 0; i < 200; i++)
			{
				var organization = new Organization
				{
					UniqueId = Guid.NewGuid(),
					Name = $"NewCreatedOrg{i}",
					IsLibrary = true,
					Description = null,
					Path = null,
					ParentOrganizationId = new Guid("5bdb10b8-e3a9-41ae-9e66-c154347b8d17"),
					ChildOrganizations = null,
					Tags = new List<string> { "tagTest" }
				};

				var organizationId =  groupShareClient.Organization.Create(organization).Result;
				 groupShareClient.Organization.DeleteOrganization(organizationId);
			}
		}
	}
}
