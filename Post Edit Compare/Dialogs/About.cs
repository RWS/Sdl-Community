using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            Text = string.Format(PluginResources.About_SDL_Trados_Studio_2015, AssemblyProduct);
            labelProductNameAndVersion.Text = AssemblyProduct + " - " + string.Format(PluginResources.Version_0, AssemblyVersion);
            linkLabelpostEditCompareNameAndVersion.Text = string.Format(PluginResources.Post_Edit_Compare_Version_0, GetPostEditCompareProductVersion());
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = CopyRight;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


        private static readonly string CopyRight = PluginResources.About_CopyRight_00
                                                   + PluginResources.About_CopyRight_01
                                                   + PluginResources.About_CopyRight_02
                                                   + PluginResources.About_CopyRight_03
                                                   + PluginResources.About_CopyRight_04
                                                   + PluginResources.About_CopyRight_05
                                                   + "\r\n";

        #region Assembly Attribute Accessors

        public string GetPostEditCompareProductVersion()
        {
            var productVersion = string.Empty;

            var fileVersionInfo = PostEditCompareVersionInfo();
            return fileVersionInfo != null ? fileVersionInfo.ProductVersion : productVersion;
        }

        public FileVersionInfo PostEditCompareVersionInfo()
        {
            FileVersionInfo versionInfo = null;

            var sPath = Path.Combine(Application.StartupPath, "PostEdit.Compare.exe");
            if (File.Exists(sPath))
                versionInfo = FileVersionInfo.GetVersionInfo(sPath);

            return versionInfo;
        }

        public string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length <= 0)
                    return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                return titleAttribute.Title != "" ? titleAttribute.Title : Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel_www_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://codificare.net/tools/post-edit-compare/");
        }
    }
}
