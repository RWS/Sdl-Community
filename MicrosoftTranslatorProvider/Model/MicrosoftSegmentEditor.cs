using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using MicrosoftTranslatorProvider.Studio;
using NLog;

namespace MicrosoftTranslatorProvider.Model
{
	public class MicrosoftSegmentEditor
	{
		private readonly string _fileName;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private EditCollection _editCollection;
		private DateTime _lastVersion;

		public MicrosoftSegmentEditor(string editCollectionFilename)
		{
			_fileName = editCollectionFilename;
			_lastVersion = File.GetLastWriteTime(_fileName);
			LoadCollection();
		}

		public void LoadCollection()
		{
			try
			{
				var reader = new StreamReader(_fileName);
				var serializer = new XmlSerializer(typeof(EditCollection));
				_editCollection = serializer.Deserialize(reader) as EditCollection;
			}
			catch (InvalidOperationException e)
			{   //invalid operation is what happens when the xml can't be parsed into the objects correctly
				HandleException(e);
			}
			catch (Exception e)
			{
				HandleException(e);
			}
		}

		public string EditText(string text)
		{
			var result = text;
			var currentversion = File.GetLastWriteTime(_fileName);
			if (currentversion > _lastVersion)
			{
				_lastVersion = currentversion;
				LoadCollection();
			}

			if (_editCollection.Items.Count == 0)
			{
				return text;
			}

			for (var i = 0; i < _editCollection.Items.Count; i++)
			{
				if (!_editCollection.Items[i].Enabled)
				{
					continue;
				}

				var find = _editCollection.Items[i].FindText;
				var replace = _editCollection.Items[i].ReplaceText;
				if (_editCollection.Items[i].Type == EditItemType.PlainText)
				{
					result = result.Replace(find, replace);
				}
				else if (_editCollection.Items[i].Type == EditItemType.RegularExpression)
				{
					var reg = new Regex(find);
					result = reg.Replace(result, replace);
				}
			}

			return result;
		}

		private IntPtr GetHandle()
		{
			var strFriendlyName = AppDomain.CurrentDomain.FriendlyName;
			var pro = Process.GetProcessesByName(strFriendlyName.Substring(0, strFriendlyName.LastIndexOf('.')));
			return pro[0].MainWindowHandle;
		}

		private void HandleException(object exObj)
		{

			var exception = exObj as Exception;
			_logger.Error($"{MethodBase.GetCurrentMethod().Name} {exception.Message}\n {exception.StackTrace}");

			var owner = new WindowWrapper(GetHandle());
			var caption = PluginResources.EditSettingsErrorCaption;
			var invalidOperationExceptionMessage = string.Format(PluginResources.EditSettingsXmlErrorMessage, Path.GetFileName(_fileName));
			var generalExceptionMessage = PluginResources.EditSettingsGenericErrorMessage + " " + exception.Message;
			var message = exObj is InvalidOperationException ? invalidOperationExceptionMessage
															 : generalExceptionMessage;
			MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			throw new Exception(message);
		}
	}
}