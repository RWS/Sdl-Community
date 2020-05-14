using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Service;
using Xunit;

namespace Sdl.Community.MTCloud.Provider.UnitTests
{
	public class ShortcutServiceTests
	{
		private readonly ShortcutService _shortcutService;

		public ShortcutServiceTests()
		{
			_shortcutService = new ShortcutService();
		}	
		[Fact]
		public void GetCustomShortcuts()
		{
			//var test = _shortcutService.GetCustomRateItShortcuts();
		}
	}
}
