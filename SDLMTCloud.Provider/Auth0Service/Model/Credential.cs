namespace Auth0Service.Model
{
	public class Credential
	{
		public Credential(string token, string refreshToken, string email) =>
			(Token, RefreshToken, Email) = (token, refreshToken, email);

		public string RefreshToken { get; }
		public string Token { get; }
		public string Email { get; }
	}
}