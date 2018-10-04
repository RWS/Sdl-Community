using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtComparisonTool.Service
{
	public  class ApiUrls
	{
		private string BaseUri = @"https://lc-api.sdl.com";

		public string LoginUrl()
		{
			return $"{BaseUri}/studio/login";
		}
	}
}
