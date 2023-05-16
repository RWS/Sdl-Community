using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.Platform.Controls.Behaviours;
using System.Windows.Input;
using System.Windows;

namespace Sdl.Community.StarTransit.Behaviours
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
