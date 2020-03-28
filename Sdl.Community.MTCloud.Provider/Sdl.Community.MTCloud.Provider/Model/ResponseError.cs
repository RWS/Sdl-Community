using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ResponseError
	{
		public List<ErrorDetails> Errors { get; set; }
		public long AccountId { get; set; }
	}
}
