using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel
	{
		private ICommand _selectFoldersCommand;

		public ICommand SelectFoldersCommand => _selectFoldersCommand ??
		                                        (_selectFoldersCommand = new CommandHandler(SelectFolder, true));

		private void SelectFolder()
		{
			
		}
	}
}
