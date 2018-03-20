using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Model
{
	public class UserRequest
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string SelectedAccountId { get; set; }
	}
}
