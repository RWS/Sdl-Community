using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using NLog;

namespace Sdl.Community.StyleSheetVerifier
{
	public partial class StyleSheetVerifierForm : Form
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public StyleSheetVerifierForm()
		{
			InitializeComponent();
		}

		private void btnBrowseSheet_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
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
			var ofd = new OpenFileDialog();
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

				TransformFile(htmlPreviewFile);
				webBrowser1.Navigate(htmlPreviewFile);
			}
			catch (Exception ex)
			{
				var message = GetErrorMessage(ex);
				MessageBox.Show(this, message, PluginResources.ErrorName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
			}
		}

		private void TransformFile(string htmlPreviewFile)
		{
			var transformer = new XslCompiledTransform();
			var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
			var reader = XmlReader.Create(txtSheet.Text, settings);

			transformer.Load(reader);

			var xmlReader = XmlReader.Create(txtXml.Text, settings);
			var writerSettings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Auto };
			var xmlWriter = XmlWriter.Create(htmlPreviewFile, writerSettings);

			transformer.Transform(xmlReader, xmlWriter);
		}

		private string GetErrorMessage(Exception ex)
		{
			var result = new StringBuilder();
			result.AppendLine(ex.Message);
			if (ex.InnerException != null)
			{
				result.AppendLine("----------");
				result.AppendLine(GetErrorMessage(ex.InnerException));
			}

			return result.ToString();
		}
	}
}