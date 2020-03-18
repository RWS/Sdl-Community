using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ResponseError
	{
		public List<ErrorDetails> Errors { get; set; }
		public int AccountId { get; set; }
	}
}
