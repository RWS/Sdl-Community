using System;
using System.Diagnostics;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.ViewModel;

namespace Sdl.Community.StarTransit.View
{
	/// <summary>
	/// Interaction logic for ReturnPackageWindow.xaml
	/// </summary>
	public partial class ReturnPackageWindow : IDisposable
	{
		private readonly IDisposable _openPackageLocationEvent;

		public ReturnPackageWindow(IEventAggregatorService eventAggregatorService)
		{
			InitializeComponent();

			_openPackageLocationEvent =
				eventAggregatorService?.Subscribe<OpenReturnPackageLocation>(OpenPackageLocation);
		}

		private void OpenPackageLocation(OpenReturnPackageLocation returnPackageEvent)
		{
			if (!string.IsNullOrEmpty(returnPackageEvent.RetuntPackageLocation))
			{
				Process.Start(returnPackageEvent.RetuntPackageLocation);
			}
			Close();
		}

		public void Dispose()
		{
			if (DataContext is ReturnPackageWindowViewModel returnViewModel)
			{
				returnViewModel.Dispose();
			}
			_openPackageLocationEvent.Dispose();
		}
	}
}
