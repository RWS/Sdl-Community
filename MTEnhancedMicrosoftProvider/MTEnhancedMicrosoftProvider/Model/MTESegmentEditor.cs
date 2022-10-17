using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using MTEnhancedMicrosoftProvider.Studio;
using NLog;

namespace MTEnhancedMicrosoftProvider.Model
{
	public class MTESegmentEditor
	{
		private readonly string _fileName;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private DateTime _lastversion;
		private EditCollection _collection;

		public MTESegmentEditor(string editCollectionFilename)
		{
			_fileName = editCollectionFilename;
			_lastversion = File.GetLastWriteTime(_fileName);
			LoadCollection();
		}

		public void LoadCollection()
		{
			try
			{
				var reader = new StreamReader(_fileName);
				var serializer = new XmlSerializer(typeof(EditCollection));
				_collection = serializer.Deserialize(reader) as EditCollection;
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
			if (currentversion > _lastversion)
			{
				_lastversion = currentversion;
				LoadCollection();
			}

			if (_collection.Items.Count == 0)
			{
				return text;
			}

			for (var i = 0; i < _collection.Items.Count; i++)
			{
				if (!_collection.Items[i].Enabled)
				{
					continue;
				}

				var find = _collection.Items[i].FindText;
				var replace = _collection.Items[i].ReplaceText;
				if (_collection.Items[i].Type == EditItem.EditItemType.PlainText)
				{
					result = result.Replace(find, replace);
				}
				else if (_collection.Items[i].Type == EditItem.EditItemType.RegularExpression)
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

			var caption = PluginResources.EditSettingsErrorCaption;
			var message = exObj is InvalidOperationException ? string.Format(PluginResources.EditSettingsXmlErrorMessage, Path.GetFileName(_fileName))
															 : PluginResources.EditSettingsGenericErrorMessage + " " + exception.Message;
			var owner = new WindowWrapper(GetHandle());
			MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			throw new Exception(message);
		}
	}
}