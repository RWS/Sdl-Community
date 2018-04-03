using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.GDPR
{
	class MyCustomBatchTaskSettingsControl : UserControl, ISettingsAware<MyCustomBatchTaskSettings>
	{
		public MyCustomBatchTaskSettings Settings
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
