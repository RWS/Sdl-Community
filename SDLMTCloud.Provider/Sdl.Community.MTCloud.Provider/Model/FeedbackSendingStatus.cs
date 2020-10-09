using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FeedbackSendingStatus
	{
		public string Message { get; private set; }

		public Status Status { get; set; }

		public void ChangeStatus(HttpResponseMessage responseMessage)
		{
			var jObj = JToken.Parse(responseMessage.ReasonPhrase);
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
				var extraInfo = "";
				extraInfo = Regex.Match(response, "translation.targetMTText").Success
					? PluginResources.OriginalMtCloudTranslationMissing
					: extraInfo;

				var title = $"{PluginResources.FeedbackNotSent_TooltipMessage}{extraInfo}.";
				Message = string.Format(PluginResources.ResponseFromServer, title, response);
			}
		}
	}
}