using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Input;

namespace Sdl.Community.StarTransit.Shared.Models
{
	//TODO: For final implementation remove unused properties
    public class LanguagePair:BaseViewModel
    {
	    private string _tmName;
	    private bool _chooseExistingTm;
	    private bool _noTm;
	    private bool _createNewTm;
	    public delegate void ClearMessageRaiser();
	    public event ClearMessageRaiser ClearEventRaised;

	    public Guid LanguagePairId { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public Image TargetFlag { get; set; }
        public Image SourceFlag { get; set; }
		/// <summary>
		/// List of all TMs/MT Files from Transit package
		/// </summary>
        public List<StarTranslationMemoryMetadata> StarTranslationMemoryMetadatas { get; set; }
		/// <summary>
		/// List of Transit TMs/MT which doesn't have penalties set. All this files will be imported in a "Main TM" file
		/// </summary>
		public List<StarTranslationMemoryMetadata> TmsForMainTm { get; set; }

		/// <summary>
		/// List of Transit TMs/MT which have penalty set. We'll create a different Studio TM for each file with the penalty set
		/// </summary>
		public List<StarTranslationMemoryMetadata> IndividualTms { get; set; }
		public bool HasTm { get; set; }
        public List<string> SourceFile { get; set; }
        public List<string> TargetFile { get; set; }

        /// <summary>
        /// Used if the user wants to create project without a tm in it.
        /// </summary>
        public bool NoTm
        {
	        get => _noTm;
	        set
	        {
		        if (_noTm == value) return;
		        _noTm = value;
		        TmName = string.Empty;
		        TmPath = string.Empty;
		        ClearEventRaised?.Invoke();
				OnPropertyChanged(nameof(NoTm));
	        }
        }

        public bool CreateNewTm
        {
	        get => _createNewTm;
	        set
	        {
		        if (_createNewTm == value) return;
		        _createNewTm = value;
		        TmName = string.Empty;
		        TmPath = string.Empty;
		        ClearEventRaised?.Invoke();
		        OnPropertyChanged(nameof(CreateNewTm));
	        }
        }

        public bool ChoseExistingTm
        {
	        get => _chooseExistingTm;
	        set
	        {
				if(_chooseExistingTm == value)return;
		        _chooseExistingTm = value;
		        OnPropertyChanged(nameof(ChoseExistingTm));
	        }
        }

        public string TmName
        {
	        get => _tmName;
	        set
	        {
		        _tmName = value;
		        OnPropertyChanged(nameof(TmName));
	        }
        }

        public string TmPath { get; set; }

		public string PairNameIso { get; set; }
        public string PairName { get; set; }
        public ICommand SelectTmCommand { get; set; }
        public ICommand RemoveSelectedTmCommand { get; set; }
    }
}
