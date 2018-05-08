using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class DropBehavior
	{
		#region The dependecy Property

		/// <summary>
		/// The Dependency property. To allow for Binding, a dependency
		/// property must be used.
		/// </summary>
		private static readonly DependencyProperty PreviewDropCommandProperty =
			DependencyProperty.RegisterAttached
			(
				"PreviewDropCommand",
				typeof(ICommand),
				typeof(DropBehavior),
				new PropertyMetadata(PreviewDropCommandPropertyChangedCallBack)
			);

		#endregion

		#region The getter and setter

		/// <summary>
		/// The setter. This sets the value of the PreviewDropCommandProperty
		/// Dependency Property. It is expected that you use this only in XAML
		///
		/// This appears in XAML with the "Set" stripped off.
		/// XAML usage:
		///
		/// <Grid mvvm:DropBehavior.PreviewDropCommand="{Binding DropCommand}" />
		///
		/// </summary>
		/// <param name="inUIElement">A UIElement object. In XAML this is automatically passed
		/// in, so you don't have to enter anything in XAML.</param>
		/// <param name="inCommand">An object that implements ICommand.</param>
		public static void SetPreviewDropCommand(this UIElement inUIElement, ICommand inCommand)
		{
			inUIElement.SetValue(PreviewDropCommandProperty, inCommand);
		}

		/// <summary>
		/// Gets the PreviewDropCommand assigned to the PreviewDropCommandProperty
		/// DependencyProperty. As this is only needed by this class, it is private.
		/// </summary>
		/// <param name="inUIElement">A UIElement object.</param>
		/// <returns>An object that implements ICommand.</returns>
		private static ICommand GetPreviewDropCommand(UIElement inUIElement)
		{
			return (ICommand)inUIElement.GetValue(PreviewDropCommandProperty);
		}

		#endregion


		/// <summary>
		/// The OnCommandChanged method. This event handles the initial binding and future
		/// binding changes to the bound ICommand
		/// </summary>
		/// <param name="inDependencyObject">A DependencyObject</param>
		/// <param name="inEventArgs">A DependencyPropertyChangedEventArgs object.</param>
		private static void PreviewDropCommandPropertyChangedCallBack(
			DependencyObject inDependencyObject, DependencyPropertyChangedEventArgs inEventArgs)
		{
			UIElement uiElement = inDependencyObject as UIElement;
			if (null == uiElement) return;

			uiElement.Drop += (sender, args) =>
			{
				GetPreviewDropCommand(uiElement).Execute(args.Data);
				args.Handled = true;
			};
		}
	}
}
