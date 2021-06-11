using System;
using Sdl.Community.StarTransit.ViewModel;

namespace Sdl.Community.StarTransit.View
{
	/// <summary>
	/// Interaction logic for ReturnPackageWindow.xaml
	/// </summary>
	public partial class ReturnPackageWindow : IDisposable
	{
		public ReturnPackageWindow()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
			if (DataContext is ReturnPackageWindowViewModel returnViewModel)
			{
				returnViewModel.Dispose();
			}
		}
	}
}
