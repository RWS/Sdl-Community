namespace Sdl.Community.MTCloud.Provider.View
{
	/// <summary>
	/// Interaction logic for BeGlobalWindow.xaml
	/// </summary>
	public partial class OptionsWindow
	{
		public OptionsWindow()
		{
			InitializeComponent();
		}

		private void Reset_Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Reset_ComboBox.IsDropDownOpen = true;
		}
	}
}