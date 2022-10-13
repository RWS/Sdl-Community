using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.Models;


namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public class EmbeddedContentViewModel : BaseModel
	{

		private bool _embeddedContentProcess;

		private string _selectedProcessorId;

		private bool _foundInCdataIsSelected;

		private bool _foundInAllIsSelected;

		public EmbeddedContentViewModel(EmbeddedContentSettings settings, List<string> embeddedProcessorIds)
		{
			Settings = settings;
			EmbeddedContentProcessorIds = new ObservableCollection<string>(embeddedProcessorIds);

			EmbeddedContentProcess = settings.EmbeddedContentProcess;
			SelectedProcessorId = EmbeddedContentProcessorIds.FirstOrDefault(a => a == settings.EmbeddedContentProcessorId)
				?? EmbeddedContentProcessorIds.FirstOrDefault();
			FoundInCdataIsSelected = settings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.CDATA;
			FoundInAllIsSelected = settings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.All;
		}

		public EmbeddedContentSettings Settings { get; set; }

		public bool EmbeddedContentProcess
		{
			get => _embeddedContentProcess;
			set
			{
				if (_embeddedContentProcess == value)
				{
					return;
				}

				_embeddedContentProcess = value;
				OnPropertyChanged(nameof(EmbeddedContentProcess));

				Settings.EmbeddedContentProcess = _embeddedContentProcess;
			}
		}

		public string SelectedProcessorId
		{
			get => _selectedProcessorId;
			set
			{
				if (_selectedProcessorId == value)
				{
					return;
				}

				_selectedProcessorId = value;
				OnPropertyChanged(nameof(SelectedProcessorId));

				Settings.EmbeddedContentProcessorId = _selectedProcessorId;
			}
		}

		public ObservableCollection<string> EmbeddedContentProcessorIds { get; set; }

		public bool FoundInCdataIsSelected
		{
			get => _foundInCdataIsSelected;
			set
			{
				if (_foundInCdataIsSelected == value)
				{
					return;
				}

				_foundInCdataIsSelected = value;
				_foundInAllIsSelected = !_foundInCdataIsSelected;


				OnPropertyChanged(nameof(FoundInCdataIsSelected));
				OnPropertyChanged(nameof(FoundInAllIsSelected));

				Settings.EmbeddedContentFoundIn = _foundInCdataIsSelected
					? EmbeddedContentSettings.FoundIn.CDATA
					: EmbeddedContentSettings.FoundIn.All;
			}
		}

		public bool FoundInAllIsSelected
		{
			get => _foundInAllIsSelected;
			set
			{
				if (_foundInAllIsSelected == value)
				{
					return;
				}

				_foundInAllIsSelected = value;
				_foundInCdataIsSelected = !_foundInAllIsSelected;

				OnPropertyChanged(nameof(FoundInAllIsSelected));
				OnPropertyChanged(nameof(FoundInCdataIsSelected));

				Settings.EmbeddedContentFoundIn = _foundInAllIsSelected
					? EmbeddedContentSettings.FoundIn.All
					: EmbeddedContentSettings.FoundIn.CDATA;
			}
		}

		public EmbeddedContentSettings ResetToDefaults()
		{
			Settings.ResetToDefaults();

			EmbeddedContentProcess = Settings.EmbeddedContentProcess;
			SelectedProcessorId = EmbeddedContentProcessorIds.FirstOrDefault(a => a == Settings.EmbeddedContentProcessorId)
								  ?? EmbeddedContentProcessorIds.FirstOrDefault();
			FoundInCdataIsSelected = Settings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.CDATA;
			FoundInAllIsSelected = Settings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.All;

			return Settings;
		}

	}
}
