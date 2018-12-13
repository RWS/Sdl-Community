using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.TellMe
{
	[TellMeProvider]
	public class AdvancedDisplayFilterTellMeProvider : ITellMeProvider
	{
		public string Name => "Advanced Display Filter Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new StoreAction
			{
				Keywords = new[]
				{
					"community advanced display filter", "community advanced display filter store", "community advanced display filter download"
				}
			},
			new HelpAction
			{
				Keywords = new[]
				{
					"community advanced display filter", "community advanced display filter help", "community advanced display filter guide"
				}
			},
			new CommunityForumAction
			{
				Keywords = new[]
				{
					"community advanced display filter", "community advanced display filter forum", "community advanced display filter report"
				}
			},
			new OpenProjectFilesAction
			{
				Keywords = new[]
				{
					"community advanced display filter", "community advanced display filter open files"
				}
			},
			new OpenAdvancedDisplayFilter
			{
				Keywords = new[]
				{
					"community advanced display filter", "community advanced display filter community"
				}
			}
		};
	}
}
