using System.Windows;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public class BindingDataProxy : Freezable
	{		
		protected override Freezable CreateInstanceCore()
		{
			return new BindingDataProxy();
		}
	
		public object Data
		{
			get => GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}
		
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(object), typeof(BindingDataProxy), new UIPropertyMetadata(null));
	}
}
