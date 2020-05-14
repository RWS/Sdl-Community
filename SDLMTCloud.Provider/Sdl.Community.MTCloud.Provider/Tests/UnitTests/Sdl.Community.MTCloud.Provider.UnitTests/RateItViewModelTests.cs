using NSubstitute;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.ViewModel;
using Xunit;

namespace Sdl.Community.MTCloud.Provider.UnitTests
{
	public class RateItViewModelTests
	{
		private readonly RateItViewModel _rateItViewModel;

		public RateItViewModelTests()
		{
			var translationService = Substitute.For<ITranslationService>();
			var shortcutService = Substitute.For<IShortcutService>();
			_rateItViewModel = new RateItViewModel(translationService,shortcutService);
		}

		[Theory]
		[InlineData(true)]
		public void Set_WordsOmission_Returns_True(bool omissionChecked)
		{
			_rateItViewModel.WordsOmissionChecked = omissionChecked;

			Assert.True(_rateItViewModel.WordsOmissionChecked);
		}

		[Theory]
		[InlineData(false)]
		public void Set_WordsOmission_Returns_False(bool omissionChecked)
		{
			_rateItViewModel.WordsOmissionChecked = true;
			_rateItViewModel.WordsOmissionChecked = omissionChecked;

			Assert.False(_rateItViewModel.WordsOmissionChecked);
		}

		[Theory]
		[InlineData(true,nameof(RateItViewModel.WordsOmissionChecked))]
		public void Set_WordsOmissionFromShortcuts_Returns_False(bool initialState,string optionName)
		{
			_rateItViewModel.WordsOmissionChecked = initialState;

			_rateItViewModel.SetRateOptionFromShortcuts(optionName);

			Assert.False(_rateItViewModel.WordsOmissionChecked);
		}

		[Theory]
		[InlineData(false, nameof(RateItViewModel.WordsOmissionChecked))]
		public void Set_WordsOmissionFromShortcuts_Returns_True(bool initialState, string optionName)
		{
			_rateItViewModel.WordsOmissionChecked = initialState;

			_rateItViewModel.SetRateOptionFromShortcuts(optionName);

			Assert.True(_rateItViewModel.WordsOmissionChecked);
		}

		[Theory]
		[InlineData(true)]
		public void Set_Grammar_Returns_True(bool grammarChecked)
		{
			_rateItViewModel.GrammarChecked = grammarChecked;

			Assert.True(_rateItViewModel.GrammarChecked);
		}
		[Theory]
		[InlineData(false)]
		public void Set_Grammar_Returns_False(bool grammarChecked)
		{
			_rateItViewModel.GrammarChecked = true;
			_rateItViewModel.GrammarChecked = grammarChecked;

			Assert.False(_rateItViewModel.GrammarChecked);
		}
		[Theory]
		[InlineData(true, nameof(RateItViewModel.GrammarChecked))]
		public void Set_GrammarFromShortcuts_Returns_False(bool initialState, string optionName)
		{
			_rateItViewModel.GrammarChecked = initialState;

			_rateItViewModel.SetRateOptionFromShortcuts(optionName);

			Assert.False(_rateItViewModel.GrammarChecked);
		}
		[Theory]
		[InlineData(false, nameof(RateItViewModel.GrammarChecked))]
		public void Set_GrammarFromShortcuts_Returns_True(bool initialState, string optionName)
		{
			_rateItViewModel.GrammarChecked = initialState;

			_rateItViewModel.SetRateOptionFromShortcuts(optionName);

			Assert.True(_rateItViewModel.GrammarChecked);
		}

		[Theory]
		[InlineData(true)]
		public void Set_Unintelligence_Returns_True(bool unintelligenceChecked)
		{
			_rateItViewModel.UnintelligenceChecked = unintelligenceChecked;

			Assert.True(_rateItViewModel.UnintelligenceChecked);
		}

		[Theory]
		[InlineData(false)]
		public void Set_Unintelligence_Returns_False(bool unintelligenceChecked)
		{
			_rateItViewModel.UnintelligenceChecked = true;
			_rateItViewModel.UnintelligenceChecked = unintelligenceChecked;

			Assert.False(_rateItViewModel.UnintelligenceChecked);
		}

		[Theory]
		[InlineData(true)]
		public void Set_WordChoice_Returns_True(bool wordChoiceChecked)
		{
			_rateItViewModel.WordChoiceChecked = wordChoiceChecked;

			Assert.True(_rateItViewModel.WordChoiceChecked);
		}
		[Theory]
		[InlineData(false)]
		public void Set_WordChoice_Returns_False(bool wordChoiceChecked)
		{
			_rateItViewModel.WordChoiceChecked = true;
			_rateItViewModel.WordChoiceChecked = wordChoiceChecked;

			Assert.False(_rateItViewModel.WordChoiceChecked);
		}

		[Theory]
		[InlineData(true)]
		public void Set_WordsAddition_Returns_True(bool wordsAdditionChecked)
		{
			_rateItViewModel.WordsAdditionChecked = wordsAdditionChecked;

			Assert.True(_rateItViewModel.WordsAdditionChecked);
		}
		[Theory]
		[InlineData(false)]
		public void Set_WordsAddition_Returns_False(bool wordsAdditionChecked)
		{
			_rateItViewModel.WordsAdditionChecked = false;
			_rateItViewModel.WordsAdditionChecked = wordsAdditionChecked;

			Assert.False(_rateItViewModel.WordsAdditionChecked);
		}

		[Theory]
		[InlineData(true)]
		public void Set_Spelling_Returns_True(bool spellingChecked)
		{
			_rateItViewModel.SpellingChecked = spellingChecked;

			Assert.True(_rateItViewModel.SpellingChecked);
		}
		[Theory]
		[InlineData(false)]
		public void Set_Spelling_Returns_False(bool spellingChecked)
		{
			_rateItViewModel.SpellingChecked = true;
			_rateItViewModel.SpellingChecked = spellingChecked;

			Assert.False(_rateItViewModel.SpellingChecked);
		}
		[Theory]
		[InlineData(true)]
		public void Set_Capitalization_Returns_True(bool capitalizationChecked)
		{
			_rateItViewModel.CapitalizationChecked = capitalizationChecked;

			Assert.True(_rateItViewModel.CapitalizationChecked);
		}
		[Theory]
		[InlineData(false)]
		public void Set_Capitalization_Returns_False(bool capitalizationChecked)
		{
			_rateItViewModel.CapitalizationChecked = true;
			_rateItViewModel.CapitalizationChecked = capitalizationChecked;

			Assert.False(_rateItViewModel.CapitalizationChecked);
		}

		[Theory]
		[InlineData("This is the feedback")]
		public void Set_FeedbackText(string feedback)
		{
			_rateItViewModel.Feedback = feedback;
			Assert.Equal(feedback,_rateItViewModel.Feedback);
		}
	}
}
