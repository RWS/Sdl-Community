using Sdl.Community.DeeplAddon.DAL;
using Sdl.Community.DeeplAddon.Interfaces;
using System.Threading.Tasks;

namespace Sdl.Community.DeeplAddon.DAL
{
	public class TenantProvider : ITenantProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TenantProvider"/> class.
		/// </summary>
		/// <param name="repository">The tenant provider.</param>
		public TenantProvider(IRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// The repository.
		/// </summary>
		private IRepository _repository;

		/// <summary>
		/// Gets the tenant secret.
		/// </summary>
		/// <param name="tenantId">The tenant id.</param>
		/// <returns>The tenant secret.</returns>
		public async Task<TenantSecret> GetTenantSecret(string tenantId)
		{
			if (string.IsNullOrEmpty(tenantId))
			{
				return null;
			}

			var publicKey = await _repository.GetPublicKeyByTenantId(tenantId);
			return new TenantSecret
			{
				PublicKey = publicKey,
				TenantId = tenantId
			};
		}
	}
}
