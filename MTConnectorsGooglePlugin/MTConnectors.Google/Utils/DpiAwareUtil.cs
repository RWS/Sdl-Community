using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.MTConnectors.Google.Utils
{
    public class DpiAwareUtil
    {

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        enum DeviceCap
        {
            HORZRES = 8,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            DESKTOPHORZRES = 118
        }

        /// <summary>
        /// Recover the DPI display settings
        /// 
        /// The desktop scaling factor is a percentage that indicates how Windows 
        /// scales the UI of a desktop application when its DPI awareness level allows 
        /// scaling. In general, the desktop scaling factor is based on the DPI:
        /// 
        /// * 96 DPI = 100% scaling
        /// * 120 DPI = 125% scaling
        /// * 144 DPI = 150% scaling
        /// * 192 DPI = 200% scaling
        /// 
        /// </summary>
        /// <returns></returns>
        public static float GetVerticalScalingFactor()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            var userDpi = (float)g.DpiY;

            var desktop = g.GetHdc();
            var logicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            var physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            var screenScalingFactor = (float)physicalScreenHeight / (float)logicalScreenHeight;

            if (userDpi > 96 && screenScalingFactor <= 1)
                screenScalingFactor = userDpi / 96;

            return screenScalingFactor;
        }

        public static float GetHorizontalScalingFactor()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            var userDpi = (float)g.DpiX;

            var desktop = g.GetHdc();
            var logicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.HORZRES);
            var physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPHORZRES);

            var screenScalingFactor = (float)physicalScreenWidth / (float)logicalScreenWidth;

            if (userDpi > 96 && screenScalingFactor <= 1)
                screenScalingFactor = userDpi / 96;

            return screenScalingFactor;
        }

        public static int AdjustHeightForCurrentDpi(int height)
        {
            return (int)Math.Round(height * GetVerticalScalingFactor());
        }

        public static int AdjustWidthForCurrentDpi(int width)
        {
            return (int)Math.Round(width * GetHorizontalScalingFactor());
        }

        /// <summary>
        /// When conecting via Remote Desktop to a Windows 10 system from a machine
        /// that has high DPI the default font size for windows forms controls is
        /// - due to some wierd Microsoft bug - very very small.
        /// 
        /// WARNING: The default font is read only so the fix is a hack that might break 
        /// when upgrading to future versions of .Net framework.
        /// 
        /// What we are doing is we overwrite Control.defaultFont property using relection.
        /// The advantage of this hack is that it is the only solution that automatically
        /// fixes all existing and future forms and user controls with a single function
        /// call when the application is launched.
        /// </summary>
        /// <returns></returns>
        public static void FixDefaultFontSize()
        {
            Font font = SystemFonts.DefaultFont;

            if (font.Size < 8)
            {
                Type settingsType = typeof(Control);
                var defaultFontField = settingsType.GetField("defaultFont", BindingFlags.Static | BindingFlags.NonPublic);
                font = new Font(font.Name, 8.25f, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
                defaultFontField.SetValue(null, font);
            }

        }

        /// <summary>
        /// Returns the System.Drawing.Size based on the current DPI
        ///
        /// Example: (baseSize = 16)
        /// 
        ///  96 | DPI(100%) | 16*1.00 | 16x16
        /// 120 | DPI(125%) | 16*1.25 | 20x20
        /// 144 | DPI(150%) | 16*1.50 | 24x24
        /// 192 | DPI(200%) | 16*2.00 | 32x32
        /// </summary>
        /// <param name="baseSize">Optional: base size</param>
        /// <returns>DPI aware System.Drawing.Size</returns>
        public static Size GetIconSizeForCurrentDpi(int baseSize = 16)
        {
            int[] sizes = { 16, 20, 24, 32, 48, 64, 128, 256 };
            int neededSize = (int)Math.Round(baseSize * GetHorizontalScalingFactor());
            int sizeIndex = sizes.Length - 1;
            while ((sizeIndex > 0) && (sizes[sizeIndex] > neededSize))
                sizeIndex--;
            return new Size(sizes[sizeIndex], sizes[sizeIndex]);
        }

        public static int AdjustHeightWithIconScalingFactor(int height, int iconBaseSize = 24)
        {
            return (int)Math.Round(1f * height * GetIconSizeForCurrentDpi(iconBaseSize).Width / iconBaseSize);
        }

        public static int AdjustHeightWithIconScalingFactor(int height, FlagImageSize iconBaseSize)
        {
            return AdjustHeightWithIconScalingFactor(height, (int)iconBaseSize);
        }

        /// <summary>
        /// Resturns a Size that is generated based on input base size (at 96 DPI - 100%) upon which
        /// the scaling factor coresponding with current DPI settings is applied
        /// </summary>
        public static System.Drawing.Size GetSizeForCurrentDpi(int width, int height)
        {
            return new System.Drawing.Size(AdjustWidthForCurrentDpi(width), AdjustHeightForCurrentDpi(height));
        }

        /// <summary>
        /// Resturns a Size that is generated based on input base size (at 96 DPI - 100%) upon which
        /// the scaling factor coresponding with current DPI settings is applied
        /// </summary>
        public static System.Drawing.Size GetSizeForCurrentDpi(System.Drawing.Size size)
        {
            return GetSizeForCurrentDpi(size.Width, size.Height);
        }

        /// <summary>
        /// Resturns a Padding that is generated based on input base Padding (at 96 DPI - 100%) upon which
        /// the scaling factor coresponding with current DPI settings is applied
        /// </summary>
        public static Padding GetPaddingForCurrentDpi(Padding padding)
        {
            return new Padding(
                AdjustWidthForCurrentDpi(padding.Left),
                AdjustHeightForCurrentDpi(padding.Top),
                AdjustWidthForCurrentDpi(padding.Right),
                AdjustHeightForCurrentDpi(padding.Bottom));
        }


        /// <summary>
        /// Image must be available in resources in multiple sizes and must follow naming
        /// convention by adding the following suffixes to base image name:
        ///  96 | DPI(100%) | ""
        /// 120 | DPI(125%) | "_1_25x"
        /// 144 | DPI(150%) | "_1_5x"
        /// 168 | DPI(175%) | "_1_75x"
        /// 192 | DPI(200%) | "_2x"
        /// 216 | DPI(225%) | "_2_25x"
        /// 240 | DPI(250%) | "_2_5x"
        /// </summary>
        /// <param name="resourceManager">Resource Manager for the image resource</param>
        /// <param name="culture">Culture of the image resource</param>
        /// <param name="imageName">Base image name</param>
        /// <returns>Returns the image that fits best current DPI settings</returns>
        public static System.Drawing.Image GetImageForCurrentDpi(ResourceManager resourceManager, CultureInfo culture, string imageName)
        {
            string[] imageNameSuffixes = { "", "_1_25x", "_1_5x", "_1_75x", "_2x", "_2_25x", "_2_5x" };
            float[] scaleFactors = { 1f, 1.25f, 1.5f, 1.75f, 2f, 2.25f, 2.5f };
            float scaleFactor = GetHorizontalScalingFactor();
            int index = scaleFactors.Length - 1;

            while (index > 0)
            {
                if (scaleFactors[index] <= scaleFactor) break;
                index--;
            }

            object obj = resourceManager.GetObject(imageName + imageNameSuffixes[index], culture);

            //if image is not avilable for current DPI scale factor use default image size
            if (null == obj)
                obj = resourceManager.GetObject(imageName, culture);

            return ((System.Drawing.Image)(obj));
        }

        /// <summary>
        /// Resturns a Point that is generated based on input base X and Y coordinates (at 96 DPI - 100%) upon which
        /// the scaling factor coresponding with current DPI settings is applied
        /// </summary>
        public static Point GetPointAdjustedForCurrentDpi(int x, int y)
        {
            return new Point(AdjustWidthForCurrentDpi(x), AdjustHeightForCurrentDpi(y));
        }

    }
}
