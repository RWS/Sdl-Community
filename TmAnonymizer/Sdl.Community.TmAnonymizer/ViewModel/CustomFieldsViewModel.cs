using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Ui;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private ObservableCollection<CustomField> _customFields;
		private readonly BackgroundWorker _backgroundWorker;
		private WaitWindow _waitWindow;


		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_customFields = new ObservableCollection<CustomField>();
			if (_tmsCollection != null)
			{
				PopulateCustomFieldGrid(_tmsCollection, _translationMemoryViewModel);
			}
			_translationMemoryViewModel = translationMemoryViewModel;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;

		}

		

		public ObservableCollection<CustomField> CustomFieldsCollection
		{
			get => _customFields;

			set
			{
				if (Equals(value, _customFields))
				{
					return;
				}
				_customFields = value;
				OnPropertyChanged(nameof(CustomFieldsCollection));

			}
		}


		

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					if (!newTm.IsServerTm)
					{
						CustomFieldsCollection = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(newTm));
					}

					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				_backgroundWorker.RunWorkerAsync(sender);
			}
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_waitWindow != null)
			{
				_waitWindow.Close();
			}
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{

			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			if (tm.IsSelected)
			{
				if (tm.IsServerTm)
				{
					//var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					//var translationProvider = new TranslationProviderServer(uri, false,
					//	_translationMemoryViewModel.Credentials.UserName,
					//	_translationMemoryViewModel.Credentials.Password);
					//var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
					//foreach (var name in names)
					//{
					//	System.Windows.Application.Current.Dispatcher.Invoke(() =>
					//	{
					//		UniqueUserNames.Add(name);
					//	});
					//}
				}
				else
				{
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
					foreach (var field in customFields)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(() =>
						{
							CustomFieldsCollection.Add(field);
						});
					}
				}
			}
			else
			{
				//if (tm.IsServerTm)
				//{
				//	var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
				//	var translationProvider = new TranslationProviderServer(uri, false,
				//		_translationMemoryViewModel.Credentials.UserName,
				//		_translationMemoryViewModel.Credentials.Password);
				//	var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
				//	var newList = UniqueUserNames.ToList();
				//	foreach (var name in names)
				//	{
				//		newList.RemoveAll(n => n.UserName.Equals(name.UserName));
				//	}
				//	UniqueUserNames = new ObservableCollection<User>(newList);
				//}
				//else
				//{
					var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
					var newList = CustomFieldsCollection.ToList();
					foreach (var fields in customFields)
					{
						newList.RemoveAll(n => n.Name.Equals(fields.Name));
					}
					CustomFieldsCollection = new ObservableCollection<CustomField>(newList);
				//}
			}
		}


		private void PopulateCustomFieldGrid(ObservableCollection<TmFile> tmsCollection, TranslationMemoryViewModel translationMemoryViewModel)
		{
			var serverBasedTms = tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
			var fileBasedTms = tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
			//if (serverBasedTms.Any())
			//{
			//	var uri = new Uri(translationMemoryViewModel.Credentials.Url);
			//	var translationProvider = new TranslationProviderServer(uri, false,
			//		translationMemoryViewModel.Credentials.UserName,
			//		translationMemoryViewModel.Credentials.Password);
			//	foreach (var serverTm in serverBasedTms)
			//	{
			//		UniqueUserNames = Helpers.SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
			//	}

			//}

			if (fileBasedTms.Any())
			{
				foreach (var fileTm in fileBasedTms)
				{
					CustomFieldsCollection = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(fileTm));
				}
			}
		}
	}
}



