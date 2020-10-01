namespace Sdl.LC.AddonBlueprint.Models
{
	public class ActivatedEvent
	{
		/// <summary>
		/// The public key.
		/// </summary>
		public string PublicKey { get; set; }		

		/// <summary>
		/// The tenant id.
		/// </summary>
		public string TenantId { get; set; }

		/// <summary>
		/// The client credentials.
		/// </summary>
		public ClientCredentials ClientCredentials { get; set; }
	}
}
