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
		private ICommand _removeCommand;
		private ICommand _selectTmCommand;

		public ICommand SelectFoldersCommand => _selectFoldersCommand ??
		                                        (_selectFoldersCommand = new CommandHandler(SelectFolder, true));
		public ICommand RemoveCommand => _removeCommand ??
		                                        (_removeCommand = new CommandHandler(Remove, true));
		public ICommand SelectTmCommand => _selectTmCommand ??
		                                 (_selectTmCommand = new CommandHandler(SelectTm, true));

		private void SelectTm()
		{
		}

		private void Remove()
		{
		}

		private void SelectFolder()
		{
		}
	}
}
