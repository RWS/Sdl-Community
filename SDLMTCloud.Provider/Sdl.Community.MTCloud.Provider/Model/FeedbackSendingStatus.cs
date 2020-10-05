using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FeedbackSendingStatus
	{
		public string Message { get; private set; }

		public Status Status { get; set; }

		public void ChangeStatus(HttpResponseMessage responseMessage)
		{
			if (responseMessage.IsSuccessStatusCode)
			{
				Status = Status.Sent;
				Message = PluginResources.FeedbackSentSuccessfully;
			}
			else
			{
				Status = Status.NotSent;
				var jObj = JObject.Parse(responseMessage.ReasonPhrase);
				jObj.TryGetValue("errors", out var jErrors);

				var reasons = jErrors?.ToObject<List<Error>>();
				var formattedReasons = reasons != null ? string.Join("\r\n", reasons.Select(r => r.Description)) : null;

				Message = $"{responseMessage.StatusCode}: {Environment.NewLine}{formattedReasons}";
			}
		}
	}
}