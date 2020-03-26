using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.Community.SDLBatchAnonymize.Model;

namespace Sdl.Community.SDLBatchAnonymize.ViewModel
{
	public class BatchAnonymizerSettingsViewModel:ModelBase
	{
		private IUserNameService _userNameService;

		public BatchAnonymizerSettingsViewModel(IUserNameService userNameService)
		{
			_userNameService = userNameService;
		}
	}
}
