using System.Collections.Generic;
using LanguageWeaverProvider.ViewModel;

namespace LanguageWeaverProvider.Studio.FeedbackController.Model
{
	public class TranslationErrors : BaseViewModel
	{
		bool _wordChoice;
		bool _wordsOmission;
		bool _wordsAddition;
		bool _unintelligible;
		bool _grammar;
		bool _spelling;
		bool _punctuation;
		bool _capitalization;

		public bool WordChoice
		{
			get => _wordChoice;
			set
			{
				_wordChoice = value;
				OnPropertyChanged();
			}
		}

		public bool WordsOmission
		{
			get => _wordsOmission;
			set
			{
				_wordsOmission = value;
				OnPropertyChanged();
			}
		}

		public bool WordsAddition
		{
			get => _wordsAddition;
			set
			{
				_wordsAddition = value;
				OnPropertyChanged();
			}
		}

		public bool Unintelligible
		{
			get => _unintelligible;
			set
			{
				_unintelligible = value;
				OnPropertyChanged();
			}
		}

		public bool Grammar
		{
			get => _grammar;
			set
			{
				_grammar = value;
				OnPropertyChanged();
			}
		}

		public bool Spelling
		{
			get => _spelling;
			set
			{
				_spelling = value;
				OnPropertyChanged();
			}
		}

		public bool Punctuation
		{
			get => _punctuation;
			set
			{
				_punctuation = value;
				OnPropertyChanged();
			}
		}

		public bool Capitalization
		{
			get => _capitalization;
			set
			{
				_capitalization = value;
				OnPropertyChanged();
			}
		}

		public List<string> GetProblemsReported()
		{
			var problemsReported = new List<string>();

			if (WordsOmission)
			{
				problemsReported.Add(Constants.WordsOmission);
			}

			if (WordsAddition)
			{
				problemsReported.Add(Constants.WordsAddition);
			}

			if (WordChoice)
			{
				problemsReported.Add(Constants.WordChoice);
			}

			if (Unintelligible)
			{
				problemsReported.Add(Constants.Unintelligible);
			}

			if (Grammar)
			{
				problemsReported.Add(Constants.Grammar);
			}

			if (Spelling)
			{
				problemsReported.Add(Constants.Spelling);
			}

			if (Punctuation || Capitalization)
			{
				problemsReported.Add(Constants.CapitalizationPunctuation);
			}

			return problemsReported;
		}

		public void ResetValues()
		{
			WordsOmission = default;
			WordsAddition = default;
			WordChoice = default;
			Unintelligible = default;
			Grammar = default;
			Spelling = default;
			Punctuation = default;
			Capitalization = default;
		}
	}
}