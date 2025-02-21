using System.Windows;
using System.Windows.Input;
using Sdl.Desktop.Platform.Controls.Behaviours;

namespace GoogleCloudTranslationProvider.Behaviours
{
	public static class DragDropBehaviour
	{
		public static readonly DependencyProperty DragDropCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(
			UIElement.DropEvent,
			"DragDrop",
			typeof(DragDropBehaviour));

		public static void SetDragDrop(DependencyObject o, ICommand value)
		{
			o.SetValue(DragDropCommand, value);
		}

		public static ICommand GetDragDrop(DependencyObject o)
		{
			return o.GetValue(DragDropCommand) as ICommand;
		}
	}
}