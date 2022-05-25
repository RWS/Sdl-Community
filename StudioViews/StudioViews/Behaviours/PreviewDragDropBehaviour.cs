﻿using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.StudioViews.Behaviours
{
	public static class PreviewDragDropBehaviour
	{
		public static readonly DependencyProperty PreviewDragDropCommand = EventBehaviourFactory.CreateCommandExecutionEventBehaviour(
			UIElement.PreviewDropEvent, "PreviewDragDrop", typeof(PreviewDragDropBehaviour));

		public static void SetPreviewDragDrop(DependencyObject o, ICommand value)
		{
			o.SetValue(PreviewDragDropCommand, value);
		}

		public static ICommand GetPreviewDragDrop(DependencyObject o)
		{
			return o.GetValue(PreviewDragDropCommand) as ICommand;
		}
	}
}
