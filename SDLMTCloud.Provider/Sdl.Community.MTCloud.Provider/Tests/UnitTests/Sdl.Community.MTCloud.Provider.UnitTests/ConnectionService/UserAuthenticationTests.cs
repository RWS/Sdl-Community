using System;
using NSubstitute;
using Sdl.Community.MTCloud.Provider.Model;
using Xunit;

namespace Sdl.Community.MTCloud.Provider.UnitTests.ConnectionService
{
	public class UserAuthenticationTests
	{
		private readonly Common _common;
		public UserAuthenticationTests()
		{
			_common = new Common();
		}

		[Fact]
		public void UserCredential_ExpiryDateIsValid_ReturnTrue()
		{
			var credential = _common.GetDefaultCredential(Authentication.AuthenticationType.User);

			var connectionService = Substitute.For<Service.ConnectionService>(null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(credential.Type, out var message);
			Assert.True(result, "Expected value: true; the credential expiry should be evaluated as 'valid' (not expired)");
		}		

		[Fact]
		public void UserCredential_ExpiryDateIsValid_ReturnFalse()
		{
			var credential = _common.GetDefaultCredential(Authentication.AuthenticationType.User);			
			credential.ValidTo = DateTime.UtcNow.Subtract(new TimeSpan(0, 1, 0, 0, 0));			

			var connectionService = Substitute.For<Service.ConnectionService>(null, null);
			connectionService.Credential.Returns(credential);
			
			var result = connectionService.IsValidCredential(credential.Type, out var message);
			Assert.False(result, "Expected value: false; the credential expiry date should be evaluated as 'not valid' (expired)");
		}
		

		[Fact]
		public void UserCredential_NameIsValid_ReturnFalse()
		{
			var credential = _common.GetDefaultCredential(Authentication.AuthenticationType.User);
			credential.Name = string.Empty;			

			var connectionService = Substitute.For<Service.ConnectionService>(null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(credential.Type, out var message);
			Assert.False(result, "Expected value: false; the credential Name should be evaluated as 'not valid' (empty)");		
		}

		[Fact]
		public void UserCredential_PasswordIsValid_ReturnFalse()
		{
			var credential = _common.GetDefaultCredential(Authentication.AuthenticationType.User);			
			credential.Password = string.Empty;

			var connectionService = Substitute.For<Service.ConnectionService>(null, null);
			connectionService.Credential.Returns(credential);

			var result = connectionService.IsValidCredential(credential.Type, out var message);
			Assert.False(result, "Expected value: false; the credential Password should be evaluated as 'not valid' (empty)");
		}	
	}
}
