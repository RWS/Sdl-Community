using NLog;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Size = System.Drawing.Size;

namespace Sdl.LanguagePlatform.MTConnectors.Google.Utils
{
    public static class IconUtil
    {
        private static Hashtable _fileIconCache = new Hashtable();

        private static readonly object _lockObject = new object();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x00; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x01; // 'Small icon
            public const uint SHGFI_USEFILEATTRIBUTES = 0x10;
            public const uint FILE_ATTRIBUTE_TEMPORARY = 0X100;
            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }

        /// <summary>
        /// Get the associated Icon for a file or application, this method always returns
        /// an icon. If the strPath is invalid or there is no idonc the default
        ///	icon is returned
        /// </summary>
        /// <param name="strPath">full path to the file or directory</param>
        /// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
        /// <param name="bOpen">if true, and strPath is a folder, returns the 'open' icon rather than the 'closed'</param>
        /// <returns></returns>
        public static Icon GetIcon(string strPath, bool bSmall, bool bOpen)
        {
            FileInfo fileInfo = new FileInfo(strPath);
            string ext = fileInfo.Extension;
            string cacheExt = fileInfo.Extension + bSmall.ToString();

            lock (_fileIconCache.SyncRoot)
            {
                Icon icon = (Icon)_fileIconCache[cacheExt];

                if (icon != null)
                {
                    return icon;
                }

                try
                {
                    SHFILEINFO shinfo = new SHFILEINFO();

                    uint flags = 0;
                    if (bSmall)
                        flags = Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES;
                    else
                        flags = Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_USEFILEATTRIBUTES;

                    //Use this to get the small Icon
                    IntPtr hImgSmall = Win32.SHGetFileInfo(strPath, Win32.FILE_ATTRIBUTE_TEMPORARY, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

                    //The icon is returned in the hIcon member of the shinfo struct
                    _fileIconCache[cacheExt] = Icon.FromHandle(shinfo.hIcon);

                    return icon;
                }
                catch (Exception ex)
                {
                    Logger _logger = LogManager.GetCurrentClassLogger();
                    _logger.Info(typeof(IconUtil).FullName);
                    _logger.Warn("Could not load icon for {0} in small format {1}, open={2}. Exception: {3}", strPath, bSmall, bOpen, ex);
                }

                return null;
            }
        }


        /// <summary>
        /// Returns an icon for a given file - indicated by the name parameter.
        /// </summary>
        /// <param name="name">Pathname for file.</param>
        /// <param name="bSmall">Large or small</param>
        /// <returns>System.Drawing.Icon</returns>
        public static Icon GetAssemblyFileIcon(string name, bool bSmall)
        {
            var shfi = new SHFILEINFO();
            uint flags = Win32.SHGFI_ICON | Win32.SHGFI_USEFILEATTRIBUTES;


            /* Check the size specified for return. */
            if (bSmall == true)
            {
                flags += Win32.SHGFI_SMALLICON;
            }
            else
            {
                flags += Win32.SHGFI_LARGEICON;
            }

            Win32.SHGetFileInfo(name,
                Win32.FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                flags);

            // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
            var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            return icon;
        }

        /// <summary>
        /// Creates a gray-scaled version of the provided bitmap using a color matrix.
        /// </summary>
        /// <param name="originalImage">The bitmap to apply gray-scale filtering to.</param>
        /// <returns>A new bitmap which is a gray-scaled version of the original.</returns>
        public static Bitmap ApplyBitmapGrayscale(Bitmap originalImage)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(originalImage.Width, originalImage.Height);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image using the grayscale color matrix
                g.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                   0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, attributes);
            }

            return newBitmap;
        }


        public static Bitmap IconToBitmap(Icon icon)
        {
            return IconToBitmap(icon, icon.Width, icon.Height);
        }

        public static Bitmap IconToBitmap(Icon icon, int width, int height)
        {
            lock (_lockObject)
            {
                Icon sizedIcon = null;
                var disposeIcon = false;

                try
                {
                    if (icon.Width == width && icon.Height == height)
                    {
                        sizedIcon = icon;
                    }
                    else
                    {
                        sizedIcon = new Icon(icon, width, height);
                        disposeIcon = true;
                    }

                    return sizedIcon.ToBitmap();
                }
                finally
                {
                    if (disposeIcon)
                    {
                        sizedIcon.Dispose();
                    }
                }
            }
        }

        private static byte[] pngiconheader =
                     new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static Icon BitmapToIcon(Image image)
        {
            return BitmapToIcon(image, image.Width, image.Height);
        }

        public static Icon BitmapToIcon(Image image, int width, int height)
        {
            var imageSize = new Size(width, height);
            using (Bitmap bmp = new Bitmap(image, imageSize))
            {
                byte[] png;
                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    bmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    fs.Position = 0;
                    png = fs.ToArray();
                }

                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    if (height >= 256) height = 0;
                    pngiconheader[6] = (byte)height;
                    pngiconheader[7] = (byte)height;
                    pngiconheader[14] = (byte)(png.Length & 255);
                    pngiconheader[15] = (byte)(png.Length / 256);
                    pngiconheader[18] = (byte)(pngiconheader.Length);

                    fs.Write(pngiconheader, 0, pngiconheader.Length);
                    fs.Write(png, 0, png.Length);
                    fs.Position = 0;
                    return new Icon(fs);
                }
            }
        }
    }
}
