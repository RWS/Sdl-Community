using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.FeedbackService.Model;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface IFeedbackService
	{
		FeedbackSettings Settings { get; }
		bool IsActiveModelQeEnabled { get; }

		Task<HttpResponseMessage> SendFeedback(FeedbackInfo feedbackInfo);
	}
}
