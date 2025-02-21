using System;
using System.Windows.Forms;

namespace Sdl.Community.InSource.Helpers
{
    public class FolderSelectDialog : IDisposable
    {
        private OpenFileDialog ofd;

        public string FileName
        {
            get
            {
                return this.ofd.FileName;
            }
        }

        public string InitialDirectory
        {
            get
            {
                return this.ofd.InitialDirectory;
            }
            set
            {
                string str;
                OpenFileDialog openFileDialog = this.ofd;
                str = (value == null || value.Length == 0 ? Environment.CurrentDirectory : value);
                openFileDialog.InitialDirectory = str;
            }
        }

        public string Title
        {
            get
            {
                return this.ofd.Title;
            }
            set
            {
                string str;
                OpenFileDialog openFileDialog = this.ofd;
                str = (value == null ? "Select a folder" : value);
                openFileDialog.Title = str;
            }
        }

        public FolderSelectDialog()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "Folders|\n";
            ofd.AddExtension = false;
            ofd.CheckFileExists = false;
            ofd.DereferenceLinks = true;
            ofd.Multiselect = false;
        }

        public bool ShowDialog()
        {
            return this.ShowDialog(IntPtr.Zero);
        }

        public bool ShowDialog(IntPtr hWndOwner)
        {
            bool flag = false;

            Reflector reflector = new Reflector("System.Windows.Forms");
            uint num = 0;
            Type type = reflector.GetType("FileDialogNative.IFileDialog");
            object obj = reflector.Call(this.ofd, "CreateVistaDialog", new object[0]);
            object[] objArray = new object[] { obj };
            reflector.Call(this.ofd, "OnBeforeVistaDialog", objArray);
            uint @enum = (uint)reflector.CallAs(typeof(FileDialog), this.ofd, "GetOptions", new object[0]);
            @enum = @enum | (uint)reflector.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
            object[] objArray1 = new object[] { @enum };
            reflector.CallAs(type, obj, "SetOptions", objArray1);
            object[] objArray2 = new object[] { this.ofd };
            object obj1 = reflector.New("FileDialog.VistaDialogEvents", objArray2);
            object[] objArray3 = new object[] { obj1, num };
            object[] objArray4 = objArray3;
            reflector.CallAs2(type, obj, "Advise", objArray4);
            num = (uint)objArray4[1];
            try
            {
                object[] objArray5 = new object[] { hWndOwner };
                int num1 = (int)reflector.CallAs(type, obj, "Show", objArray5);
                flag = 0 == num1;
            }
            finally
            {
                object[] objArray6 = new object[] { num };
                reflector.CallAs(type, obj, "Unadvise", objArray6);
                GC.KeepAlive(obj1);
            }

            return flag;
        }
        public void Dispose()
        {
            
        }
    }
}
