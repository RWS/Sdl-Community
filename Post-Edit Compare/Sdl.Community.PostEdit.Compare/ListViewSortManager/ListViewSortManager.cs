using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PostEdit.Compare.ListViewSortManager
{
    #region Comparers

    /// <summary>
    /// Provides text sorting (case sensitive)
    /// </summary>
    public class ListViewTextSort : IComparer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewTextSort(int sortColumn, bool ascending)
        {
            m_column = sortColumn;
            m_ascending = ascending;
        }

        /// <summary>
        /// Implementation of IComparer.Compare
        /// </summary>
        /// <param name="lhs">First Object to compare</param>
        /// <param name="rhs">Second Object to compare</param>
        /// <returns>Less that zero if lhs is less than rhs. Greater than zero if lhs greater that rhs. Zero if they are equal</returns>
        public Int32 Compare(Object lhs, Object rhs)
        {
            ListViewItem lhsLvi = lhs as ListViewItem;
            ListViewItem rhsLvi = rhs as ListViewItem;

            if (lhsLvi == null || rhsLvi == null)    // We only know how to sort ListViewItems, so return equal
                return 0;

            ListViewItem.ListViewSubItemCollection lhsItems = lhsLvi.SubItems;
            ListViewItem.ListViewSubItemCollection rhsItems = rhsLvi.SubItems;

            String lhsText = (lhsItems.Count > m_column) ? lhsItems[m_column].Text : String.Empty;
            String rhsText = (rhsItems.Count > m_column) ? rhsItems[m_column].Text : String.Empty;

            Int32 result = 0;
            if (lhsText.Length == 0 || rhsText.Length == 0)
                result = lhsText.CompareTo(rhsText);

            else
                result = OnCompare(lhsText, rhsText);

            if (!m_ascending)
                result = -result;

            return result;
        }

        /// <summary>
        /// Overridden to do type-specific comparision.
        /// </summary>
        /// <param name="lhs">First Object to compare</param>
        /// <param name="rhs">Second Object to compare</param>
        /// <returns>Less that zero if lhs is less than rhs. Greater than zero if lhs greater that rhs. Zero if they are equal</returns>
        protected virtual Int32 OnCompare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, false);
        }

        private Int32 m_column;
        private Boolean m_ascending;
    }

    /// <summary>
    /// Provides text sorting (case insensitive)
    /// </summary>
    public class ListViewTextCaseInsensitiveSort : ListViewTextSort
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewTextCaseInsensitiveSort(Int32 sortColumn, Boolean ascending) :
            base(sortColumn, ascending)
        {
        }

        /// <summary>
        /// Case-insensitive compare
        /// </summary>
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, true);
        }
    }

    /// <summary>
    /// Provides date sorting
    /// </summary>
    public class ListViewDateSort : ListViewTextSort
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewDateSort(Int32 sortColumn, Boolean ascending) :
            base(sortColumn, ascending)
        {
        }

        /// <summary>
        /// Date compare
        /// </summary>
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return DateTime.Parse(lhs).CompareTo(DateTime.Parse(rhs));
        }
    }

    /// <summary>
    /// Provides integer (32 bits) sorting
    /// </summary>
    public class ListViewInt32Sort : ListViewTextSort
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewInt32Sort(Int32 sortColumn, Boolean ascending) :
            base(sortColumn, ascending)
        {
        }

        /// <summary>
        /// Integer compare
        /// </summary>
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return Int32.Parse(lhs.Replace("KB", "").Trim(), NumberStyles.Number) - Int32.Parse(rhs.Replace("KB", "").Trim(), NumberStyles.Number);
        }
    }

    /// <summary>
    /// Provides integer (64 bits) sorting
    /// </summary>
    public class ListViewInt64Sort : ListViewTextSort
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewInt64Sort(Int32 sortColumn, Boolean ascending) :
            base(sortColumn, ascending)
        {
        }

        /// <summary>
        /// Integer compare
        /// </summary>
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            return (Int32)(Int32.Parse(lhs.Replace("KB", "").Trim(), NumberStyles.Number) - Int32.Parse(rhs.Replace("KB", "").Trim(), NumberStyles.Number));
        }
    }

    /// <summary>
    /// Provides floating-point sorting
    /// </summary>
    public class ListViewDoubleSort : ListViewTextSort
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        /// <param name="ascending">true, if ascending order, false otherwise</param>
        public ListViewDoubleSort(Int32 sortColumn, Boolean ascending) :
            base(sortColumn, ascending)
        {
        }

        /// <summary>
        /// Floating-point compare
        /// </summary>
        protected override Int32 OnCompare(String lhs, String rhs)
        {
            Double result = Double.Parse(lhs) - Double.Parse(rhs);

            if (result > 0)
                return 1;

            else if (result < 0)
                return -1;

            else
                return 0;
        }
    }

    #endregion

    #region ListViewSortManager

    /// <summary>
    /// Provides sorting of ListView columns 
    /// </summary>
    public class ListViewSortManager
    {
        private static readonly bool SUseNativeArrows = ComCtlDllSupportsArrows();

        /// <summary>
        /// Creates the ListView Sort Manager
        /// </summary>
        /// <param name="list">ListView that this manager will provide sorting to</param>
        /// <param name="comparers">Array of Types of comparers (One for each column)</param>
        /// <param name="column">Initial column to sort</param>
        /// <param name="order">Initial sort order</param>
        public ListViewSortManager(ListView list, Type[] comparers, Int32 column, SortOrder order)
        {
            m_column = -1;
            m_sortOrder = SortOrder.None;

            m_list = list;
            m_comparers = comparers;

            if (!SUseNativeArrows)
            {
                m_imgList = new ImageList();
                m_imgList.ImageSize = new Size(8, 8);
                m_imgList.TransparentColor = System.Drawing.Color.Magenta;

                m_imgList.Images.Add(GetArrowBitmap(ArrowType.Ascending));		// Add ascending arrow
                m_imgList.Images.Add(GetArrowBitmap(ArrowType.Descending));		// Add descending arrow

                SetHeaderImageList(m_list, m_imgList);
            }

            list.ColumnClick += new ColumnClickEventHandler(ColumnClick);

            if (column != -1)
                Sort(column, order);
        }

        /// <summary>
        /// Creates the ListView Sort Manager
        /// </summary>
        /// <param name="list">ListView that this manager will provide sorting to</param>
        /// <param name="comparers">Array of Types of comparers (One for each column)</param>
        public ListViewSortManager(ListView list, Type[] comparers) :
            this(list, comparers, -1, SortOrder.None)
        {
        }

        /// <summary>
        /// Returns the current sort column
        /// </summary>
        public Int32 Column
        {
            get { return m_column; }
        }

        /// <summary>
        /// Returns the current sort order
        /// </summary>
        public SortOrder SortOrder
        {
            get { return m_sortOrder; }
        }

        /// <summary>
        /// Returns the type of the comparer for the given column
        /// </summary>
        /// <param name="column">Column index</param>
        /// <returns></returns>
        public Type GetColumnComparerType(Int32 column)
        {
            return m_comparers[column];
        }

        /// <summary>
        /// Sets the type of the comparer for the given column
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="comparerType">Comparer type</param>
        public void SetColumnComparerType(Int32 column, Type comparerType)
        {
            m_comparers[column] = comparerType;
        }

        /// <summary>
        /// Reassigns the comparer types for all the columns
        /// </summary>
        /// <param name="comparers">Array of Types of comparers (One for each column)</param>
        public void SetComparerTypes(Type[] comparers)
        {
            m_comparers = comparers;
        }

        /// <summary>
        /// Sorts the rows based on the given column and the current sort order
        /// </summary>
        /// <param name="sortColumn">Column to be sorted</param>
        public void Sort(Int32 column)
        {
            SortOrder order = SortOrder.Ascending;

            if (column == m_column)
                order = (m_sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

            Sort(column, order);
        }

        /// <summary>
        /// Sorts the rows based on the given column and sort order
        /// </summary>
        /// <param name="column">Column to be sorted</param>
        /// <param name="order">Sort order</param>
        public void Sort(Int32 column, SortOrder order)
        {
            if (column < 0 || column >= m_comparers.Length)
                throw new IndexOutOfRangeException();

            if (column != m_column)
            {
                ShowHeaderIcon(m_list, m_column, SortOrder.None);
                m_column = column;
            }

            ShowHeaderIcon(m_list, m_column, order);
            m_sortOrder = order;

            if (m_sortOrder != SortOrder.None)
            {
                ListViewTextSort comp = (ListViewTextSort)Activator.CreateInstance(m_comparers[m_column], new Object[] { m_column, m_sortOrder == SortOrder.Ascending });
                m_list.ListViewItemSorter = comp;
            }

            else
                m_list.ListViewItemSorter = null;
        }

        /// <summary>
        /// Enables/Disables list sorting
        /// </summary>
        public Boolean SortEnabled
        {
            get
            {
                return m_list.ListViewItemSorter != null;
            }

            set
            {
                if (value)
                {
                    if (!this.SortEnabled)
                    {
                        m_list.ColumnClick += new ColumnClickEventHandler(ColumnClick);
                        m_list.ListViewItemSorter = (ListViewTextSort)Activator.CreateInstance(m_comparers[m_column], new Object[] { m_column, m_sortOrder == SortOrder.Ascending });
                        ShowHeaderIcon(m_list, m_column, m_sortOrder);
                    }
                }

                else
                {
                    if (this.SortEnabled)
                    {
                        m_list.ColumnClick -= new ColumnClickEventHandler(ColumnClick);
                        m_list.ListViewItemSorter = null;
                        ShowHeaderIcon(m_list, m_column, SortOrder.None);
                    }
                }
            }
        }

        /// <summary>
        /// ColumnClick event handler
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void ColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.Sort(e.Column);
        }

        private Int32 m_column;
        private SortOrder m_sortOrder;
        private ListView m_list;
        private Type[] m_comparers;
        private ImageList m_imgList;

        #region Graphics

        enum ArrowType { Ascending, Descending }

        Bitmap GetArrowBitmap(ArrowType type)
        {
            Bitmap bmp = new Bitmap(8, 8);
            Graphics gfx = Graphics.FromImage(bmp);

            Pen lightPen = SystemPens.ControlLightLight;
            Pen shadowPen = SystemPens.ControlDark;

            gfx.FillRectangle(System.Drawing.Brushes.Magenta, 0, 0, 8, 8);

            if (type == ArrowType.Ascending)
            {
                gfx.DrawLine(lightPen, 0, 7, 7, 7);
                gfx.DrawLine(lightPen, 7, 7, 4, 0);
                gfx.DrawLine(shadowPen, 3, 0, 0, 7);
            }

            else if (type == ArrowType.Descending)
            {
                gfx.DrawLine(lightPen, 4, 7, 7, 0);
                gfx.DrawLine(shadowPen, 3, 7, 0, 0);
                gfx.DrawLine(shadowPen, 0, 0, 7, 0);
            }

            gfx.Dispose();

            return bmp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HDITEM
        {
            public Int32 mask;
            public Int32 cxy;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 lParam;
            public Int32 iImage;
            public Int32 iOrder;
        };

        [DllImport("user32")]
        static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", EntryPoint = "SendMessage")]
        static extern IntPtr SendMessage2(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

        const Int32 HDI_WIDTH = 0x0001;
        const Int32 HDI_HEIGHT = HDI_WIDTH;
        const Int32 HDI_TEXT = 0x0002;
        const Int32 HDI_FORMAT = 0x0004;
        const Int32 HDI_LPARAM = 0x0008;
        const Int32 HDI_BITMAP = 0x0010;
        const Int32 HDI_IMAGE = 0x0020;
        const Int32 HDI_DI_SETITEM = 0x0040;
        const Int32 HDI_ORDER = 0x0080;
        const Int32 HDI_FILTER = 0x0100;		// 0x0500

        const Int32 HDF_LEFT = 0x0000;
        const Int32 HDF_RIGHT = 0x0001;
        const Int32 HDF_CENTER = 0x0002;
        const Int32 HDF_JUSTIFYMASK = 0x0003;
        const Int32 HDF_RTLREADING = 0x0004;
        const Int32 HDF_OWNERDRAW = 0x8000;
        const Int32 HDF_STRING = 0x4000;
        const Int32 HDF_BITMAP = 0x2000;
        const Int32 HDF_BITMAP_ON_RIGHT = 0x1000;
        const Int32 HDF_IMAGE = 0x0800;
        const Int32 HDF_SORTUP = 0x0400;		// 0x0501
        const Int32 HDF_SORTDOWN = 0x0200;		// 0x0501

        const Int32 LVM_FIRST = 0x1000;		// List messages
        const Int32 LVM_GETHEADER = LVM_FIRST + 31;

        const Int32 HDM_FIRST = 0x1200;		// Header messages
        const Int32 HDM_SETIMAGELIST = HDM_FIRST + 8;
        const Int32 HDM_GETIMAGELIST = HDM_FIRST + 9;
        const Int32 HDM_GETITEM = HDM_FIRST + 11;
        const Int32 HDM_SETITEM = HDM_FIRST + 12;

        private void ShowHeaderIcon(ListView list, int columnIndex, SortOrder sortOrder)
        {
            if (columnIndex < 0 || columnIndex >= list.Columns.Count)
                return;

            IntPtr hHeader = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            ColumnHeader colHdr = list.Columns[columnIndex];

            HDITEM hd = new HDITEM();
            hd.mask = HDI_FORMAT;

            HorizontalAlignment align = colHdr.TextAlign;

            if (align == HorizontalAlignment.Left)
                hd.fmt = HDF_LEFT | HDF_STRING | HDF_BITMAP_ON_RIGHT;

            else if (align == HorizontalAlignment.Center)
                hd.fmt = HDF_CENTER | HDF_STRING | HDF_BITMAP_ON_RIGHT;

            else	// HorizontalAlignment.Right
                hd.fmt = HDF_RIGHT | HDF_STRING;

            if (SUseNativeArrows)
            {
                if (sortOrder == SortOrder.Ascending)
                    hd.fmt |= HDF_SORTUP;

                else if (sortOrder == SortOrder.Descending)
                    hd.fmt |= HDF_SORTDOWN;
            }
            else
            {
                hd.mask |= HDI_IMAGE;

                if (sortOrder != SortOrder.None)
                    hd.fmt |= HDF_IMAGE;

                hd.iImage = (int)sortOrder - 1;
            }

            SendMessage2(hHeader, HDM_SETITEM, new IntPtr(columnIndex), ref hd);
        }

        private void SetHeaderImageList(ListView list, ImageList imgList)
        {
            IntPtr hHeader = SendMessage(list.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hHeader, HDM_SETIMAGELIST, IntPtr.Zero, imgList.Handle);
        }
        #endregion

        #region ComCtrl information

        [StructLayout(LayoutKind.Sequential)]
        private struct DLLVERSIONINFO
        {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("comctl32.dll")]
        static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);

        static private bool ComCtlDllSupportsArrows()
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = LoadLibrary("comctl32.dll");
                if (hModule != IntPtr.Zero)
                {
                    UIntPtr proc = GetProcAddress(hModule, "DllGetVersion");
                    if (proc == UIntPtr.Zero)    // Old versions don't support this method
                        return false;
                }

                DLLVERSIONINFO vi = new DLLVERSIONINFO();
                vi.cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO));

                DllGetVersion(ref vi);

                return vi.dwMajorVersion >= 6;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    FreeLibrary(hModule);
            }
        }

        #endregion


    }

    #endregion
}
