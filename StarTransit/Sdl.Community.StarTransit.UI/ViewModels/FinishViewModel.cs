using System;
using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class FinishViewModel : BaseViewModel
	{
		private bool _active;
		private string _location;
		private string _txtName;
		private string _txtDescription;
		private string _sourceLanguage;
		private List<string> _targetLanguage;
		private string _templateName;
		private string _customer;
		private string _dueDate;
		private string _tmName;
		private string _tmPath;
		private readonly PackageDetailsViewModel _packageDetailsViewModel;
		private readonly TranslationMemoriesViewModel _translationMemoriesViewModel;
		private PackageModel _package;
		
		public FinishViewModel(TranslationMemoriesViewModel translationMemoriesViewModel, PackageDetailsViewModel packageDetailsViewModel)
		{
			_translationMemoriesViewModel = translationMemoriesViewModel;
			_packageDetailsViewModel = packageDetailsViewModel;
			_package = _translationMemoriesViewModel.GetPackageModel();
			Refresh();
		}

		public void Refresh()
		{
			_package = _translationMemoriesViewModel.GetPackageModel();
			var targetLanguage = new List<string>();
			Name = _package.Name;
			Location = _package.Location;
			if (_package.HasDueDate)
			{
				DueDate = Helpers.TimeHelper.SetDateTime(_package.DueDate,
						_packageDetailsViewModel.SelectedHour, _packageDetailsViewModel.SelectedMinute,
						_packageDetailsViewModel.SelectedMoment).ToString();
			}

			if (_package.ProjectTemplate != null)
			{
				TemplateName = _package.ProjectTemplate.Name;
			}
			if (_package.Customer != null)
			{
				Customer = _package.Customer.Name;
			}

			Description = _packageDetailsViewModel.Description;
			SourceLanguage = _packageDetailsViewModel.SourceLanguage;
			
			foreach (var pair in _package.LanguagePairs)
			{
				var pairMessage = $"{pair.TargetLanguage.DisplayName}, translation units will be imported {Environment.NewLine} into Translation Memory named: {pair.TmName}";
				var tmPair = string.IsNullOrEmpty(pair.TmName) ? $"No TM selected for {pair.PairName} pair." : pairMessage;
				targetLanguage.Add(tmPair);
			}
			TargetLanguage = targetLanguage;
		}

		public string Name
		{
			get => _packageDetailsViewModel.Name;
			set
			{
				if (Equals(value, _txtName))
				{
					return;
				}
				_txtName = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Description
		{
			get => _txtDescription;
			set
			{
				if (Equals(value, _txtDescription))
				{
					return;
				}
				_txtDescription = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		public string SourceLanguage
		{
			get => _packageDetailsViewModel?.SourceLanguage;
			set
			{
				if (Equals(value, _sourceLanguage))
				{
					return;

				}
				_sourceLanguage = value;
				OnPropertyChanged(nameof(SourceLanguage));
			}
		}

		public List<string> TargetLanguage
		{
			get => _targetLanguage ?? (_targetLanguage = new List<string>());
			set
			{
				_targetLanguage = value;
				OnPropertyChanged(nameof(TargetLanguage));
			}
		}

		public string Location
		{
			get => _packageDetailsViewModel?.TextLocation;
			set
			{
				if (Equals(value, _location))
				{
					return;
				}
				_location = value;
				OnPropertyChanged(nameof(Location));
			}
		}

		public bool Active
		{
			get => _active;
			set
			{
				if (Equals(value, _active))
				{
					return;
				}
				_active = value;
				OnPropertyChanged(nameof(Active));
			}
		}

		public string TemplateName
		{
			get => _templateName;
			set
			{
				if (Equals(value, _templateName))
				{
					return;
				}
				_templateName = value;
				OnPropertyChanged(nameof(TemplateName));
			}
		}

		public string Customer
		{
			get => _customer;
			set
			{
				if (Equals(value, _customer))
				{
					return;
				}
				_customer = value;
				OnPropertyChanged(nameof(Customer));
			}
		}

		public string DueDate
		{
			get => _dueDate;
			set
			{
				if (Equals(value, _dueDate))
				{
					return;
				}
				_dueDate = value;
				OnPropertyChanged(nameof(DueDate));
			}
		}

		public string TmName
		{
			get => _tmName;
			set
			{
				if (Equals(value, _tmName)) { return; }
				_tmName = value;
				OnPropertyChanged(nameof(TmName));
			}
		}

		public string TmPath
		{
			get => _tmPath;
			set
			{
				if (Equals(_tmPath, value)) { return; }
				_tmPath = value;
				OnPropertyChanged(nameof(TmPath));
			}
		}
	}
}