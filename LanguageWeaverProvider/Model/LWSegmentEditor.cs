using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using LanguageWeaverProvider.Extensions;

namespace LanguageWeaverProvider.Model
{
	public class LWSegmentEditor
	{
		private readonly string _fileName;

		private EditCollection _editCollection;
		private DateTime _lastVersion;

		public LWSegmentEditor(string editCollectionFilename)
		{
			_fileName = editCollectionFilename;
			_lastVersion = File.GetLastWriteTime(_fileName);
			LoadCollection();
		}

		public bool IsValid { get; private set; }

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
				IsValid = true;
			}
			catch (InvalidOperationException e)
			{ //invalid operation is what happens when the xml can't be parsed into the objects correctly
				e.ShowDialog(string.Empty, e.Message, true);
				IsValid = false;
			}
			catch (Exception e)
			{
				e.ShowDialog(string.Empty, e.Message, true);
				IsValid = false;
			}
		}
	}
}