using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TMLifting.Helpers
{
	public class UserCredentials
	{
		private string _password = "N/A";
		private string _username = "N/A";
		public string Password
		{
			get => _password;
			set => _password = value;
		}
		public string  UserName
		{
			get => _username;
			set => _username = value;
		}
	}
}
