using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Input;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class LanguagePair//:BaseViewModel
    {
	    private string _tmName;
	    public Guid LanguagePairId { get; set; }
        public CultureInfo SourceLanguage { get; set; }
        public CultureInfo TargetLanguage { get; set; }
        public Image TargetFlag { get; set; }
        public Image SourceFlag { get; set; }
        public List<StarTranslationMemoryMetadata> StarTranslationMemoryMetadatas { get; set; }
        public bool HasTm { get; set; }
        public List<string> SourceFile { get; set; }
        public List<string> TargetFile { get; set; }
		/// <summary>
		/// Used if the user wants to create project without a tm in it.
		/// </summary>
        public bool NoTm { get; set; }
        public bool CreateNewTm { get; set; }
        public bool ChoseExistingTm { get; set; }
        public string TmPath { get; set; }

        //public string TmName
        //{
	       // get => _tmName;
	       // set
	       // {
		      //  _tmName = value;
		      //  OnPropertyChanged(nameof(TmName));
	       // }
        //}
        public string TmName { get; set; }

        public string PairNameIso { get; set; }
        public string PairName { get; set; }
        public ICommand SelectTmCommand { get; set; }
    }
}
