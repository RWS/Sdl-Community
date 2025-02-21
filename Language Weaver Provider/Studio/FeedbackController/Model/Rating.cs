using System.Collections.Generic;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class Rating
	{
		public Rating(string feedbackMessage, TranslationErrors translationErrors)
		{
			if (!string.IsNullOrEmpty(feedbackMessage))
			{
				Comments.Add(feedbackMessage);
			}

			var problemsReported = translationErrors.GetProblemsReported();
			Comments.AddRange(problemsReported);
		}

		[JsonProperty("comments")]
		public List<string> Comments { get; set; } = new();
	}
}