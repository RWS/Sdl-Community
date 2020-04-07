using NSubstitute;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Xunit;

namespace Sdl.Community.MTCloud.Provider.UnitTests
{
	public class CredentialsViewModelTests
	{
		private readonly Common _common;

		public CredentialsViewModelTests()
		{
			_common = new Common();
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Constructor_InitializesPropertiesCorrectly_ReturnsTrue(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);

			var connectionService = Substitute.For<Service.ConnectionService>(null, null);
			connectionService.Credential.Returns(credential);

			if (type == Authentication.AuthenticationType.Studio)
			{
				connectionService.IsValidStudioCredential(out Arg.Any<string>()).Returns(x =>
				{
					x[0] = string.Empty;
					return true;
				});
			}

			
			var model = Substitute.For<CredentialsViewModel>(null, connectionService);

			Assert.True(model.SelectedAuthentication.Type == credential.Type, "Expected: '" + credential.Type + "' Found: '" + model.SelectedAuthentication.Type + "'");

			switch (type)
			{
				case Authentication.AuthenticationType.Studio:
					Assert.True(model.StudioSignedInAs == credential.Name, "Expected: '" + credential.Name + "' Found: '" + model.StudioSignedInAs + "'");
					break;
				case Authentication.AuthenticationType.User:
					Assert.True(model.UserName == credential.Name, "Expected: '" + credential.Name + "' Found: '" + model.UserName + "'");
					Assert.True(model.UserPassword == credential.Password, "Expected: '" + credential.Password + "' Found: '" + model.UserPassword + "'");
					break;
				case Authentication.AuthenticationType.Client:

					Assert.True(model.ClientId == credential.Name, "Expected: '" + credential.Name + "' Found: '" + model.ClientId + "'");
					Assert.True(model.ClientSecret == credential.Password, "Expected: '" + credential.Password + "' Found: '" + model.ClientSecret + "'");
					break;				
			}		
		}
	}
}
