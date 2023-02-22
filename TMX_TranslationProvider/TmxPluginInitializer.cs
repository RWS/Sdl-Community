using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi;
using TMX_Lib.Utils;

namespace TMX_TranslationProvider
{

	[ApplicationInitializer]
	public class TmxPluginInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			LogUtil.Setup();
		}
	}

}
