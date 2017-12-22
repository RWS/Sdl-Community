using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service
{
	public static class Constants
	{
		public static readonly string BaseUri = @"https://lc-api.sdl.com";
		public static readonly string Login = string.Concat(BaseUri, @"/studio/login");
	}
}
