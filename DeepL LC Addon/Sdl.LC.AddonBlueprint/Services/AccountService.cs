using Sdl.LC.AddonBlueprint.DAL;
using Sdl.LC.AddonBlueprint.Exceptions;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sdl.LC.AddonBlueprint.Services
{
    /// <summary>
    /// Defines a service that allows creation and manipulation of account related data.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IRepository _repository;

        /// <summary>
        /// The descriptor service.
        /// </summary>
        private readonly IDescriptorService _descriptorService;

        /// <summary>
        /// The secret mask const string.
        /// </summary>
        private const string SecretMask = "*****";

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="descriptorService">The descriptor service.</param>
        public AccountService(IRepository repository, IDescriptorService descriptorService)
        {
            _repository = repository;
            _descriptorService = descriptorService;
        }

        /// <summary>
        /// Saves the account information.
        /// </summary>
        /// <param name="activatedEvent">The activated event.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task SaveAccountInfo(ActivatedEvent activatedEvent, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(activatedEvent.PublicKey))
            {
                throw new AddonValidationException($"Invalid {nameof(activatedEvent.PublicKey)} provided.", new Details { Code = ErrorCodes.InvalidInput, Name = nameof(activatedEvent.PublicKey), Value = null });
            }

            if (string.IsNullOrEmpty(activatedEvent.TenantId))
            {
                throw new AddonValidationException($"Invalid {nameof(activatedEvent.TenantId)} provided.", new Details { Code = ErrorCodes.InvalidInput, Name = nameof(activatedEvent.TenantId), Value = null });
            }

            if (string.IsNullOrEmpty(activatedEvent.ClientCredentials?.ClientId))
            {
                throw new AddonValidationException($"Invalid {nameof(activatedEvent.ClientCredentials.ClientId)} provided.", new Details { Code = ErrorCodes.InvalidInput, Name = nameof(activatedEvent.ClientCredentials.ClientId), Value = null });
            }

            if (string.IsNullOrEmpty(activatedEvent.ClientCredentials?.ClientSecret))
            {
                throw new AddonValidationException($"Invalid {nameof(activatedEvent.ClientCredentials.ClientSecret)} provided.", new Details { Code = ErrorCodes.InvalidInput, Name = nameof(activatedEvent.ClientCredentials.ClientSecret), Value = null });
            }

            AccountInfoEntity accountInfoEntity = new AccountInfoEntity()
            {
                PublicKey = activatedEvent.PublicKey,
                TenantId = activatedEvent.TenantId,
                ClientCredentials = activatedEvent.ClientCredentials
            };

            var accountInfo = await _repository.GetAccountInfoByTenantId(activatedEvent.TenantId).ConfigureAwait(false);
            if (accountInfo != null)
            {
                throw new AccountValidationException($"Account {activatedEvent.TenantId} is already activated.", new Details { Code = ErrorCodes.AccountAlreadyActivated, Name = nameof(activatedEvent.TenantId), Value = activatedEvent.TenantId });
            }

            await _repository.SaveAccount(accountInfoEntity).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the account information.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task RemoveAccountInfo(string tenantId, CancellationToken cancellationToken)
        {
            await _repository.RemoveAccount(tenantId).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes all the tenant related information.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RemoveAccounts(CancellationToken cancellationToken)
        {
            await _repository.RemoveAccounts().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the configuration settings.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The configuration settings result.</returns>
        public async Task<ConfigurationSettingsResult> GetConfigurationSettings(string tenantId, CancellationToken cancellationToken)
        {
            var accountInfo = await _repository.GetAccountInfoByTenantId(tenantId).ConfigureAwait(false);
            if (accountInfo == null)
            {
                throw new AccountValidationException($"Account {tenantId} is not activated!", new Details { Code = ErrorCodes.AccountNotActivated, Name = nameof(tenantId), Value = tenantId });
            }

            if (accountInfo.ConfigurationValues == null)
            {
                return new ConfigurationSettingsResult(new List<ConfigurationValueModel>());
            }

            return MaskSecretConfigurations(accountInfo.ConfigurationValues);
        }

        /// <summary>
        /// Saves or updates the configuration settings
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated configuration settings result.</returns>
        public async Task<ConfigurationSettingsResult> SaveOrUpdateConfigurationSettings(string tenantId, List<ConfigurationValueModel> configurationValues, CancellationToken cancellationToken)
        {
            AccountInfoEntity accountInfo = await _repository.GetAccountInfoByTenantId(tenantId).ConfigureAwait(false);
            if (accountInfo == null)
            {
                throw new AccountValidationException($"Account {tenantId} is not activated!", new Details { Code = ErrorCodes.AccountNotActivated, Name = nameof(tenantId), Value = tenantId });
            }

            accountInfo = UpdateConfigurationsForAccount(accountInfo, configurationValues);
            var updatedAccountInfoEntity = await _repository.SaveOrUpdateConfigurationSettings(accountInfo).ConfigureAwait(false);

            return MaskSecretConfigurations(updatedAccountInfoEntity.ConfigurationValues);
        }

        /// <summary>
        /// Updates the configurations for an account.
        /// </summary>
        /// <param name="accountInfo">The account information.</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <returns>The updated account info entity.</returns>
        private AccountInfoEntity UpdateConfigurationsForAccount(AccountInfoEntity accountInfo, List<ConfigurationValueModel> configurationValues)
        {
            if (accountInfo.ConfigurationValues == null)
            {
                accountInfo.ConfigurationValues = configurationValues;
            }
            else
            {
                foreach (var config in configurationValues)
                {
                    var matchedItem = accountInfo.ConfigurationValues.FirstOrDefault(f => f.Id == config.Id);
                    if (matchedItem == null)
                    {
                        accountInfo.ConfigurationValues.Add(config);
                    }
                    else
                    {
                        matchedItem.Value = config.Value;
                    }
                }
            }

            return accountInfo;
        }

        /// <summary>
        /// Masks the secret configuration values.
        /// </summary>
        /// <param name="configurations">A list of configurations</param>
        /// <returns></returns>
        private ConfigurationSettingsResult MaskSecretConfigurations(List<ConfigurationValueModel> configurations)
        {
            var secretConfigurationIds = _descriptorService.GetSecretConfigurations();

            foreach (var config in configurations.Where(config => secretConfigurationIds.Contains(config.Id)))
            {
                config.Value = SecretMask;
            }

            return new ConfigurationSettingsResult(configurations);
        }
    }
}
