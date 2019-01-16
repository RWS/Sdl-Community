using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BringBackTheButton.TellMe
{
	[TellMeProvider]
	class BringBackTheButtonTellMeProvider : ITellMeProvider
	{
		public string Name => "Bring back the button tell me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]

		{

			new BringBackTheButtonAction

			{

				Keywords = new[]

				{

					"don't push it", "bring back"

				}

			}
		};
	}
}
