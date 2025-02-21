using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Sdl.Community.StarTransit.Shared.Models
{
	//TODO: For final implementation remove unused properties
    public class LanguagePair:BaseViewModel
    {
	    private string _tmName;
	    private string _tmPath;
	    private bool _chooseExistingTm;
	    private bool _noTm;
	    private bool _createNewTm;
	    public delegate void TmOptionsEventRaised();
	    public event TmOptionsEventRaised TmOptionChangedEventRaised;

	    public delegate void BrowseTmEventRaised();
	    public event BrowseTmEventRaised BrowseTmChangedEventRaised;

		public Guid LanguagePairId { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public Image TargetFlag { get; set; }
        public Image SourceFlag { get; set; }
		/// <summary>
		/// List of all TMs/MT Files from Transit package
		/// </summary>
        public List<StarTranslationMemoryMetadata> StarTranslationMemoryMetadatas { get; set; }
		public List<StarTranslationMemoryMetadata> SelectedTranslationMemoryMetadatas { get; set; }
		public bool HasTm { get; set; }
        public List<string> SourceFile { get; set; }
        public List<string> TargetFile { get; set; }
        public int TemplatePenalty { get; set; }

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
		        if (_noTm)
		        {
			        TmName = string.Empty;
			        TmPath = string.Empty;
			        TmOptionChangedEventRaised?.Invoke();
		        }
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
		        
		        if (_createNewTm)
				{
					TmName = string.Empty;
					TmPath = string.Empty;
					ChoseExistingTm = false;
					TmOptionChangedEventRaised?.Invoke();
				}
				OnPropertyChanged(nameof(CreateNewTm));
	        }
        }

        public bool ChoseExistingTm
        {
	        get => _chooseExistingTm;
	        set
	        {
		        _chooseExistingTm = value;
		        BrowseTmChangedEventRaised?.Invoke();
		        OnPropertyChanged(nameof(ChoseExistingTm));
	        }
        }

        public string TmName
        {
	        get => _tmName;
	        set
	        {
				if(_tmName == value)return;
				_tmName = value;
		        OnPropertyChanged(nameof(TmName));
	        }
        }

        public string TmPath
        {
	        get => _tmPath;
	        set
	        {
		        if (_tmPath == value) return;
		        _tmPath = value;
		        OnPropertyChanged(nameof(TmPath));
	        }
        }

        public string PairNameIso { get; set; }
        public string PairName { get; set; }
        public ICommand SelectTmCommand { get; set; }
        public ICommand RemoveSelectedTmCommand { get; set; }
    }
}
