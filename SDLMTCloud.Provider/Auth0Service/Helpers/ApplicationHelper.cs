using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Auth0Service.Helpers
{
	public static class ApplicationHelper
	{
		public static void DoEvents(this Application application)
		{
			application.Dispatcher.Invoke(DispatcherPriority.Background,
												  new Action(delegate { }));
		}
	}
}
