using System.Windows;
using System.Windows.Controls;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for Translations.xaml
	/// </summary>
	public partial class ContentFilteringRulesView : UserControl
	{
		public ContentFilteringRulesView()
		{
			InitializeComponent();
		}

		private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
		{
			if (!(sender is DataGridRow row))
			{
				return;
			}

			if (!row.IsNewItem)
			{
				if ((DataContext is ContentFilteringRulesViewModel dataContext))
				{
					dataContext.CancelRuleCommand.Execute(null);
				}				
			}
		}
	}
}
