// Painting Controls With Fade Animations Using the Buffered Paint API
// Bradley Smith - 2011/10/23 (updated 2014/03/12)

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GroupshareExcelAddIn.ComboTreeBoxControl
{
    internal static class Interop
    {
        private const int MM_TEXT = 1;

        [Flags]
        internal enum BP_ANIMATIONSTYLE
        {
            BPAS_NONE = 0,
            BPAS_LINEAR = 1,
            BPAS_CUBIC = 2,
            BPAS_SINE = 3
        }

        internal enum BP_BUFFERFORMAT
        {
            BPBF_COMPATIBLEBITMAP,
            BPBF_DIB,
            BPBF_TOPDOWNDIB,
            BPBF_TOPDOWNMONODIB
        }

        [DllImport("uxtheme")]
        public static extern IntPtr BeginBufferedAnimation(
            IntPtr hwnd,
            IntPtr hdcTarget,
            ref Rectangle rcTarget,
            BP_BUFFERFORMAT dwFormat,
            IntPtr pPaintParams,
            ref BP_ANIMATIONPARAMS pAnimationParams,
            out IntPtr phdcFrom,
            out IntPtr phdcTo
        );

        [DllImport("uxtheme")]
        public static extern IntPtr BufferedPaintInit();

        [DllImport("uxtheme")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BufferedPaintRenderAnimation(IntPtr hwnd, IntPtr hdcTarget);

        [DllImport("uxtheme")]
        public static extern IntPtr BufferedPaintStopAllAnimations(IntPtr hwnd);

        [DllImport("uxtheme")]
        public static extern IntPtr BufferedPaintUnInit();

        public static void DrawFocusRect(Graphics graphics, Rectangle r)
        {
            if (r.IsEmpty) return;

            IntPtr hdc = graphics.GetHdc();
            try
            {
                int iMode = SetMapMode(hdc, MM_TEXT);
                RECT rect = new RECT() { left = r.Left, top = r.Top, right = r.Right, bottom = r.Bottom };
                DrawFocusRect(new HandleRef(graphics, hdc), ref rect);
                if (iMode != MM_TEXT) SetMapMode(hdc, iMode);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        [DllImport("uxtheme")]
        public static extern IntPtr EndBufferedAnimation(IntPtr hbpAnimation, bool fUpdateTarget);

        [DllImport("user32", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool DrawFocusRect(HandleRef hDc, ref RECT lpRect);

        [DllImport("gdi32")]
        private static extern int SetMapMode(IntPtr hdc, int fnMapMode);

        [StructLayout(LayoutKind.Sequential)]
        internal struct BP_ANIMATIONPARAMS
        {
            public Int32 cbSize, dwFlags;
            public BP_ANIMATIONSTYLE style;
            public Int32 dwDuration;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}