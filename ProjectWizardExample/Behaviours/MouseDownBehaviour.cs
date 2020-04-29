using System.Windows;
using System.Windows.Input;

namespace ProjectWizardExample.Behaviours
{
	public static class MouseDownBehaviour
	{
		public static readonly DependencyProperty MouseDownCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(
			UIElement.MouseDownEvent, "MouseDown", typeof(MouseDownBehaviour));

		public static void SetMouseDown(DependencyObject o, ICommand value)
		{
			o.SetValue(MouseDownCommand, value);
		}

		public static ICommand GetMouseDown(DependencyObject o)
		{
			return o.GetValue(MouseDownCommand) as ICommand;
		}
	}
}
