using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Helpers;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;
using Sdl.LC.AddonBlueprint.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System;

namespace Sdl.LC.AddonBlueprint.Controllers
{
	[Route("api")]
	[ApiController]
	public class StandardController : ControllerBase
	{
		private IConfiguration _configuration;	
		private readonly ILogger _logger;
		private readonly IDescriptorService _descriptorService;
		private readonly IAccountService _accountService;
		private readonly IHealthReporter _healthReporter;
		private readonly ITranslationService _translationService;
		private readonly ILanguageService _languageService;
		private const string ApiKeyId = "apikey";
		private const string FormalityId = "translationFormality";

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountService"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="descriptorService">The descriptor service.</param>
		/// <param name="accountService">The account service.</param>
		/// <param name="healthReporter">The health reporter.</param>
		public StandardController(IConfiguration configuration,
			ILogger<StandardController> logger,
			IDescriptorService descriptorService,
			IAccountService accountService,
			IHealthReporter healthReporter,ILanguageService languageService,ITranslationService translationService)
		{
			_configuration = configuration;
			_logger = logger;
			_descriptorService = descriptorService;
			_accountService = accountService;
			_healthReporter = healthReporter;
			_translationService = translationService;
			_languageService = languageService;
		}

		/// <summary>
		/// Gets the add-on descriptor.
		/// </summary>
		/// <returns>The descriptor</returns>
		[HttpGet("descriptor")]
		public IActionResult Descriptor()
		{
			// This endpoint provides the descriptor for the Language Cloud to inspect and register correctly.
			// It can be implemented in any number of ways. The example implementation is to load the descriptor.json file
			// into the AddonDescriptorModel object and then serialize it as a result.
			// Alternative implementation can be generating the descriptor based on config settings, environment variables,
			// etc.
			_logger.LogInformation("Entered Descriptor endpoint.");

			// Descriptor service will provide an object describing the descriptor.
			var descriptor = _descriptorService.GetDescriptor();

			var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
			{
				NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
				ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
				
			};

			return Content(Newtonsoft.Json.JsonConvert.SerializeObject(descriptor, serializerSettings), "application/json", Encoding.UTF8);
		}

		/// <summary>
		/// Gets the add-on health.
		/// </summary>
		/// <returns>200 status code if it's healthy.</returns>
		[HttpGet("health")]
		public IActionResult Health()
		{
			// This is a health check endpoint. In most cases returnin Ok is enough, but you might want to make checks
			// to resources this service uses, like: DB, message queues, storage etc.
			// Any response besides 200 Ok, will be considered as failure. As a suggestion use "return StatusCode(500);"
			// when you need to signal that the service is having health issues.

			var isHealthy = _healthReporter.IsServiceHealthy();
			if (isHealthy)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		/// <summary>
		/// This endpoint provides the documentation for the Add-On. It can return the HTML page with the documentation
		/// or redirect to a page. In this sample redirect is used with URL configured in appsettings.json
		/// </summary>
		[HttpGet("help")]
		public IActionResult Documentation()
		{
			return Redirect(_configuration.GetValue<string>("documentationUrl"));
		}

		/// <summary>
		/// Receive lifecycle events for the add-on.
		/// </summary>
		/// <returns></returns>
		[HttpPost("addon-lifecycle")]
		public async Task<IActionResult> AddonLifecycle()
		{
			string payload;
			using (var sr = new StreamReader(Request.Body))
			{
				payload = await sr.ReadToEndAsync();
			}

			var lifecycle = JsonSerializer.Deserialize<AddOnLifecycleEvent>(payload, JsonSettings.Default());
			switch (lifecycle.Id)
			{
				case AddOnLifecycleEventEnum.REGISTERED:
					_logger.LogInformation("Addon Registered Event Received.");
					// This is the event notifying that the Add-On has been registered in Language Cloud.
					// No further details are available for that event.
					break;
				case AddOnLifecycleEventEnum.ACTIVATED:
					// This is an Activation event, tenant id and it's public key must be saved to db.
					_logger.LogInformation("Addon Activated Event Received.");
					var activatedEvent = JsonSerializer.Deserialize<AddOnLifecycleEvent<ActivatedEvent>>(payload, JsonSettings.Default());

					 var tenantId = activatedEvent.Data.TenantId;
					_logger.LogInformation($"Addon Activated Event Received for tenant id {tenantId}.");

					await _accountService.SaveAccountInfo(activatedEvent.Data, CancellationToken.None).ConfigureAwait(false);
					break;
				case AddOnLifecycleEventEnum.UNREGISTERED:
					// This is the event notifying that the Add-On has been unregistered/deleted from Language Cloud.
					// No further details are available for that event.
					_logger.LogInformation("Addon Unregistered Event Received.");
					// All the tenant information should be removed.
					await _accountService.RemoveAccounts(CancellationToken.None).ConfigureAwait(false);
					break;
			}

			return Ok();
		}

		/// <summary>
		/// Receive lifecycle events for the account.
		/// </summary>
		/// <returns></returns>
		[Authorize]
		[HttpPost("account-lifecycle")]
		public async Task<IActionResult> AccountLifecycle()
		{
			// This endpoint receives events related to an Account where the Add-On has been activated
			// and the requests are authenticated.
			// Currently this is only the DEACTIVATED event.
			var tenantId = Request.HttpContext.User.Claims.Single(c => c.Type == "X-LC-Tenant").Value;

			var sr = new StreamReader(Request.Body);
			var payload = await sr.ReadToEndAsync();
			var lifecycle = JsonSerializer.Deserialize<AccountLifecycleEvent>(payload, JsonSettings.Default());

			switch (lifecycle.Id)
			{
				case AccountLifecycleEventEnum.DEACTIVATED:
					// This is the event notifying that the Add-On has been uninstalled from a tenant account.
					// No further details are available for that event.
					_logger.LogInformation("Addon Deactivated Event Received.");
					await _accountService.RemoveAccountInfo(tenantId, CancellationToken.None).ConfigureAwait(false);
					break;
			}

			return Ok();
		}

		/// <summary>
		/// Gets the configuration settings.
		/// </summary>
		/// <returns>The updated configuration settings.</returns>
		[Authorize]
		[HttpGet("configuration")]
		public async Task<IActionResult> GetConfigurationSettings()
		{
			// All configuration settings must be returned to the caller.
			// Configurations that are secret will be returned with the value set to "*****", if they have a value.

			_logger.LogInformation("Retrieving the configuration settings.");

			var tenantId = Request.HttpContext.User.Claims.Single(c => c.Type == "X-LC-Tenant").Value;
			var configurationSettingsResult = await _accountService.GetConfigurationSettings(tenantId,true, CancellationToken.None).ConfigureAwait(false);

			var resultValue = Content(JsonSerializer.Serialize(configurationSettingsResult, JsonSettings.Default()), "application/json", Encoding.UTF8);
			resultValue.StatusCode = 200;

			return resultValue;
		}

		/// <summary>
		/// Sets or updates the configuration settings.
		/// </summary>
		/// <returns>The updated configuration settings.</returns>
		[Authorize]
		[HttpPost("configuration")]
		public async Task<IActionResult> SetConfigurationSettings()
		{
			_logger.LogInformation("Setting the configuration settings.");

			string payload;
			using (var sr = new StreamReader(Request.Body))
			{
				payload = await sr.ReadToEndAsync();
			}

			var tenantId = Request.HttpContext.User.Claims.Single(c => c.Type == "X-LC-Tenant").Value;
			var configurationValues = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigurationValueModel>>(payload);
			var configurationSettingsResult = await _accountService.SaveOrUpdateConfigurationSettings(tenantId, configurationValues, CancellationToken.None).ConfigureAwait(false);
			var resultValue = Content(JsonSerializer.Serialize(configurationSettingsResult, JsonSettings.Default()), "application/json", Encoding.UTF8);
			resultValue.StatusCode = 200;

			return resultValue;
		}

		//[Authorize]
		[HttpGet("translation-engines")]
		public async Task<IActionResult> GetTranslationEngines([FromQuery]TranslationEngineRequest request, [FromHeader(Name = "X-LC-Tenant")]string tenantId, [FromHeader(Name = "TR_ID")]string traceId)
		{
			var apiKey = await GetSettingValue(tenantId, ApiKeyId);

			if (!string.IsNullOrEmpty(apiKey))
			{
				if (!string.IsNullOrEmpty(request.SourceLanguage) && request.TargetLanguage.Any())
				{
					var engineResponse = await _languageService.GetCorrespondingEngines(apiKey, request.SourceLanguage, request.TargetLanguage);
					return Ok(engineResponse);
				}
			}
			else
			{
				return Unauthorized("DeepL API Key is not valid");
			}

			return Ok();
		}

		//[Authorize]
		[HttpPost("translate")]
		public async Task<IActionResult> Translate([FromBody]TranslationRequest request, [FromHeader(Name = "X-LC-Tenant")]string tenantId)
		{
			var apiKey = await GetSettingValue(tenantId,ApiKeyId);
			var formality = await GetSettingValue(tenantId, FormalityId);

			if (!string.IsNullOrEmpty(apiKey))
			{
				var translationResponse = await _translationService.Translate(request, apiKey,formality);
				return Ok(translationResponse);
			}
			else
			{
				return Unauthorized("DeepL API Key is not valid");
			}
		}

		private async Task<string> GetSettingValue(string tenantId,string propertyId)
		{
			var propertyValue = string.Empty;
			if (!string.IsNullOrEmpty(tenantId))
			{
				var configurationSettingsResult = await _accountService.GetConfigurationSettings(tenantId, false, CancellationToken.None).ConfigureAwait(false);
				if (configurationSettingsResult != null)
				{
					propertyValue = (string)configurationSettingsResult.Items.FirstOrDefault(c => c.Id.ToLower().Equals(propertyId.ToLower()))?.Value;
				}
			}
			
			return propertyValue;
		}
	}
}