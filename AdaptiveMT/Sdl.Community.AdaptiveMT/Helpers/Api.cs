using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sdl.Community.AdaptiveMT.Service.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.AdaptiveMT.Helpers
{
	public static class Api
	{
		public static FeedbackRequest CreateFeedbackRequest(string translatedText,ISegmentPair segmentPair, EngineMappingDetails engine)
		{
			var feedbackRequest = new FeedbackRequest
			{
				LanguagePair = new LanguagePair
				{
					Source = engine.SourceLang,
					Target = engine.TargetLang
				},
				Source = segmentPair.Source.ToString(),
				OriginalOutput = HttpUtility.HtmlDecode(translatedText),
				PostEdited = segmentPair.Target.ToString(),
				Definition = new Definition
				{
					Resources = new List<Resource>()
				}
			};
			var resource = new Resource
			{
				Type = "MT",
				ResourceId = engine.Id
			};
			feedbackRequest.Definition.Resources.Add(resource);

			return feedbackRequest;
		}

		public static TranslateRequest CreateTranslateRequest(ISegmentPair segmentPair, EngineMappingDetails engine)
		{
			var translateRequest = new TranslateRequest
			{
				Content = new Content
				{
					InputFormat = "plain",
					Text = new[] { HttpUtility.UrlEncode(segmentPair.Source.ToString()) }
				},
				Definition = new Definition
				{
					Resources = new List<Resource>()
				},
				LanguagePair = new LanguagePair
				{
					Source = engine.SourceLang,
					Target = engine.TargetLang
				}
			};
			var resource = new Resource
			{
				Type = "MT",
				ResourceId = engine.Id
			};
			translateRequest.Definition.Resources.Add(resource);
			return translateRequest;
		}
		
	}
}
