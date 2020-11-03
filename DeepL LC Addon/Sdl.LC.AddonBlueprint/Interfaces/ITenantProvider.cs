using Sdl.Community.DeeplAddon.DAL;
using System.Threading.Tasks;

namespace Sdl.Community.DeeplAddon.Interfaces
{
    public interface ITenantProvider
	{
		/// <summary>
		/// Gets the tenant public key.
		/// </summary>
		/// <param name="tenantId">The tenant id.</param>
		/// <returns>The tenant secret.</returns>
		Task<TenantSecret> GetTenantSecret(string tenantId);
	}
}
