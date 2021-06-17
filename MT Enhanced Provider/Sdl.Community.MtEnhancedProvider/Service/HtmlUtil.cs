using System;
using System.Reflection;
using System.Web;
using NLog;

namespace Sdl.Community.MtEnhancedProvider.Service
{
	public class HtmlUtil
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		
		public string HtmlDecode(string input)
		{
			try
			{
				return HttpUtility.HtmlDecode(input);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n { ex.StackTrace}");
			}

			return input;
		}
	}
}
