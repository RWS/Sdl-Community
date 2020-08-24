using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel: ModelBase
	{
		private ViewDetails _selectedView;

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				OnPropertyChanged(nameof(SelectedView));
			}
		}
	}
}
