namespace Auth0Service.Model
{
	public class AuthorizationSettings
	{
		public AuthorizationSettings(string clientId, string audience, string auth0Url) =>
			(ClientId, Audience, Auth0Url) = (clientId, audience, auth0Url);

		public string Audience { get; }
		public string Auth0Url { get; }
		public string ClientId { get; }
	}
}