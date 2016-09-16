using System;
using System.Reflection;
using System.Windows.Forms;
using Ionic.Utils;
using log4net;

namespace FolderSelect
{
    public class FolderSelectDialog
    {
        // Fields
        private OpenFileDialog ofd = new OpenFileDialog();

        // Methods
        public FolderSelectDialog()
        {
            this.ofd.Filter = "Folders|\n";
            this.ofd.AddExtension = false;
            this.ofd.CheckFileExists = false;
            this.ofd.DereferenceLinks = true;
            this.ofd.Multiselect = false;
        }

        public bool ShowDialog()
        {
            return this.ShowDialog(IntPtr.Zero);
        }

        public bool ShowDialog(IntPtr hWndOwner)
        {
            if ((Environment.OSVersion.Version.Major >= 6) && !this.UseOldDialog)
            {
                Reflector reflector = new Reflector("System.Windows.Forms");
                uint num = 0;
                Type type = reflector.GetType("FileDialogNative.IFileDialog");
                object obj2 = reflector.Call(this.ofd, "CreateVistaDialog", new object[0]);
                reflector.Call(this.ofd, "OnBeforeVistaDialog", new object[] {obj2});
                uint num2 = (uint) reflector.CallAs(typeof (FileDialog), this.ofd, "GetOptions", new object[0]);
                num2 |= (uint) reflector.GetEnum("FileDialogNative.FOS", "FOS_PICKFOLDERS");
                reflector.CallAs(type, obj2, "SetOptions", new object[] {num2});
                object obj3 = reflector.New("FileDialog.VistaDialogEvents", new object[] {this.ofd});
                object[] parameters = new object[] {obj3, num};
                reflector.CallAs2(type, obj2, "Advise", parameters);
                num = (uint) parameters[1];
                try
                {
                    int num3 = (int) reflector.CallAs(type, obj2, "Show", new object[] {hWndOwner});
                    return (0 == num3);
                }
                finally
                {
                    reflector.CallAs(type, obj2, "Unadvise", new object[] {num});
                    GC.KeepAlive(obj3);
                }
            }
            FolderBrowserDialogEx ex = new FolderBrowserDialogEx();
            ex.Description = this.Title;
            ex.SelectedPath = this.InitialDirectory;
            ex.ShowNewFolderButton = true;
            ex.ShowEditBox = true;
            ex.ShowFullPathInEditBox = true;
            if (ex.ShowDialog(new WindowWrapper(hWndOwner)) != DialogResult.OK)
            {
                return false;
            }
            this.ofd.FileName = ex.SelectedPath;
            return true;
        }

        // Properties
        public string FileName
        {
            get { return this.ofd.FileName; }
        }

        public string InitialDirectory
        {
            get { return this.ofd.InitialDirectory; }
            set
            {
                this.ofd.InitialDirectory = ((value == null) || (value.Length == 0))
                                                ? Environment.CurrentDirectory
                                                : value;
            }
        }

        public string Title
        {
            get { return this.ofd.Title; }
            set { this.ofd.Title = (value == null) ? "Select a folder" : value; }
        }

        public bool UseOldDialog { get; set; }
    }



    public class Reflector
    {
        // Fields
        private Assembly m_asmb;
        private string m_ns;
        private ILog logger = LogManager.GetLogger(typeof (Reflector));

        // Methods
        public Reflector(string ns)
            : this(ns, ns)
        {
        }

        public Reflector(string an, string ns)
        {
            this.m_ns = ns;
            this.m_asmb = null;
            foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (name.FullName.StartsWith(an))
                {
                    this.m_asmb = Assembly.Load(name);
                    return;
                }
            }
        }

        public object Call(object obj, string func, params object[] parameters)
        {
            return this.Call2(obj, func, parameters);
        }

        public object Call2(object obj, string func, object[] parameters)
        {
            return this.CallAs2(obj.GetType(), obj, func, parameters);
        }

        public object CallAs(Type type, object obj, string func, params object[] parameters)
        {
            return this.CallAs2(type, obj, func, parameters);
        }

        public object CallAs2(Type type, object obj, string func, object[] parameters)
        {
            return type.GetMethod(func, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Invoke(obj, parameters);
        }

        public object Get(object obj, string prop)
        {
            return this.GetAs(obj.GetType(), obj, prop);
        }

        public object GetAs(Type type, object obj, string prop)
        {
            return type.GetProperty(prop, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null);
        }

        public object GetEnum(string typeName, string name)
        {
            return this.GetType(typeName).GetField(name).GetValue(null);
        }

        public Type GetType(string typeName)
        {
            Type nestedType = null;
            string[] strArray = typeName.Split(new char[] { '.' });
            if (strArray.Length > 0)
            {
                nestedType = this.m_asmb.GetType(this.m_ns + "." + strArray[0]);
            }
            for (int i = 1; i < strArray.Length; i++)
            {
                nestedType = nestedType.GetNestedType(strArray[i], BindingFlags.NonPublic);
            }
            return nestedType;
        }

        public object New(string name, params object[] parameters)
        {
            foreach (ConstructorInfo info in this.GetType(name).GetConstructors())
            {
                try
                {
                    return info.Invoke(parameters);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
            }
            return null;
        }
    }

    public class WindowWrapper : IWin32Window
    {
        // Fields
        private IntPtr _hwnd;

        // Methods
        public WindowWrapper(IntPtr handle)
        {
            this._hwnd = handle;
        }

        // Properties
        public IntPtr Handle
        {
            get
            {
                return this._hwnd;
            }
        }
    }


}
 
