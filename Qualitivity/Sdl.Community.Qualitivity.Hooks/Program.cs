using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Hooks
{
    public class Viewer
    {

        public static bool IsTracking { get; set; }
        public static KeyStroke KsCache { get; set; }

        public static bool LastKeyWasLetter { get; set; }


        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;
        private static readonly LowLevelKeyboardProc Proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        static Viewer()
        {
            LastKeyWasLetter = false;
            IsTracking = true;
            KsCache = new KeyStroke();
        }
        public static void StartTracking()
        {
            _hookId = SetHook(Proc);
        }

        public static void StopTracking()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static void ToggleCapsLock()
        {
            const int keyeventfExtendedkey = 0x1;
            const int keyeventfKeyup = 0x2;

            UnhookWindowsHookEx(_hookId);
            keybd_event(0x14, 0x45, keyeventfExtendedkey, (UIntPtr)0);
            keybd_event(0x14, 0x45, keyeventfExtendedkey | keyeventfKeyup, (UIntPtr)0);
            _hookId = SetHook(Proc);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 || wParam != (IntPtr) WmKeydown) return CallNextHookEx(_hookId, nCode, wParam, lParam);
     
            if (!IsTracking) return CallNextHookEx(_hookId, nCode, wParam, lParam);
            var key = (Keys)Marshal.ReadInt32(lParam);

            KsCache = new KeyStroke {Created = DateTime.Now};

            if ((int)key <= 32 || key == Keys.Delete || key == Keys.Tab || key == Keys.Back || key == Keys.Space)
                KsCache.Key = "[" + key + "]";
            else
                KsCache.Key = key.ToString();
                  
            KsCache.Text = KsCache.Key; //not used here
               
            KsCache.Shift = (Control.ModifierKeys & Keys.Shift) != 0;

            KsCache.Alt = (Control.ModifierKeys & Keys.Alt) != 0;

            KsCache.Ctrl = (Control.ModifierKeys & Keys.Control) != 0;


            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

       
    }
}
