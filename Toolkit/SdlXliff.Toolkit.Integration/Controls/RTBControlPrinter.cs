using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SdlXliff.Toolkit.Integration.Controls
{
    class RTBControlPrinter
    {
        private const double anInch = 14.4;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            // First character of range (0 for start of doc)
            public int cpMin;
            // Last character of range (-1 for end of doc)
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FORMATRANGE
        {
            // Actual DC to draw on
            public IntPtr hdc;
            // Target DC for determining text formatting
            public IntPtr hdcTarget;
            // Region of the DC to draw to (in twips)
            public RECT rc;
            // Region of the whole DC (page size) (in twips)
            public RECT rcPage;
            // Range of text to draw (see earlier declaration)
            public CHARRANGE chrg;
        }

        private const int WM_USER = 0x0400;
        private const int EM_FORMATRANGE = WM_USER + 57;

        [DllImport("USER32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        /// <summary>
        /// Render the contents of the RichTextBox for printing
        /// Return the last character printed + 1 (printing start from this point for next page)
        /// </summary>
        /// <param name="richTextBoxHandle"></param>
        /// <param name="charFrom"></param>
        /// <param name="charTo"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int Print(IntPtr richTextBoxHandle, int charFrom, int charTo, PrintPageEventArgs e)
        {
            //Calculate the area to render and print
            RECT rectToPrint;
            rectToPrint.Top = (int)(e.MarginBounds.Top * anInch);
            rectToPrint.Bottom = (int)(e.MarginBounds.Bottom * anInch);
            rectToPrint.Left = (int)(e.MarginBounds.Left * anInch);
            rectToPrint.Right = (int)(e.MarginBounds.Right * anInch);

            //Calculate the size of the page
            RECT rectPage;
            rectPage.Top = (int)(e.PageBounds.Top * anInch);
            rectPage.Bottom = (int)(e.PageBounds.Bottom * anInch);
            rectPage.Left = (int)(e.PageBounds.Left * anInch);
            rectPage.Right = (int)(e.PageBounds.Right * anInch);

            IntPtr hdc = e.Graphics.GetHdc();

            FORMATRANGE fmtRange;
            fmtRange.chrg.cpMax = charTo;				//Indicate character from to character to 
            fmtRange.chrg.cpMin = charFrom;
            fmtRange.hdc = hdc;                    //Use the same DC for measuring and rendering
            fmtRange.hdcTarget = hdc;              //Point at printer hDC
            fmtRange.rc = rectToPrint;             //Indicate the area on page to print
            fmtRange.rcPage = rectPage;            //Indicate size of page

            IntPtr res = IntPtr.Zero;

            IntPtr wparam = IntPtr.Zero;
            wparam = new IntPtr(1);

            //Get the pointer to the FORMATRANGE structure in memory
            IntPtr lparam = IntPtr.Zero;
            lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fmtRange));
            Marshal.StructureToPtr(fmtRange, lparam, false);

            //Send the rendered data for printing 
            res = SendMessage(richTextBoxHandle, EM_FORMATRANGE, wparam, lparam);

            //Free the block of memory allocated
            Marshal.FreeCoTaskMem(lparam);

            //Release the device context handle obtained by a previous call
            e.Graphics.ReleaseHdc(hdc);

            // Release and cached info
            SendMessage(richTextBoxHandle, EM_FORMATRANGE, (IntPtr)0, (IntPtr)0);

            //Return last + 1 character printer
            return res.ToInt32();
        }

        public static Image Print(RichTextBox ctl, int width, int height)
        {
            Image img = new Bitmap(width, height);
            float scale;

            using (Graphics g = Graphics.FromImage(img))
            {
                // --- Begin code addition D_Kondrad

                // HorizontalResolution is measured in pix/inch         
                scale = (float)(width * 100) / img.HorizontalResolution;
                width = (int)scale;

                // VerticalResolution is measured in pix/inch
                scale = (float)(height * 100) / img.VerticalResolution;
                height = (int)scale;

                // --- End code addition D_Kondrad

                Rectangle marginBounds = new Rectangle(0, 0, width, height);
                Rectangle pageBounds = new Rectangle(0, 0, width, height);
                PrintPageEventArgs args = new PrintPageEventArgs(g, marginBounds, pageBounds, null);

                Print(ctl.Handle, 0, ctl.Text.Length, args);
            }

            return img;
        }
    }
}
