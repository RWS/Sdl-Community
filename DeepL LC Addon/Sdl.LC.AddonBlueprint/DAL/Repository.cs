using MongoDB.Driver;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sdl.LC.AddonBlueprint.DAL
{
    public class Repository : IRepository
	{
		/// <summary>
		/// The accounts info entity collection.
		/// </summary>
		private readonly IMongoCollection<AccountInfoEntity> _accounts;

		/// <summary>
		/// The database context.
		/// </summary>
		private readonly IDatabaseContext _databaseContext;

		/// <summary>
		/// Initializes a new instance of the <see cref="Repository"/> class.
		/// </summary>
		/// <param name="databaseContext">The database context.</param>
		public Repository(IDatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
			_accounts = _databaseContext.Mongo.GetCollection<AccountInfoEntity>("Accounts");
		}

		/// <summary>
		/// Saves the account info entity.
		/// </summary>
		/// <param name="entity">The account info entity.</param>
		/// <returns></returns>
		public async Task SaveAccount(AccountInfoEntity entity)
		{
			await _accounts.InsertOneAsync(entity).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the account info entity by tenant id.
		/// </summary>
		/// <param name="tenantId">The tenant id.</param>
		/// <returns>The account info entity.</returns>
		public async Task<AccountInfoEntity> GetAccountInfoByTenantId(string tenantId)
		{
			var result = await _accounts.FindAsync(account => account.TenantId == tenantId).ConfigureAwait(false);
			return result.SingleOrDefault();
		}

		/// <summary>
		/// Removes the account by tenant id.
		/// </summary>
		/// <param name="tenantId">The tenant id.</param>
		/// <returns></returns>
		public async Task RemoveAccount(string tenantId)
		{
			await _accounts.FindOneAndDeleteAsync(account => account.TenantId == tenantId).ConfigureAwait(false);
		}

		/// <summary>
		/// Removes all the accounts.
		/// </summary>
		/// <returns></returns>
		public async Task RemoveAccounts()
		{
			await _databaseContext.Mongo.DropCollectionAsync("Accounts").ConfigureAwait(false);
		}

		/// <summary>
		/// Saves or updates the configurations settings.
		/// </summary>
		/// <param name="accountInfoEntity">The account info entity.</param>
		/// <returns>The updated account info entity.</returns>
		public async Task<AccountInfoEntity> SaveOrUpdateConfigurationSettings(AccountInfoEntity accountInfoEntity)
		{			
			var filter = Builders<AccountInfoEntity>.Filter
				.Where(accountInfo => accountInfo.TenantId == accountInfoEntity.TenantId);
			
			var options = new FindOneAndUpdateOptions<AccountInfoEntity>
			{
				ReturnDocument = ReturnDocument.After
			};

			var update = Builders<AccountInfoEntity>.Update
				.Set(account => account.TenantId, accountInfoEntity.TenantId)
				.Set(account => account.PublicKey, accountInfoEntity.PublicKey)
				.Set(account => account.ConfigurationValues, accountInfoEntity.ConfigurationValues);

			return await _accounts.FindOneAndUpdateAsync<AccountInfoEntity>(filter, update, options);
		}

		/// <summary>
		/// Gets the public key by tenant id.
		/// </summary>
		/// <param name="tenantId">The tenant id.</param>
		/// <returns>The public key.</returns>
		public async Task<string> GetPublicKeyByTenantId(string tenantId)
		{
			var result = await _accounts.FindAsync(account => account.TenantId == tenantId).ConfigureAwait(false);
			var publicKey = result.SingleOrDefault()?.PublicKey;

			return publicKey;
		}
	}
}
