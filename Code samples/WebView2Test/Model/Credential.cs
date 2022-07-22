namespace WebView2Test.Model
{
	public class Credential
	{
		public Credential(string token, string refreshToken) =>
			(Token, RefreshToken) = (token, refreshToken);

		public string RefreshToken { get; }
		public string Token { get; }
	}
}