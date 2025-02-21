using System;
using Microsoft.Win32;
using Trados.TargetRenamer.Helpers;
using Trados.TargetRenamer.Interfaces;

namespace Trados.TargetRenamer.Services
{
	public class FolderDialogService : IFolderDialogService
    {
        public string ShowDialog()
        {
            var folderPath = string.Empty;
            var folderDialog = new FolderSelectDialog
            {
                Title = "Select Folder"
            };
            if (folderDialog.ShowDialog())
            {
                folderPath = folderDialog.FileName;
            }

            return folderPath;
        }

        public class FolderSelectDialog
        {
            private readonly OpenFileDialog _ofd;

            public FolderSelectDialog()
            {
                _ofd = new OpenFileDialog
                {
                    Filter = "Folders|\n",
                    AddExtension = false,
                    CheckFileExists = false,
                    DereferenceLinks = true,
                    Multiselect = false
                };
            }

            public string FileName => _ofd.FileName;

            public string Title
            {
                get => _ofd.Title;
                set
                {
                    var openFileDialog = _ofd;
                    var str = value ?? "Select a folder";
                    openFileDialog.Title = str;
                }
            }

            public bool ShowDialog()
            {
                return ShowDialog(IntPtr.Zero);
            }

            public bool ShowDialog(IntPtr hWndOwner)
            {
                bool flag;

                var reflector = new Reflector("System.Windows.Forms");
                uint num = 0;
                var type = reflector.GetType("FileDialogNative.IFileDialog");
                var obj = reflector.Call(_ofd, "CreateVistaDialog");
				object[] objArray = [obj];
                reflector.Call(_ofd, "OnBeforeVistaDialog", objArray);
                var @enum = (uint)reflector.CallAs(typeof(FileDialog), _ofd, "GetOptions");
                @enum |= (uint)reflector.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
				object[] objArray1 = [@enum];
                reflector.CallAs(type, obj, "SetOptions", objArray1);
				object[] objArray2 = [_ofd];
                var obj1 = reflector.New("FileDialog.VistaDialogEvents", objArray2);
                object[] objArray3 = [obj1, num];
                var objArray4 = objArray3;
                reflector.CallAs2(type, obj, "Advise", objArray4);
                num = (uint)objArray4[1];
                try
                {
                    object[] objArray5 = [hWndOwner];
                    var num1 = (int)reflector.CallAs(type, obj, "Show", objArray5);
                    flag = 0 == num1;
                }
                finally
                {
                    object[] objArray6 = [num];
                    reflector.CallAs(type, obj, "Unadvise", objArray6);
                    GC.KeepAlive(obj1);
                }

                return flag;
            }
        }
    }
}