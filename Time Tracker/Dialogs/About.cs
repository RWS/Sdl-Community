using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            Text = string.Format("About {0}", AssemblyProduct) + @" for SDL Trados Studio 2017";
            labelProductNameAndVersion.Text = AssemblyProduct + " - " + String.Format("Version {0}", AssemblyVersion);
            //this.linkLabelpostEditCompareNameAndVersion.Text = "Post-Edit Compare " + " - " + String.Format("Version {0}", getPostEditCompareProductVersion());
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = CopyRight;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }


        private const string CopyRight = "License agreement\r\n" + "\r\n"

                                         //+ "---------------------\r\n"
                                         + "DISCLAIMER OF WARRANTIES: YOU AGREE THAT THE DEVELOPER OF THE SOFTWARE HAS MADE NO EXPRESS WARRANTIES TO YOU REGARDING THE SOFTWARE AND THAT THE SOFTWARE IS BEING PROVIDED TO YOU \"AS IS\" WITHOUT WARRANTY OF ANY KIND.  THE DEVELOPER DISCLAIMS ALL WARRANTIES WITH REGARD TO THE SOFTWARE, EXPRESS OR IMPLIED, INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, MERCHANTABLE QUALITY, OR NONINFRINGEMENT OF THIRD-PARTY RIGHTS.\r\n\r\n" + "LIMIT OF LIABILITY: IN NO EVENT WILL THE DEVELOPER BE LIABLE TO YOU FOR ANY LOSS OF USE, INTERRUPTION OF BUSINESS, OR ANY DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES OF ANY KIND (INCLUDING LOST PROFITS) REGARDLESS OF THE FORM OF ACTION WHETHER IN CONTRACT, TORT (INCLUDING NEGLIGENCE), STRICT PRODUCT LIABILITY OR OTHERWISE, EVEN IF THE DEVELOPER HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\r\n\r\n" + "Restrictions:\r\n"
                                         //+ "-------------\r\n"
                                         + "1. You may redistribute this software, but the distribution package must not be modified in ANY WAY. All redistributions of this package must retain all occurrences of the above copyright notice and web links that are currently in place (for example, in the about boxes or documentation).\r\n\r\n" + "2. You must not emulate, clone, rent, lease, sell, modify, decompile, disassemble, otherwise reverse engineer, or transfer any version of the software, or any subset of it. Any such unauthorized use shall result in immediate and automatic termination of this license and may result in criminal and/or civil prosecution.\r\n\r\n" + "\r\n";

        #region Assembly Attribute Accessors

        public string GetPostEditCompareProductVersion()
        {
            var productVersion = string.Empty;

            var fileVersionInfo = PostEditCompareVersionInfo(); 
            if (fileVersionInfo!=null)
                return fileVersionInfo.ProductVersion;

            return productVersion;
        }

        public FileVersionInfo PostEditCompareVersionInfo()
        {
            FileVersionInfo versionInfo =null;
            
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
    }
}
