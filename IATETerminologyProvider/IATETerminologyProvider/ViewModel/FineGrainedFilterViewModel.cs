using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class FineGrainedFilterViewModel : ViewModelBase, ISettingsViewModel
	{
		private List<CollectionModel> _collections = new List<CollectionModel>();
		private SettingsModel _settings;
		private List<InstitutionModel> _institutions = new List<InstitutionModel>();
		private Reliabilities _sourceReliabilities = new Reliabilities();
		private Reliabilities _targetReliabilities = new Reliabilities();
		private Primarities _primarities;

		public FineGrainedFilterViewModel()
		{
			PropertyChanged += FineGrainedFilterViewModel_PropertyChanged;
		}

		public bool AllCollectionsChecked
		{
			get => AreAllCollectionsSelected();
			set
			{
				SwitchAllCollections(value);
				OnPropertyChanged(nameof(AllCollectionsChecked));
			}
		}
		
		public bool AllInstitutionsChecked
		{
			get => AreAllInstitutionsSelected();
			set
			{
				SwitchAllInstitutions(value);
				OnPropertyChanged(nameof(AllInstitutionsChecked));
			}
		}

		private void SwitchAllInstitutions(bool onOff)
		{
			Institutions.ForEach(c => c.IsSelected = onOff);
		}

		public List<CollectionModel> Collections
		{
			get => _collections;
			set
			{
				_collections = value;

				if (_collections.Any())
				{
					foreach (var collection in _collections)
					{
						collection.PropertyChanged -= DataPropertyChanged;
					}
				}

				if (_collections.Any())
				{
					foreach (var collection in _collections)
					{
						collection.PropertyChanged += DataPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(Collections));
			}
		}

		public List<InstitutionModel> Institutions
		{
			get => _institutions;
			set
			{
				_institutions = value;

				if (_institutions.Any())
				{
					foreach (var institution in _institutions)
					{
						institution.PropertyChanged -= DataPropertyChanged;
					}
				}

				if (_institutions.Any())
				{
					foreach (var institution in _institutions)
					{
						institution.PropertyChanged += DataPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(Institutions));
			}
		}

		public SettingsModel Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged(nameof(Settings));
			}
		}

		public Primarities Primarities
		{
			get => _primarities;
			set
			{
				_primarities = value;
				OnPropertyChanged(nameof(Primarities));
			}
		}

		public Reliabilities SourceReliabilities
		{
			get => _sourceReliabilities;
			set
			{
				_sourceReliabilities = value;
				OnPropertyChanged(nameof(SourceReliabilities));
			}
		}

		public Reliabilities TargetReliabilities
		{
			get => _targetReliabilities;
			set
			{
				_targetReliabilities = value;
				OnPropertyChanged(nameof(TargetReliabilities));
			}
		}

		public void Reset()
		{
			Collections.ForEach(c => c.IsSelected = false);
			Institutions.ForEach(c => c.IsSelected = false);
			Primarities = new Primarities();
			SourceReliabilities = new Reliabilities();
			TargetReliabilities = new Reliabilities();
		}

		public void Setup()
		{
			LoadCollections();
			LoadInstitutions();

			SetFieldsSelection();
		}

		private bool AreAllCollectionsSelected() => Collections.Count > 0 && Collections.All(c => c.IsSelected);
		
		private bool AreAllInstitutionsSelected() => Institutions.Count > 0 && Institutions.All(c => c.IsSelected);

		private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(CollectionModel.IsSelected))
			{
				OnPropertyChanged(nameof(AllCollectionsChecked));
			}
			if (e.PropertyName is nameof(InstitutionModel.IsSelected))
				OnPropertyChanged(nameof(AllInstitutionsChecked));
		}

		private void FineGrainedFilterViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Settings))
			{
				Setup();
			}
		}

		private void LoadCollections()
		{
			Collections = IATEApplication.InventoriesProvider.Collections.Select(c => new CollectionModel
			{
				Name = c.Name.FullName,
				Code = c.Code,
				InstitutionName = c.Name.InstitutionName
			}).ToList();
		}

		private void LoadInstitutions()
		{
			var iateInstitutions = IATEApplication.InventoriesProvider.Institutions;

			var institutions = new List<InstitutionModel>();
			foreach (var iateInstitution in iateInstitutions)
			{
				var parent = iateInstitutions.FirstOrDefault(it => it.Code == iateInstitution.ParentCode)?.Name;

				var institutionModel = new InstitutionModel
				{
					Code = iateInstitution.Code,
					Name = iateInstitution.Name,
					Parent = parent
				};

				institutions.Add(institutionModel);
			}
			Institutions = institutions;
		}

		private void SwitchAllCollections(bool onOff)
		{
			Collections.ForEach(c => c.IsSelected = onOff);
		}

		private void SetFieldsSelection()
		{
			Settings.Collections.ForEach(sc =>
			{
				var currentCollection = Collections.FirstOrDefault(c => c.Name == sc.Name);
				if (currentCollection != null) currentCollection.IsSelected = true;
			});

			Settings.Institutions.ForEach(si =>
			{
				InstitutionModel currentInstitution = null;
				foreach (var i in Institutions)
				{
					if (i.Name != si.Name || i.Parent != si.Parent) continue;
					currentInstitution = i;
					break;
				}
				if (currentInstitution != null) currentInstitution.IsSelected = true;
			});

			Primarities = Settings.Primarities ?? new Primarities();

			SourceReliabilities = Settings.SourceReliabilities ?? new Reliabilities();
			TargetReliabilities = Settings.TargetReliabilities ?? new Reliabilities();
		}
	}

	
}