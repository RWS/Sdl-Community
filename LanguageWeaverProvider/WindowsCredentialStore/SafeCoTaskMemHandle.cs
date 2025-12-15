using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace LanguageWeaverProvider.WindowsCredentialStore;

public sealed class SafeCoTaskMemHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public SafeCoTaskMemHandle(IntPtr handle) : base(true)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        Marshal.FreeCoTaskMem(handle);
        return true;
    }
}