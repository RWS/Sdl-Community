using System;
using System.Threading.Tasks;
using NSubstitute;
using Sdl.Community.MTCloud.Provider.Model;
using Xunit;

namespace Sdl.Community.MTCloud.Provider.UnitTests
{
	public class ConnectionServiceTests
	{
		private readonly Common _common;

		public ConnectionServiceTests()
		{
			_common = new Common();
		}

		[Theory(Skip = "Not valid anymore")]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void EnsureSignedIn_CredentialIsValid_ReturnsTrue(Authentication.AuthenticationType type)
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

				var languageCloudIdentityApiModel = new LanguageCloudCredentials
				{
					AccessToken = credential.Token,
					Email = credential.Name
				};
				connectionService.StudioSignIn().Returns((languageCloudIdentityApiModel, string.Empty));

				var userDetails = new UserDetails
				{
					AccountId = 0123,
					UserId = "abc123",
					ClientId = null
				};
				connectionService.GetUserDetails(Arg.Any<string>()).Returns(
					Task.FromResult((userDetails, string.Empty)));
			}

			//var result = connectionService.EnsureSignedIn(credential);
			//Assert.True(result.Item1, "Expected value: true");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Connect_CredentialIsValid_ReturnsTrue(Authentication.AuthenticationType type)
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

				var languageCloudIdentityApiModel = new LanguageCloudCredentials
				{
					AccessToken = credential.Token,
					Email = credential.Name
				};
				connectionService.StudioSignIn().Returns((languageCloudIdentityApiModel, string.Empty));

				var userDetails = new UserDetails
				{
					AccountId = 0123,
					UserId = "abc123",
					ClientId = null
				};
				connectionService.GetUserDetails(Arg.Any<string>()).Returns(
					Task.FromResult((userDetails, string.Empty)));
			}

			var result = connectionService.Connect(credential);
			Assert.True(result.Item1, "Expected value: true");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Connect_CredentialIsInvalidAndAttemptConnection_ReturnsFalse(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);
			credential.ValidTo = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0, 0, 0));

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			if (type == Authentication.AuthenticationType.Studio)
			{
				connectionService.IsValidCredential(out Arg.Any<string>()).Returns(x =>
				{
					x[0] = "Credential has expired";
					return false;
				});

				var languageCloudIdentityApiModel = new LanguageCloudCredentials
				{
					AccessToken = string.Empty,
					Email = string.Empty
				};
				connectionService.StudioSignIn()
					.Returns((languageCloudIdentityApiModel, "Invalid Credentials!"));
			}
			else
			{
				var authorizationResponse = new AuthorizationResponse
				{
					AccessToken = string.Empty
				};
				connectionService.SignIn(Arg.Any<string>(), Arg.Any<string>())
					.Returns(Task.FromResult((authorizationResponse, "Invalid Credentials!")));
			}

			var userDetails = new UserDetails
			{
				AccountId = 0123,
				UserId = type != Authentication.AuthenticationType.Client ? "abc123" : null,
				ClientId = type == Authentication.AuthenticationType.Client ? "abc123" : null,
			};
			connectionService.GetUserDetails(Arg.Any<string>())
				.Returns(Task.FromResult((userDetails, string.Empty)));

			var result = connectionService.Connect(credential);
			Assert.False(result.Item1, "Expected value: false");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.Studio)]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Connect_CredentialIsInvalidAndAttemptConnection_ReturnsTrue(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);
			credential.ValidTo = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0, 0, 0));

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);
			if (type == Authentication.AuthenticationType.Studio)
			{
				connectionService.IsValidCredential(out Arg.Any<string>()).Returns(x =>
				{
					x[0] = "Credential has expired";
					return false;
				});


				var languageCloudIdentityApiModel = new LanguageCloudCredentials
				{
					AccessToken = credential.Token,
					Email = credential.Name
				};
				connectionService.StudioSignIn().Returns((languageCloudIdentityApiModel, string.Empty));
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

			var result = connectionService.Connect(credential);
			Assert.True(result.Item1, "Expected value: true");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Credential_ExpiryDateIsValid_ReturnsTrue(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(out var message);
			Assert.True(result, "Expected value: true; the credential expiry should be evaluated as 'valid' (not expired)");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Credential_ExpiryDateIsValid_ReturnsFalse(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);
			credential.ValidTo = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0, 0, 0));

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(out var message);
			Assert.False(result, "Expected value: false; the credential expiry date should be evaluated as 'not valid' (expired)");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Credential_NameIsEmpty_ReturnsFalse(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);
			credential.Name = string.Empty;

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(out var message);
			Assert.False(result, "Expected value: false; the credential Name should be evaluated as 'not valid' (empty)");
		}

		[Theory]
		[InlineData(Authentication.AuthenticationType.User)]
		[InlineData(Authentication.AuthenticationType.Client)]
		public void Credential_PasswordIsEmpty_ReturnsFalse(Authentication.AuthenticationType type)
		{
			var credential = _common.GetDefaultCredential(type);
			credential.Password = string.Empty;

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(out var message);
			Assert.False(result, "Expected value: false; the credential Password should be evaluated as 'not valid' (empty)");
		}


		[Fact]
		public void GetCredential_IsSerializedCorrectly_ReturnsTrue()
		{
			var credential = _common.GetDefaultCredential(Authentication.AuthenticationType.Client);

			var connectionService = Substitute.For<Service.ConnectionService>(null, null, null);
			connectionService.Credential.Returns(credential);

			var strCredential = connectionService.CredentialToString();
			var result = connectionService.GetCredential(strCredential);

			Assert.True(result.Token == credential.Token, "Expected: '" + credential.Token + "' Found: '" + result.Token + "'");
			Assert.True(result.AccountId == credential.AccountId, "Expected: '" + credential.AccountId + "' Found: '" + result.AccountId + "'");
			Assert.True(result.Name == credential.Name, "Expected: '" + credential.Name + "' Found: '" + result.Name + "'");
			Assert.True(result.Password == credential.Password, "Expected: '" + credential.Password + "' Found: '" + result.Password + "'");
			Assert.True(result.Type.ToString() == credential.Type.ToString(), "Expected: '" + credential.Type + "' Found: '" + result.Type + "'");
			Assert.True(result.ValidTo.ToBinary() == credential.ValidTo.ToBinary(), "Expected: '" + credential.ValidTo + "' Found: '" + result.ValidTo + "'");
		}
	}
}
