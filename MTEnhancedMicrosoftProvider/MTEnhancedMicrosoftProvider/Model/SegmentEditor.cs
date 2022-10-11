using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using NLog;

namespace MTEnhancedMicrosoftProvider.Model
{
	public class SegmentEditor
	{
		private readonly string _filename;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private EditCollection _edcoll;
		private DateTime _lastversion;


		public SegmentEditor(string editCollectionFilename)
		{
			_filename = editCollectionFilename;
			_lastversion = File.GetLastWriteTime(_filename);
			LoadCollection();
		}


		public void LoadCollection()
		{
			try
			{
				using (var reader = new StreamReader(_filename))
				{
					var serializer = new XmlSerializer(typeof(EditCollection));
					_edcoll = (EditCollection)serializer.Deserialize(reader);
				}
			}
			catch (InvalidOperationException ex) //invalid operation is what happens when the xml can't be parsed into the objects correctly
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");

				var caption = PluginResources.EditSettingsErrorCaption;
				var message = string.Format(PluginResources.EditSettingsXmlErrorMessage, Path.GetFileName(_filename));
				MessageBox.Show(new WindowWrapper(GetHandle()), message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				throw new Exception(message);
			}
			catch (Exception exp) //catch-all for any other kind of error...passes up a general message with the error description
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {exp.Message}\n {exp.StackTrace}");

				var caption = PluginResources.EditSettingsErrorCaption;
				var message = PluginResources.EditSettingsGenericErrorMessage + " " + exp.Message;
				MessageBox.Show(new WindowWrapper(GetHandle()), message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				throw new Exception(message);
			}
		}

		public string EditText(string text)
		{
			var result = text;
			var currentversion = File.GetLastWriteTime(_filename);
			if (currentversion > _lastversion)
			{
				_lastversion = currentversion;
				LoadCollection();
			}

			if (_edcoll.Items.Count == 0)
			{
				return text;
			}

			for (var i = 0; i < _edcoll.Items.Count; i++)
			{
				if (_edcoll.Items[i].Enabled)
				{
					var find = _edcoll.Items[i].FindText;
					var replace = _edcoll.Items[i].ReplaceText;

					if (_edcoll.Items[i].Type == EditItem.EditItemType.PlainText)
					{
						result = result.Replace(find, replace);
					}
					else if (_edcoll.Items[i].Type == EditItem.EditItemType.RegularExpression)
					{
						var reg = new Regex(find);
						result = reg.Replace(result, replace);
					}
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
	}
}