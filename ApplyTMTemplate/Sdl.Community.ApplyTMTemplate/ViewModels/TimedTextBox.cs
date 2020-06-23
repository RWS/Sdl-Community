﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.ApplyTMTemplate.Commands;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class TimedTextBox : ModelBase
	{
		private bool _isOpen;
		private string _path;
		private ICommand _menuCommand;

		//TODO: This class has to go; it is not at all part of any pattern;
		//Unfortunately this class is not easy to give up, since it is the VM of TimedTextBox,
		//which holds all the menu logic for the template
		public TimedTextBox()
		{
			Timer.Tick += StartValidation;
			PropertyChanged += StartValidationTimer;
		}

		public event EventHandler ShouldStartValidation;
		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public bool IsOpen
		{
			get => _isOpen;
			set
			{
				_isOpen = value;
				OnPropertyChanged(nameof(IsOpen));
			}
		}

		//property needed for dynamic accessing of command it holds since the property of the command itself is rerouted
		public ICommand BrowseField { get; set; } 
		public ICommand ImportFromExcelField { get; set; }
		public ICommand ExportField { get; set; }

		//all commands rerouted for the purpose of adding common functionality(close menu) to all of them
		public ICommand BrowseCommand
		{
			get => new RelayCommand(s => CommandHandlerMethod(s, nameof(BrowseField)));
			set => BrowseField = value;
		}
		public ICommand ImportCommand // This command imports from Excel or from TMs depending on the CommandParameter sent by the control which triggers it
		{
			get => new RelayCommand(s => CommandHandlerMethod(s, nameof(ImportFromExcelField)));
			set => ImportFromExcelField = value;
		}

		public ICommand ExportCommand
		{
			get => new RelayCommand(s => CommandHandlerMethod(s, nameof(ExportField)));
			set => ExportField = value;
		}

		public ICommand MenuCommand => _menuCommand ?? (_menuCommand = new CommandHandler(ToggleMenu, true));

		public Timer Timer { get; } = new Timer { Interval = 500 };

		private void StartValidationTimer(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Path")
			{
				Timer.Stop();
				Timer.Start();
			}
		}

		private void StartValidation(object sender, EventArgs e)
		{
			Timer.Stop();
			ShouldStartValidation?.Invoke(sender, e);
		}

		public void ToggleMenu()
		{
			IsOpen = !IsOpen;
		}

		private void CommandHandlerMethod(object parameter, string commandName)
		{
			ToggleMenu();

			var commandProperty = GetType().GetProperty(commandName);
			var command = commandProperty?.GetMethod.Invoke(this, null);
			var executeMethod = command?.GetType().GetMethod("Execute");

			if (command != null)
			{
				executeMethod?.Invoke(command, new object[] {parameter});
			}
		}
	}
}