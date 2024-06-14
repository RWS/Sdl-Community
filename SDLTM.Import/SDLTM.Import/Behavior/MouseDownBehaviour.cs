using Sdl.Desktop.Platform.Controls.Behaviours;
using System.Windows.Input;
using System.Windows;

namespace SDLTM.Import.Behavior
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
