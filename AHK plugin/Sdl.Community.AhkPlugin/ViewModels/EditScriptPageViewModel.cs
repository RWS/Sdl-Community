﻿using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.ViewModels
{
	public class EditScriptPageViewModel:ViewModelBase
	{
		private static MainWindowViewModel _mainWindowViewModel;
		private ICommand _backCommand;
		private ICommand _saveCommand;
		private string _scriptName;
		private string _scriptDescription;
		private string _scriptContent;
		private bool _isDisabled;
		private static Script _script;
		private static readonly DbContext DbContext = new DbContext();
		public static readonly Log Log = Log.Instance;

		public EditScriptPageViewModel()
		{
			ScriptContent = _script.Text;
			ScriptDescription = _script.Description;
			ScriptName = _script.Name;
			IsDisabled = !_script.Active;
		}

		public EditScriptPageViewModel(Script script,MainWindowViewModel mainWindowViewModel)
		{
			_mainWindowViewModel = mainWindowViewModel;
			_script = script;

		}
		public ICommand BackCommand => _backCommand ?? (_backCommand = new CommandHandler(BackToScriptsList, true));
		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveScript, true));

		public string ScriptDescription
		{
			get => _scriptDescription;

			set
			{
				if (Equals(value, _scriptDescription))
				{
					return;
				}
				_scriptDescription = value;
				OnPropertyChanged(nameof(ScriptDescription));
			}
		}

		public string ScriptContent
		{
			get => _scriptContent;

			set
			{
				if (Equals(value, _scriptContent))
				{
					return;
				}
				_scriptContent = value;
				OnPropertyChanged(nameof(ScriptContent));
			}
		}

		public bool IsDisabled
		{
			get => _isDisabled;

			set
			{
				if (Equals(value, _isDisabled))
				{
					return;
				}
				_isDisabled = value;
				OnPropertyChanged(nameof(IsDisabled));
			}
		}

		public string ScriptName
		{
			get => _scriptName;

			set
			{
				if (Equals(value, _scriptName))
				{
					return;
				}
				_scriptName = value;
				OnPropertyChanged(nameof(ScriptName));
			}
		}

		private void BackToScriptsList()
		{
			_mainWindowViewModel.LoadScriptsPage();
		}

		private async void SaveScript()
		{
			try
			{
				var masterScript = await DbContext.GetMasterScript();
				var scriptToBeUpdated = masterScript.Scripts.FirstOrDefault(s => s.ScriptId.Equals(_script.ScriptId));
				if (scriptToBeUpdated != null)
				{
					scriptToBeUpdated.Active = !IsDisabled;
					scriptToBeUpdated.Name = ScriptName;
					scriptToBeUpdated.Text = ScriptContent;
					scriptToBeUpdated.Description = ScriptDescription;
					if (string.IsNullOrWhiteSpace(scriptToBeUpdated.FileName))
					{
						scriptToBeUpdated.FileName = "AhkMasterScript";
					}
					scriptToBeUpdated.ScriptStateAction = !IsDisabled ? "Disable" : "Enable";
					scriptToBeUpdated.RowColor = !IsDisabled ? "Black" : "DarkGray";
					await DbContext.UpdateScript(masterScript);
					//write masterscript on the disk
					ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);
					var result = MessageBox.Show("Script edited successfully", string.Empty,
						MessageBoxButton.OK, MessageBoxImage.Information);
					if (result == MessageBoxResult.OK)
					{
						_mainWindowViewModel.LoadScriptsPage();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.SaveScript}: {Constants.Message}: {ex.Message}\n " +
					$"{Constants.StackTrace}: {ex.StackTrace}\n {Constants.InnerException}: {ex.InnerException}");
			}
		}
	}
}