using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Controllers
{
	[Route("api/deepl")]
    [ApiController]
    public class DeeplController : ControllerBase
    {
		private readonly IAccountService _accountService;
		private readonly ITranslationService _translationService;
		private readonly ILanguageService _languageService;
		private const string ApiKeyId = "apikey";
		private const string FormalityId = "translationFormality";

		public DeeplController(IAccountService accountService,ILanguageService languageService, ITranslationService translationService)
		{
			_accountService = accountService;
			_languageService = languageService;
			_translationService = translationService;
		}

		[Authorize]
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

		[Authorize]
		[HttpPost("translate")]
		public async Task<IActionResult> Translate([FromBody]TranslationRequest request, [FromHeader(Name = "X-LC-Tenant")]string tenantId)
		{
			var apiKey = await GetSettingValue(tenantId, ApiKeyId);
			var formality = await GetSettingValue(tenantId, FormalityId);

			if (!string.IsNullOrEmpty(apiKey))
			{
				var translationResponse = await _translationService.Translate(request, apiKey, formality);
				return Ok(translationResponse);
			}
			else
			{
				return Unauthorized("DeepL API Key is not valid");
			}
		}

		private async Task<string> GetSettingValue(string tenantId, string propertyId)
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