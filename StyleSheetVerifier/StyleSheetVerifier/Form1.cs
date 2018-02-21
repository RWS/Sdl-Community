using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Xsl;

namespace Sdl.Community.StyleSheetVerifier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowseSheet_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Stylesheet|*.xslt;*.xsl";
            ofd.FilterIndex = 2;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtSheet.Text = ofd.FileName;

            }

        }

        private void btnBrowseXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML |*.xml";
            ofd.FilterIndex = 2;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtXml.Text = ofd.FileName;

            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtXml.Text)) return;
            if (string.IsNullOrEmpty(txtSheet.Text)) return;
            try
            {
                var path = Path.GetDirectoryName(txtXml.Text);
                var htmlPreviewFile = $@"{path}\preview.html";
                if (File.Exists(htmlPreviewFile))
                {
                    File.Delete(htmlPreviewFile);
                }
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(txtSheet.Text);


                transformer.Transform(txtXml.Text, htmlPreviewFile);

                webBrowser1.Navigate(htmlPreviewFile);
            }catch(Exception ex)
            {
                var message = GetErrorMessage(ex);
                

                MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetErrorMessage(Exception ex)
        {
            var result = new StringBuilder();
            result.AppendLine(ex.Message);
            if(ex.InnerException != null)
            {
                result.AppendLine("----------");
                result.AppendLine(GetErrorMessage(ex.InnerException));
            }

            return result.ToString();
        }
    }
}
