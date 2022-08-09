using System.Collections.Generic;

namespace Auth0Service.Model
{
	public class AuthenticationResult
	{
		private static Dictionary<CredentialsValidity, string> Messages = new()
		{
			[CredentialsValidity.NotPresent] = "User is signed out!",
			[CredentialsValidity.Invalid] = "Credentials invalid",
			[CredentialsValidity.Valid] = "",
		};

		public AuthenticationResult(CredentialsValidity validity)
		{
			Validity = validity;
		}

		public enum CredentialsValidity
		{
			NotPresent,
			Invalid,
			Valid
		}

		public bool IsSuccessful => Validity == CredentialsValidity.Valid;
		public string Message => Messages[Validity];
		private CredentialsValidity Validity { get; }
	}
}