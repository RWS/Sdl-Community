using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.CleanUpTasks.Dialogs;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;

namespace Sdl.Community.CleanUpTasks
{
	public class ConversionSettingsPresenter : IConversionsSettingsPresenter
	{
		private readonly IConversionsSettingsControl _control;
		private readonly Dictionary<string, bool> _convFiles = new Dictionary<string, bool>();
		private readonly IFileDialog _dialog;
		private bool _isDisabled;

		public ConversionSettingsPresenter(IConversionsSettingsControl control, IFileDialog dialog)
		{
			_control = control;
			_dialog = dialog;

			control.SetPresenter(this);
			UpdateButtons();
		}

		public void AddFile()
		{
			var files = _dialog.GetFile(_control.Settings.LastFileDirectory);

			foreach (var file in files)
			{
				if (File.Exists(file))
				{
					XmlUtilities.Deserialize(file);
					AddFileInternal(file);
				}
			}
			UpdateLastFileDirectory();
		}

		public void DownClick()
		{
			if (_control.FileList.Items.Count > 1)
			{
				if (_control.FileList.SelectedIndex > -1 &&
				    _control.FileList.SelectedIndex < _control.FileList.Items.Count - 1)
				{
					int selectedIndex = _control.FileList.SelectedIndex;
					var checkState = _control.FileList.GetItemCheckState(selectedIndex);
					var convFile = _control.FileList.SelectedItem as ConversionFile;

					_control.FileList.Items.RemoveAt(selectedIndex);
					_control.FileList.Items.Insert(selectedIndex + 1, convFile);

					_control.FileList.SelectedIndex = selectedIndex + 1;
					_control.FileList.SetItemCheckState(_control.FileList.SelectedIndex, checkState);
				}
			}
		}

		public void EditFile(IConversionFileView view)
		{
			var curItem = GetCurItem();
			if (curItem != null)
			{
				// Set the save path before opening the file!
				view.SavedFilePath = curItem.FullPath;
				DialogResult dr = ShowFileViewDialog(view);
				if (dr == DialogResult.OK)
				{
				}
			}
		}

		public void GenerateFile(IConversionFileView view)
		{
			DialogResult dr = ShowFileViewDialog(view);
			if (dr == DialogResult.OK)
			{
				var savePath = view.SavedFilePath;
				AddFileInternal(savePath);
			}
		}

		public void Initialize()
		{
			var fileList = _control.FileList.Items;

			foreach (var pair in _control.Settings.ConversionFiles)
			{
				var convFile = new ConversionFile { FullPath = pair.Key, FileName = Path.GetFileName(pair.Key) };
				fileList.Add(convFile, pair.Value);
			}

			UpdateButtons();
		}

		public void RemoveFile()
		{
			// Multi-selection is not supported on CheckedListBox
			var curItem = GetCurItem();
			if (curItem != null)
			{
				_control.FileList.Items.Remove(curItem);
				_convFiles.Remove(curItem.FullPath);
				UpdateButtons();
			}

			_control.Settings.ConversionFiles = _convFiles;
		}

		public void SaveSettings()
		{
			var dict = new Dictionary<string, bool>();

			var count = _control.FileList.Items.Count;
			var fileList = _control.FileList;

			for (int i = 0; i < count; ++i)
			{
				var checkState = fileList.GetItemCheckState(i);
				var item = fileList.Items[i] as ConversionFile;

				if (checkState == CheckState.Checked)
				{
					if (item != null)
					{
						dict.Add(item.FullPath, true);
					}
				}
				else
				{
					if (item != null)
					{
						dict.Add(item.FullPath, false);
					}
				}
			}

			_control.Settings.ConversionFiles = dict;
		}

		public void UpClick()
		{
			if (_control.FileList.Items.Count > 1)
			{
				if (_control.FileList.SelectedIndex > 0)
				{
					int selectedIndex = _control.FileList.SelectedIndex;
					var checkState = _control.FileList.GetItemCheckState(selectedIndex);
					var convFile = _control.FileList.SelectedItem as ConversionFile;

					_control.FileList.Items.RemoveAt(selectedIndex);
					_control.FileList.Items.Insert(selectedIndex - 1, convFile);

					_control.FileList.SelectedIndex = selectedIndex - 1;

					_control.FileList.SetItemCheckState(_control.FileList.SelectedIndex, checkState);
				}
			}
		}

		private static DialogResult ShowFileViewDialog(IConversionFileView view)
		{
			view.InitializeUI();
			return view.ShowDialog();
		}

		private void AddFileInternal(string file)
		{
			var convFile = new ConversionFile { FullPath = file, FileName = Path.GetFileName(file) };

			var fileList = _control.FileList.Items;

			if (_control.FileList.FindStringExact(convFile.FileName) == ListBox.NoMatches)
			{
				fileList.Add(convFile, true);
				_convFiles.Add(file, true);
				_control.Settings.ConversionFiles = _convFiles;

				UpdateButtons();
			}
		}

		private ConversionFile GetCurItem()
		{
			return _control.FileList.SelectedItem as ConversionFile;
		}

		private void UpdateButtons()
		{
			if (_control.FileList.Items.Count == 0)
			{
				_control.Edit.Enabled = false;
				_control.Remove.Enabled = false;
				_control.Up.Enabled = false;
				_control.Down.Enabled = false;
				_isDisabled = true;
			}
			else if (_isDisabled)
			{
				_control.Edit.Enabled = true;
				_control.Remove.Enabled = true;
				_control.Up.Enabled = true;
				_control.Down.Enabled = true;
				_isDisabled = false;
			}
		}

		private void UpdateLastFileDirectory()
		{
			if (_convFiles.Count > 0)
			{
				var newDirectory = Path.GetDirectoryName(_convFiles.Last().Key);

				if (!string.IsNullOrEmpty(newDirectory))
				{
					// http://stackoverflow.com/a/1756776/906773
					if (string.Compare(
						    Path.GetFullPath(newDirectory).TrimEnd(Path.DirectorySeparatorChar),
						    Path.GetFullPath(_control.Settings.LastFileDirectory).TrimEnd(Path.DirectorySeparatorChar),
						    StringComparison.OrdinalIgnoreCase) != 0)
					{
						_control.Settings.LastFileDirectory = newDirectory;
					}
				}
			}
		}
	}
}