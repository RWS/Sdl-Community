using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Annotations;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class TranslationMemoriesPenaltiesViewModel : INotifyPropertyChanged
	{
		#region Private Fields
		private PackageModel _packageModel;
		private ObservableCollection<TranslationMemoriesPenaltiesModel> _translationMemoriesPenaltiesModelList;
		private string _translationMemoryName;
		private string _translationMemoryPath;
		private int _tmPenalty;
		private ICommand _okCommand;
		private ICommand _cancelCommand;

		#endregion

		#region Constructors
		public TranslationMemoriesPenaltiesViewModel(PackageModel packageModel)
		{
			_packageModel = packageModel;
			LoadTranslationMemories();
		}
		#endregion

		#region Commands
		#endregion

		#region Public Properties
		public string Error { get; }

		public ObservableCollection<TranslationMemoriesPenaltiesModel> TranslationMemoriesPenaltiesModelList
		{
			get
			{
				return _translationMemoriesPenaltiesModelList;
			}
			set
			{
				if (Equals(value, _translationMemoriesPenaltiesModelList))
				{
					return;
				}
				_translationMemoriesPenaltiesModelList = value;
				OnPropertyChanged();
			}
		}

		public string TranslationMemoryName
		{
			get
			{
				return _translationMemoryName;
			}
			set
			{
				if (Equals(value, _translationMemoryName))
				{
					return;
				}
				_translationMemoryName = value;
				OnPropertyChanged();
			}
		}

		public string TranslationMemoryPath
		{
			get
			{
				return _translationMemoryPath;
			}
			set
			{
				if (Equals(value, _translationMemoryPath))

				{
					return;
				}
				_translationMemoryPath = value;
				OnPropertyChanged();
			}
		}

		public int TMPenalty
		{
			get
			{
				return _tmPenalty;
			}
			set
			{
				if (Equals(value, _tmPenalty))
				{
					return;
				}
				_tmPenalty = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Public Methods
		#endregion

		#region Private Methods
		/// <summary>
		/// Load translation memories
		/// </summary>
		private void LoadTranslationMemories()
		{
			TranslationMemoriesPenaltiesModelList = new ObservableCollection<TranslationMemoriesPenaltiesModel>();
			if (_packageModel != null)
			{
				foreach (var langPair in _packageModel.LanguagePairs)
				{
					if (langPair.HasTm)
					{
						foreach (var filePath in langPair.SourceFile)
						{
							TranslationMemoryName = Path.GetFileName(filePath);
							TranslationMemoryPath = filePath;

							var translationMemoriesPenaltiesModel = new TranslationMemoriesPenaltiesModel()
							{
								TranslationMemoryName = TranslationMemoryName,
								TranslationMemoryPath = TranslationMemoryPath,
								TMPenalty = TMPenalty
							};
							
							TranslationMemoriesPenaltiesModelList.Add(translationMemoriesPenaltiesModel);
						}
					}
				}
			}
		}
		#endregion

		#region Virtual Methods
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		#region Commands
		public ICommand OkCommand
		{
			get { return _okCommand ?? (_okCommand = new CommandHandler(OkAction, true)); }
		}

		public ICommand CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new CommandHandler(CancelAction, true)); }
		}
		#endregion

		#region Actions
		private void OkAction()
		{
			_packageModel.TMPenalties = new System.Collections.Generic.Dictionary<string, int>();

			foreach (var tm in TranslationMemoriesPenaltiesModelList)
			{
				_packageModel.TMPenalties.Add(tm.TranslationMemoryPath, tm.TMPenalty);
			}
			CloseWindow();
		}

		private void CancelAction()
		{
			CloseWindow();
		}

		private void CloseWindow()
		{
			var windows = Application.Current.Windows;
			foreach (Window window in windows)
			{
				if (window.Title.Equals("Translation Memories Penalties"))
				{
					window.Close();
				}
			}
		}
		#endregion

		#region Events
		public event PropertyChangedEventHandler PropertyChanged;		
		#endregion
	}
}