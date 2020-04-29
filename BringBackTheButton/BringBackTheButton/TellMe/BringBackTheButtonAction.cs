using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.BringBackTheButton.TellMe
{
	class BringBackTheButtonAction : AbstractTellMeAction
	{
		public BringBackTheButtonAction()
		{
			Name = "Don't push it!";
		}
		public override string Category => "Bring back the button";
		public override bool IsAvailable => true;

		public override void Execute()
		{
			var action = new BringBackTheButtonViewPartAction() as IAction;
			action.Execute();
		}
	}
}
