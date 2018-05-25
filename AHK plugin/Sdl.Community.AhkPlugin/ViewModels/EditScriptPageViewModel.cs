using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.AhkPlugin.Helpers;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.DataBase;

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
		private static readonly MasterScriptDb MasterScriptDb = new MasterScriptDb();
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

		private async void SaveScript()
		{

			var masterScript = await MasterScriptDb.GetMasterScript();
			var scriptToBeUpdated = masterScript.Scripts.FirstOrDefault(s => s.ScriptId.Equals(_script.ScriptId));
			if (scriptToBeUpdated != null)
			{
				scriptToBeUpdated.Active = !IsDisabled;
				scriptToBeUpdated.Name = ScriptName;
				scriptToBeUpdated.Text = ScriptContent;
				scriptToBeUpdated.Description = ScriptDescription;
				scriptToBeUpdated.ScriptStateAction = !IsDisabled ? "Disable" : "Enable";
				scriptToBeUpdated.RowColor = !IsDisabled ? "Black" : "DarkGray";
				await MasterScriptDb.UpdateScript(masterScript);
				//write masterscript on the disk
				ProcessScript.ExportScript(Path.Combine(masterScript.Location, masterScript.Name), masterScript.Scripts);
				var result =MessageBox.Show("Script edited successfully", string.Empty,
					MessageBoxButton.OK,MessageBoxImage.Information);
				if (result == MessageBoxResult.OK)
				{
					_mainWindowViewModel.LoadScriptsPage();
				}
			}
		}
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
		private void BackToScriptsList()
		{
			_mainWindowViewModel.LoadScriptsPage();
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
	}
}
