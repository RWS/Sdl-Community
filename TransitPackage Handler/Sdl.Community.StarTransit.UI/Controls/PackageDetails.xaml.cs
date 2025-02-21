using System.Windows.Controls;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI.Controls
{
	/// <summary>
	/// Interaction logic for PackageDetails.xaml
	/// </summary>
	public partial class PackageDetails : UserControl
	{

		public PackageDetails(PackageDetailsViewModel packageDetailsViewModel)
		{
			DataContext = packageDetailsViewModel;
			InitializeComponent();
			dueDatePicker.BlackoutDates.AddDatesInPast();
		}

		public bool FieldsAreCompleted()
		{
			var completed = true;

			if (string.IsNullOrEmpty(txtLocation.Text))
			{
				completed = false;
			}
			if (comboBox.SelectedItem == null)
			{
				completed = false;
			}
			return completed;
		}
	}
}