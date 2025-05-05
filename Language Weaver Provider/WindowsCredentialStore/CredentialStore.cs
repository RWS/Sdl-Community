using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace LanguageWeaverProvider.WindowsCredentialStore;

public static class CredentialStore
{
    private const string Target = "LanguageWeaverProviderCredentials";
    private const uint Generic = 1;
    private const uint PersistLocalMachine = 2;

    public static void Save(string key, string secret)
    {
        using var blob = SecureStringToCoTaskMem(secret);
        var cred = new CREDENTIAL
        {
            Type = Generic,
            TargetName = key,
            CredentialBlob = blob.DangerousGetHandle(),
            CredentialBlobSize = (uint)Encoding.Unicode.GetByteCount(secret),
            Persist = PersistLocalMachine,
            UserName = ""
        };

        if (!CredWrite(ref cred, 0))
            throw new Win32Exception(Marshal.GetLastWin32Error());
    }

    public static string Load(string key)
    {
        if (!CredRead(key, Generic, 0, out var ptr))
            return null;

        try
        {
            var cred = Marshal.PtrToStructure<CREDENTIAL>(ptr);
            return Marshal.PtrToStringUni(cred.CredentialBlob, (int)cred.CredentialBlobSize / 2);
        }
        finally
        {
            CredFree(ptr);
        }
    }


    public static void Delete(string key)
    {
        CredDelete(Target, Generic, 0);
    }

    // ---- internals hidden ----
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public uint Flags, Type;
        public string TargetName, Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public uint Persist, AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias, UserName;
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredWrite([In] ref CREDENTIAL cred, uint flags);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredRead(string target, uint type, uint flags, out IntPtr ptr);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern void CredFree(IntPtr buffer);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredDelete(string target, uint type, uint flags);

    private static SafeCoTaskMemHandle SecureStringToCoTaskMem(string s)
    {
        var bytes = Encoding.Unicode.GetBytes(s);
        var handle = new SafeCoTaskMemHandle(Marshal.AllocCoTaskMem(bytes.Length));
        Marshal.Copy(bytes, 0, handle.DangerousGetHandle(), bytes.Length);
        return handle;
    }
}