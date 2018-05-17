using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class AcceptWindowViewModel:ViewModelBase
	{
		private string _description;
		private bool _accepted;
		private ICommand _okCommand;

		public AcceptWindowViewModel()
		{
			Description = Constants.AcceptDescription();
		}
		public ICommand OkCommand => _okCommand ??
		                                        (_okCommand = new CommandHandler(Ok, true));

		private void Ok()
		{
			
		}
		public string Description
		{
			get => _description;

			set
			{
				if (Equals(value, _description))
				{
					return;
				}
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}
		public bool Accepted
		{
			get => _accepted;

			set
			{
				if (Equals(value, _accepted))
				{
					return;
				}
				_accepted = value;
				OnPropertyChanged(nameof(Accepted));
			}
		}
	}
}
