﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sdl.Community.StudioViews.Behaviours
{
	public static class MouseDoubleClickBehaviour
	{
		public static readonly DependencyProperty MouseDoubleClickCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(
			Control.MouseDoubleClickEvent, "MouseDoubleClick", typeof(MouseDoubleClickBehaviour));

		public static void SetMouseDoubleClick(DependencyObject o, ICommand value)
		{
			o.SetValue(MouseDoubleClickCommand, value);
		}

		public static ICommand GetMouseDoubleClick(DependencyObject o)
		{
			return o.GetValue(MouseDoubleClickCommand) as ICommand;
		}
	}
}
