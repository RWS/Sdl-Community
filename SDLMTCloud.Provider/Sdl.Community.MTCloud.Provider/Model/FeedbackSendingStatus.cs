using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FeedbackSendingStatus
	{
		public string Message { get; private set; }

		public Status Status { get; set; }

		public async Task ChangeStatus(HttpResponseMessage responseMessage)
		{
			var responseReason = await MtCloudApplicationInitializer.Client.GetResponseAsString(responseMessage);
			var jObj = JToken.Parse(responseReason);
			if (responseMessage.IsSuccessStatusCode)
			{
				Status = Status.Sent;
				var response = $"{jObj.ToString(Formatting.Indented)}";
				Message = string.Format(PluginResources.ResponseFromServer, PluginResources.FeedbackSentSuccessfully, response);
			}
			else
			{
				Status = Status.RequestFailed;
				var response = jObj["errors"].ToString(Formatting.Indented);
				var extraInfo = string.Empty;
				extraInfo = Regex.Match(response, "translation.targetMTText").Success
					? PluginResources.OriginalMtCloudTranslationMissing
					: extraInfo;

				var title = $"{PluginResources.FeedbackNotSent_TooltipMessage}{extraInfo}.";
				Message = string.Format(PluginResources.ResponseFromServer, title, response);
			}
		}
	}
}