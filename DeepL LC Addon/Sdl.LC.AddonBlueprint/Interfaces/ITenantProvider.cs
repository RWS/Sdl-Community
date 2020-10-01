using Sdl.LC.AddonBlueprint.DAL;
using System.Threading.Tasks;

namespace Sdl.LC.AddonBlueprint.Interfaces
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
