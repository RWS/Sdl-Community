using System;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Community.XliffToLegacyConverter
{
    partial class About : Form
    {


        private string copyRight = "License agreement\r\n"
                                    +"====================================\r\n\r\n"
                              
                                    + "Liability disclaimer:\r\n"
                                    + "---------------------\r\n"
                                    + "This software is provided \"as-is,\" without any express or implied warranty. In no event shall the author be held liable for any damages arising from the use of this software.\r\n\r\n"

                                    + "Permissions:\r\n"
                                    + "------------\r\n"
                                    + "1. This software is distributed as a FREEWARE. Permission is granted to anyone to use this software in personal, educational or corporate environment.\r\n\r\n"


                                    + "Restrictions:\r\n"
                                    + "-------------\r\n"
                                    + "1. You may redistribute this software, but the distribution package must not be modified in ANY WAY. All redistributions of this package must retain all occurrences of the above copyright notice and web links that are currently in place (for example, in the about boxes or documentation).\r\n\r\n"
                                    + "2. You must not emulate, clone, rent, lease, sell, modify, decompile, disassemble, otherwise reverse engineer, or transfer any version of the software, or any subset of it. Any such unauthorized use shall result in immediate and automatic termination of this license and may result in criminal and/or civil prosecution.\r\n\r\n"
                            
                                    + "\r\n";


        public About()
        {
            InitializeComponent();
            Text = String.Format("About {0}", AssemblyProduct + " for SDL Trados Studio 2015");
            labelProductName.Text = AssemblyProduct + @" for SDL Trados Studio 2015";
            labelVersion.Text = String.Format("Version {0}", AssemblyVersion );
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AssemblyDescription + "\r\n\r\n\r\n" + copyRight;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
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
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void AboutBox1_Load(object sender, EventArgs e)
        {

        }
    }
}
