using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace Ionic.Utils
{
    public class FolderBrowserDialogEx : CommonDialog
    {
        // Fields
        private PInvoke.BrowseFolderCallbackProc _callback;
        private string _descriptionText;
        private bool _dontIncludeNetworkFoldersBelowDomainLevel;
        private IntPtr _hwndEdit;
        private bool _newStyle = true;
        private Environment.SpecialFolder _rootFolder;
        private IntPtr _rootFolderLocation;
        private string _selectedPath;
        private bool _selectedPathNeedsCheck;
        private bool _showBothFilesAndFolders;
        private bool _showEditBox;
        private bool _showFullPathInEditBox = true;
        private bool _showNewFolderButton;
        private int _uiFlags;
        private static readonly int MAX_PATH = 260;

        // Events
        public event EventHandler HelpRequest
        {
            add { base.HelpRequest += value; }
            remove { base.HelpRequest -= value; }
        }

        // Methods
        public FolderBrowserDialogEx()
        {
            this.Reset();
        }

        private void BecomeComputerBrowser()
        {
            this._uiFlags += 0x1000;
            this.Description = "Select a computer:";
            PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, 0x12, ref this._rootFolderLocation);
            this.ShowNewFolderButton = false;
            this.ShowEditBox = false;
        }

        private void BecomePrinterBrowser()
        {
            this._uiFlags += 0x2000;
            this.Description = "Select a printer:";
            PInvoke.Shell32.SHGetSpecialFolderLocation(IntPtr.Zero, 4, ref this._rootFolderLocation);
            this.ShowNewFolderButton = false;
            this.ShowEditBox = false;
        }

        public static FolderBrowserDialogEx ComputerBrowser()
        {
            FolderBrowserDialogEx ex = new FolderBrowserDialogEx();
            ex.BecomeComputerBrowser();
            return ex;
        }

        private int FolderBrowserCallback(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
        {
            switch (msg)
            {
                case 1:
                    if (this._selectedPath.Length != 0)
                    {
                        PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 0x467, 1, this._selectedPath);
                        if (this._showEditBox && this._showFullPathInEditBox)
                        {
                            this._hwndEdit = PInvoke.User32.FindWindowEx(new HandleRef(null, hwnd), IntPtr.Zero, "Edit",
                                                                         null);
                            PInvoke.User32.SetWindowText(this._hwndEdit, this._selectedPath);
                        }
                    }
                    break;

                case 2:
                    {
                        IntPtr pidl = lParam;
                        if (pidl != IntPtr.Zero)
                        {
                            if (((this._uiFlags & 0x2000) != 0x2000) && ((this._uiFlags & 0x1000) != 0x1000))
                            {
                                IntPtr pszPath = Marshal.AllocHGlobal((int) (MAX_PATH*Marshal.SystemDefaultCharSize));
                                bool flag = PInvoke.Shell32.SHGetPathFromIDList(pidl, pszPath);
                                string str = Marshal.PtrToStringAuto(pszPath);
                                Marshal.FreeHGlobal(pszPath);
                                PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 0x465, 0, flag ? 1 : 0);
                                if (flag && !string.IsNullOrEmpty(str))
                                {
                                    if ((this._showEditBox && this._showFullPathInEditBox) &&
                                        (this._hwndEdit != IntPtr.Zero))
                                    {
                                        PInvoke.User32.SetWindowText(this._hwndEdit, str);
                                    }
                                    if ((this._uiFlags & 4) == 4)
                                    {
                                        PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 0x464, 0, str);
                                    }
                                }
                                break;
                            }
                            PInvoke.User32.SendMessage(new HandleRef(null, hwnd), 0x465, 0, 1);
                        }
                        break;
                    }
            }
            return 0;
        }

        private static PInvoke.IMalloc GetSHMalloc()
        {
            PInvoke.IMalloc[] ppMalloc = new PInvoke.IMalloc[1];
            PInvoke.Shell32.SHGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        public static FolderBrowserDialogEx PrinterBrowser()
        {
            FolderBrowserDialogEx ex = new FolderBrowserDialogEx();
            ex.BecomePrinterBrowser();
            return ex;
        }

        public override void Reset()
        {
            this._rootFolder = Environment.SpecialFolder.Desktop;
            this._descriptionText = string.Empty;
            this._selectedPath = string.Empty;
            this._selectedPathNeedsCheck = false;
            this._showNewFolderButton = true;
            this._showEditBox = true;
            this._newStyle = true;
            this._dontIncludeNetworkFoldersBelowDomainLevel = false;
            this._hwndEdit = IntPtr.Zero;
            this._rootFolderLocation = IntPtr.Zero;
        }

        protected override bool RunDialog(IntPtr hWndOwner)
        {
            bool flag = false;
            if (this._rootFolderLocation == IntPtr.Zero)
            {
                PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int) this._rootFolder,
                                                           ref this._rootFolderLocation);
                if (this._rootFolderLocation == IntPtr.Zero)
                {
                    PInvoke.Shell32.SHGetSpecialFolderLocation(hWndOwner, 0, ref this._rootFolderLocation);
                    if (this._rootFolderLocation == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("FolderBrowserDialogNoRootFolder");
                    }
                }
            }
            this._hwndEdit = IntPtr.Zero;
            if (this._dontIncludeNetworkFoldersBelowDomainLevel)
            {
                this._uiFlags += 2;
            }
            if (this._newStyle)
            {
                this._uiFlags += 0x40;
            }
            if (!this._showNewFolderButton)
            {
                this._uiFlags += 0x200;
            }
            if (this._showEditBox)
            {
                this._uiFlags += 0x10;
            }
            if (this._showBothFilesAndFolders)
            {
                this._uiFlags += 0x4000;
            }
            if (Control.CheckForIllegalCrossThreadCalls && (Application.OleRequired() != ApartmentState.STA))
            {
                throw new ThreadStateException("DebuggingException: ThreadMustBeSTA");
            }
            IntPtr zero = IntPtr.Zero;
            IntPtr hglobal = IntPtr.Zero;
            IntPtr pszPath = IntPtr.Zero;
            try
            {
                PInvoke.BROWSEINFO lpbi = new PInvoke.BROWSEINFO();
                hglobal = Marshal.AllocHGlobal((int) (MAX_PATH*Marshal.SystemDefaultCharSize));
                pszPath = Marshal.AllocHGlobal((int) (MAX_PATH*Marshal.SystemDefaultCharSize));
                this._callback = new PInvoke.BrowseFolderCallbackProc(this.FolderBrowserCallback);
                lpbi.pidlRoot = this._rootFolderLocation;
                lpbi.Owner = hWndOwner;
                lpbi.pszDisplayName = hglobal;
                lpbi.Title = this._descriptionText;
                lpbi.Flags = this._uiFlags;
                lpbi.callback = this._callback;
                lpbi.lParam = IntPtr.Zero;
                lpbi.iImage = 0;
                zero = PInvoke.Shell32.SHBrowseForFolder(lpbi);
                if (((this._uiFlags & 0x2000) == 0x2000) || ((this._uiFlags & 0x1000) == 0x1000))
                {
                    this._selectedPath = Marshal.PtrToStringAuto(lpbi.pszDisplayName);
                    return true;
                }
                if (zero != IntPtr.Zero)
                {
                    PInvoke.Shell32.SHGetPathFromIDList(zero, pszPath);
                    this._selectedPathNeedsCheck = true;
                    this._selectedPath = Marshal.PtrToStringAuto(pszPath);
                    flag = true;
                }
            }
            finally
            {
                PInvoke.IMalloc sHMalloc = GetSHMalloc();
                sHMalloc.Free(this._rootFolderLocation);
                this._rootFolderLocation = IntPtr.Zero;
                if (zero != IntPtr.Zero)
                {
                    sHMalloc.Free(zero);
                }
                if (pszPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszPath);
                }
                if (hglobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hglobal);
                }
                this._callback = null;
            }
            return flag;
        }

        // Properties
        public string Description
        {
            get { return this._descriptionText; }
            set { this._descriptionText = (value == null) ? string.Empty : value; }
        }

        public bool DontIncludeNetworkFoldersBelowDomainLevel
        {
            get { return this._dontIncludeNetworkFoldersBelowDomainLevel; }
            set { this._dontIncludeNetworkFoldersBelowDomainLevel = value; }
        }

        public bool NewStyle
        {
            get { return this._newStyle; }
            set { this._newStyle = value; }
        }

        public Environment.SpecialFolder RootFolder
        {
            get { return this._rootFolder; }
            set
            {
                if (!Enum.IsDefined(typeof (Environment.SpecialFolder), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof (Environment.SpecialFolder));
                }
                this._rootFolder = value;
            }
        }

        public string SelectedPath
        {
            get
            {
                if (((this._selectedPath != null) && (this._selectedPath.Length != 0)) && this._selectedPathNeedsCheck)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this._selectedPath).Demand();
                    this._selectedPathNeedsCheck = false;
                }
                return this._selectedPath;
            }
            set
            {
                this._selectedPath = (value == null) ? string.Empty : value;
                this._selectedPathNeedsCheck = true;
            }
        }

        public bool ShowBothFilesAndFolders
        {
            get { return this._showBothFilesAndFolders; }
            set { this._showBothFilesAndFolders = value; }
        }

        public bool ShowEditBox
        {
            get { return this._showEditBox; }
            set { this._showEditBox = value; }
        }

        public bool ShowFullPathInEditBox
        {
            get { return this._showFullPathInEditBox; }
            set { this._showFullPathInEditBox = value; }
        }

        public bool ShowNewFolderButton
        {
            get { return this._showNewFolderButton; }
            set { this._showNewFolderButton = value; }
        }

        // Nested Types
        private class BrowseFlags
        {
            // Fields
            public const int BIF_BROWSEFORCOMPUTER = 0x1000;
            public const int BIF_BROWSEFORPRINTER = 0x2000;
            public const int BIF_BROWSEINCLUDEFILES = 0x4000;
            public const int BIF_BROWSEINCLUDEURLS = 0x80;
            public const int BIF_DEFAULT = 0;
            public const int BIF_DONTGOBELOWDOMAIN = 2;
            public const int BIF_EDITBOX = 0x10;
            public const int BIF_NEWDIALOGSTYLE = 0x40;
            public const int BIF_NONEWFOLDERBUTTON = 0x200;
            public const int BIF_NOTRANSLATETARGETS = 0x400;
            public const int BIF_RETURNFSANCESTORS = 8;
            public const int BIF_RETURNONLYFSDIRS = 1;
            public const int BIF_SHAREABLE = 0x8000;
            public const int BIF_STATUSTEXT = 4;
            public const int BIF_UAHINT = 0x100;
            public const int BIF_VALIDATE = 0x20;
        }

        private static class BrowseForFolderMessages
        {
            // Fields
            public const int BFFM_ENABLEOK = 0x465;
            public const int BFFM_INITIALIZED = 1;
            public const int BFFM_IUNKNOWN = 5;
            public const int BFFM_SELCHANGED = 2;
            public const int BFFM_SETSELECTIONA = 0x466;
            public const int BFFM_SETSELECTIONW = 0x467;
            public const int BFFM_SETSTATUSTEXT = 0x464;
            public const int BFFM_VALIDATEFAILEDA = 3;
            public const int BFFM_VALIDATEFAILEDW = 4;
        }

        private class CSIDL
        {
            // Fields
            public const int NETWORK = 0x12;
            public const int PRINTERS = 4;
        }
    }


    internal static class PInvoke
    {
        // Nested Types
        public delegate int BrowseFolderCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class BROWSEINFO
        {
            public IntPtr Owner;
            public IntPtr pidlRoot;
            public IntPtr pszDisplayName;
            public string Title;
            public int Flags;
            public PInvoke.BrowseFolderCallbackProc callback;
            public IntPtr lParam;
            public int iImage;
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity,
         Guid("00000002-0000-0000-c000-000000000046")]
        public interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class Shell32
        {
            // Methods
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SHBrowseForFolder([In] PInvoke.BROWSEINFO lpbi);

            [DllImport("shell32.dll")]
            public static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] PInvoke.IMalloc[] ppMalloc);

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

            [DllImport("shell32.dll")]
            public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);
        }

        internal static class User32
        {
            // Methods
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindowEx(HandleRef hwndParent, IntPtr hwndChildAfter, string lpszClass,
                                                     string lpszWindow);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetWindowText(IntPtr hWnd, string text);
        }
    }

}

