using System.Collections.Generic;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class ResponseError
	{
		public List<ErrorDetails> Errors { get; set; }
		public int AccountId { get; set; }
	}
}
