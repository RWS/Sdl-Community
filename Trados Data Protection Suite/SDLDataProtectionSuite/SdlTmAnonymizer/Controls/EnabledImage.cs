using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls
{
	public class EnabledImage : Image
	{
		static EnabledImage()
		{			
			IsEnabledProperty.OverrideMetadata(typeof(EnabledImage), 
				new FrameworkPropertyMetadata(true, OnAutoGreyScaleImageIsEnabledPropertyChanged));
		}

		private static void OnAutoGreyScaleImageIsEnabledPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
		{			
			if (source is EnabledImage autoGreyScaleImg)
			{			
				if (Convert.ToBoolean(args.NewValue))
				{
					autoGreyScaleImg.Source = ((FormatConvertedBitmap)autoGreyScaleImg.Source).Source;
					autoGreyScaleImg.OpacityMask = null;					
				}
				else
				{
					var bitmapImage = new BitmapImage(new Uri(autoGreyScaleImg.Source.ToString()));
					autoGreyScaleImg.Source = new FormatConvertedBitmap(bitmapImage, PixelFormats.Gray32Float, null, 0);
					autoGreyScaleImg.OpacityMask = new ImageBrush(bitmapImage);
				}
			}
		}
	}
}
