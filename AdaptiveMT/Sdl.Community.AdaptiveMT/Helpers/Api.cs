using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AdaptiveMT.Service.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.AdaptiveMT.Helpers
{
	public static class Api
	{
		public static FeedbackRequest CreateFeedbackRequest(ISegmentPair segmentPair, EngineMappingDetails engine)
		{
			var feedbackRequest = new FeedbackRequest
			{
				LanguagePair = new LanguagePair
				{
					Source = engine.SourceLang,
					Target = engine.TargetLang
				},
				Source = segmentPair.Source.ToString(),
				OriginalOutput = string.Empty,
				PostEdited = segmentPair.Target.ToString(),
				Definition = new Definition
				{
					Resources = new List<Resource>()
				}
			};
			//TO DO: loop for each resouce Id
			var resource = new Resource
			{
				Type = "MT",
				ResourceId = engine.Id
			};
			feedbackRequest.Definition.Resources.Add(resource);

			return feedbackRequest;
		}
	}
}
