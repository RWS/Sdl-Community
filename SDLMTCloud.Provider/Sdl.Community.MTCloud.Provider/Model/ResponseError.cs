using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ResponseError
	{
		public long AccountId { get; set; }
		public List<ErrorDetails> Errors { get; set; }
	}
}