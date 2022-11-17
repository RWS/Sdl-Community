using System;
using System.Threading.Tasks;
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

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			if (type == Authentication.AuthenticationType.Studio)
			{
				connectionService.IsValidCredential(out Arg.Any<string>()).Returns(x =>
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

		[Theory]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void SignInCommand_CredentialIsValid_ReturnsTrue(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			
			if (type == Authentication.AuthenticationType.Studio)
			{
				connectionService.IsValidCredential(out Arg.Any<string>()).Returns(x =>
				{
					x[0] = "Credential is not valid!";
					return false;
				});

				var languageCloudIdentityApiModel = new LanguageCloudCredentials
				{
					AccessToken = credential.Token,
					Email = credential.Name
				};
				connectionService.StudioSignIn()
					.Returns((languageCloudIdentityApiModel, string.Empty));
			}
			else
			{
				var authorizationResponse = new AuthorizationResponse
				{
					AccessToken = credential.Token
				};
				connectionService.SignIn(Arg.Any<string>(), Arg.Any<string>())
					.Returns(Task.FromResult((authorizationResponse, string.Empty)));
			}

			var userDetails = new UserDetails
			{
				AccountId = 0123,
				UserId = type != Authentication.AuthenticationType.Client ? "abc123" : null,
				ClientId = type == Authentication.AuthenticationType.Client ? "abc123" : null,
			};
			connectionService.GetUserDetails(Arg.Any<string>())
				.Returns(Task.FromResult((userDetails, string.Empty)));

			var model = Substitute.For<CredentialsViewModel>(null, connectionService);
			Assert.False(model.IsSignedIn, "Expected: 'False' Found: 'True'");

			model.LoginCommand.Execute(null);
			Assert.True(model.IsSignedIn, "Expected: 'True' Found: 'False'");
		}
	}
}
