using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class LoginWindowViewModel:ViewModelBase
	{
		private string _url;

		public string Url
		{
			get => _url;

			set
			{
				if (Equals(value, _url))
				{
					return;
				}
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}
	}
}
