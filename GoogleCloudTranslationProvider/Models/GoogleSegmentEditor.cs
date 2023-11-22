using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using NLog;

namespace GoogleCloudTranslationProvider.Models
{
	public class GoogleSegmentEditor
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _fileName;

		private EditCollection _editCollection;
		private DateTime _lastVersion;

		public GoogleSegmentEditor(string editCollectionFilename)
		{
			_fileName = editCollectionFilename;
			_lastVersion = File.GetLastWriteTime(_fileName);
			LoadCollection();
		}

		public string EditText(string text)
		{
			var result = text;
			var currentVersion = File.GetLastWriteTime(_fileName);
			if (currentVersion > _lastVersion)
			{
				_lastVersion = currentVersion;
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
					result = new Regex(find).Replace(result, replace);
				}
			}

			return result;
		}

		private void LoadCollection()
		{
			try
			{
				var reader = new StreamReader(_fileName);
				var serializer = new XmlSerializer(typeof(EditCollection));
				_editCollection = (EditCollection)serializer.Deserialize(reader);
			}
			catch (InvalidOperationException e)
			{ //invalid operation is what happens when the xml can't be parsed into the objects correctly
				HandleException(e);
			}
			catch (Exception e)
			{
				HandleException(e);
			}
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

		private IntPtr GetHandle()
		{
			var processName = AppDomain.CurrentDomain.FriendlyName;
			processName = processName.Substring(0, processName.LastIndexOf('.'));
			var process = Process.GetProcessesByName(processName);
			return process[0].MainWindowHandle;
		}
	}
}