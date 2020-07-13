using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.XLIFF.Manager.Behaviours
{
	public class SelectedItemBehaviour
	{
		public static readonly DependencyProperty SelectedItemChangedCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(
			TreeView.SelectedItemChangedEvent, "SelectedItemChangedEvent", typeof(SelectedItemBehaviour));

		public static void SetSelectedItemChangedEvent(DependencyObject o, ICommand value)
		{
			o.SetValue(SelectedItemChangedCommand, value);
		}

		public static ICommand GetSelectedItemChangedEvent(DependencyObject o)
		{
			return o.GetValue(SelectedItemChangedCommand) as ICommand;
		}
	}
}
