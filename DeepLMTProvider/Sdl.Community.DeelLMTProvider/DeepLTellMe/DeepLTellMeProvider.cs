using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	[TellMeProvider]
	public class DeepLTellMeProvider : ITellMeProvider
	{
		public string Name =>"DeepL tell me provider";

		public AbstractTellMeAction[] ProviderActions => new[]
		{
			new DeepLStoreAction
			{
				Keywords = new[] {"deepL", "deepl store", "deepl download"}
			}
		};
	}
}
