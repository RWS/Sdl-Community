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
			var shortcutService = Substitute.For<IShortcutService>();
			var providerAction = Substitute.For<IActionProvider>();
			var segmentSupervisor = Substitute.For<ISegmentSupervisor>();
			var messageBoxService = Substitute.For<IMessageBoxService>();
			//_rateItViewModel = new RateItViewModel(shortcutService, providerAction, segmentSupervisor, messageBoxService, editorController);
		}

		//[Theory]
		//[InlineData(true)]
		//public void Set_WordsOmission_Returns_True(bool omissionChecked)
		//{
		//	_rateItViewModel.WordsOmissionOption.IsChecked = omissionChecked;

		//	Assert.True(_rateItViewModel.WordsOmissionOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(false)]
		//public void Set_WordsOmission_Returns_False(bool omissionChecked)
		//{
		//	_rateItViewModel.WordsOmissionOption.IsChecked = true;
		//	_rateItViewModel.WordsOmissionOption.IsChecked = omissionChecked;

		//	Assert.False(_rateItViewModel.WordsOmissionOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(true,nameof(RateItViewModel.WordsOmissionOption))]
		//public void Set_WordsOmissionFromShortcuts_Returns_False(bool initialState,string optionName)
		//{
		//	_rateItViewModel.WordsOmissionOption.IsChecked = initialState;

		//	_rateItViewModel.SetRateOptionFromShortcuts(optionName);

		//	Assert.False(_rateItViewModel.WordsOmissionOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(false, nameof(RateItViewModel.WordsOmissionOption))]
		//public void Set_WordsOmissionFromShortcuts_Returns_True(bool initialState, string optionName)
		//{
		//	_rateItViewModel.WordsOmissionOption.IsChecked = initialState;

		//	_rateItViewModel.SetRateOptionFromShortcuts(optionName);

		//	Assert.True(_rateItViewModel.WordsOmissionOption.IsChecked);
		//}


		//[Theory]
		//[InlineData("ALT+C", nameof(RateItViewModel.WordsOmissionOption), Skip = "View model refactored")]
		//public void Set_WordsOmission_Tooltip(string tooltip, string optionName)
		//{
		//	//_rateItViewModel.SetActionTooltip(optionName,tooltip);

		//	Assert.Equal(tooltip,_rateItViewModel.WordsOmissionOption.Tooltip);
		//}

		//[Theory]
		//[InlineData(true)]
		//public void Set_Grammar_Returns_True(bool grammarChecked)
		//{
		//	_rateItViewModel.GrammarOption.IsChecked = grammarChecked;

		//	Assert.True(_rateItViewModel.GrammarOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false)]
		//public void Set_Grammar_Returns_False(bool grammarChecked)
		//{
		//	_rateItViewModel.GrammarOption.IsChecked = true;
		//	_rateItViewModel.GrammarOption.IsChecked = grammarChecked;

		//	Assert.False(_rateItViewModel.GrammarOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(true, nameof(RateItViewModel.GrammarOption))]
		//public void Set_GrammarFromShortcuts_Returns_False(bool initialState, string optionName)
		//{
		//	_rateItViewModel.GrammarOption.IsChecked = initialState;

		//	_rateItViewModel.SetRateOptionFromShortcuts(optionName);

		//	Assert.False(_rateItViewModel.GrammarOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false, nameof(RateItViewModel.GrammarOption))]
		//public void Set_GrammarFromShortcuts_Returns_True(bool initialState, string optionName)
		//{
		//	_rateItViewModel.GrammarOption.IsChecked = initialState;

		//	_rateItViewModel.SetRateOptionFromShortcuts(optionName);

		//	Assert.True(_rateItViewModel.GrammarOption.IsChecked);
		//}
		//[Theory]
		//[InlineData("ALT+I", nameof(RateItViewModel.GrammarOption), Skip = "View model refactored")]
		//public void Set_Grammar_Tooltip(string tooltip, string optionName)
		//{
		//	//_rateItViewModel.SetActionTooltip(optionName, tooltip);

		//	Assert.Equal(tooltip, _rateItViewModel.GrammarOption.Tooltip);
		//}

		//[Theory]
		//[InlineData(true)]
		//public void Set_Unintelligence_Returns_True(bool unintelligenceChecked)
		//{
		//	_rateItViewModel.UnintelligenceOption.IsChecked = unintelligenceChecked;

		//	Assert.True(_rateItViewModel.UnintelligenceOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(false)]
		//public void Set_Unintelligence_Returns_False(bool unintelligenceChecked)
		//{
		//	_rateItViewModel.UnintelligenceOption.IsChecked = true;
		//	_rateItViewModel.UnintelligenceOption.IsChecked = unintelligenceChecked;

		//	Assert.False(_rateItViewModel.UnintelligenceOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(true)]
		//public void Set_WordChoice_Returns_True(bool wordChoiceChecked)
		//{
		//	_rateItViewModel.WordChoiceOption.IsChecked = wordChoiceChecked;

		//	Assert.True(_rateItViewModel.WordChoiceOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false)]
		//public void Set_WordChoice_Returns_False(bool wordChoiceChecked)
		//{
		//	_rateItViewModel.WordChoiceOption.IsChecked = true;
		//	_rateItViewModel.WordChoiceOption.IsChecked = wordChoiceChecked;

		//	Assert.False(_rateItViewModel.WordChoiceOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(true)]
		//public void Set_WordsAddition_Returns_True(bool wordsAdditionChecked)
		//{
		//	_rateItViewModel.WordsAdditionOption.IsChecked = wordsAdditionChecked;

		//	Assert.True(_rateItViewModel.WordsAdditionOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false)]
		//public void Set_WordsAddition_Returns_False(bool wordsAdditionChecked)
		//{
		//	_rateItViewModel.WordsAdditionOption.IsChecked = false;
		//	_rateItViewModel.WordsAdditionOption.IsChecked = wordsAdditionChecked;

		//	Assert.False(_rateItViewModel.WordsAdditionOption.IsChecked);
		//}

		//[Theory]
		//[InlineData(true)]
		//public void Set_Spelling_Returns_True(bool spellingChecked)
		//{
		//	_rateItViewModel.SpellingOption.IsChecked = spellingChecked;

		//	Assert.True(_rateItViewModel.SpellingOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false)]
		//public void Set_Spelling_Returns_False(bool spellingChecked)
		//{
		//	_rateItViewModel.SpellingOption.IsChecked = true;
		//	_rateItViewModel.SpellingOption.IsChecked = spellingChecked;

		//	Assert.False(_rateItViewModel.SpellingOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(true)]
		//public void Set_Capitalization_Returns_True(bool capitalizationChecked)
		//{
		//	_rateItViewModel.CapitalizationOption.IsChecked = capitalizationChecked;

		//	Assert.True(_rateItViewModel.CapitalizationOption.IsChecked);
		//}
		//[Theory]
		//[InlineData(false)]
		//public void Set_Capitalization_Returns_False(bool capitalizationChecked)
		//{
		//	_rateItViewModel.CapitalizationOption.IsChecked = true;
		//	_rateItViewModel.CapitalizationOption.IsChecked = capitalizationChecked;

		//	Assert.False(_rateItViewModel.CapitalizationOption.IsChecked);
		//}

		[Theory]
		[InlineData("This is the feedback")]
		public void Set_FeedbackText(string feedback)
		{
			_rateItViewModel.FeedbackMessage = feedback;
			Assert.Equal(feedback,_rateItViewModel.FeedbackMessage);
		}

		[Theory]
		[InlineData(2)]
		public void SetRating_FromShortcuts_NoRating_Initialy(int ratingValue)
		{
			_rateItViewModel.IncreaseRating();
			_rateItViewModel.IncreaseRating();

			Assert.Equal(ratingValue,_rateItViewModel.Rating);
		}

		[Theory]
		[InlineData(3)]
		public void SetRating_FromShortcuts_OneStar_Initialy(int ratingValue)
		{
			_rateItViewModel.Rating = 1;
			_rateItViewModel.IncreaseRating();
			_rateItViewModel.IncreaseRating();

			Assert.Equal(ratingValue, _rateItViewModel.Rating);
		}

		[Fact]
		public void SetRating_MaxNumber()
		{
			_rateItViewModel.Rating = 5;
			_rateItViewModel.IncreaseRating();

			Assert.Equal(5,_rateItViewModel.Rating);
		}

		[Fact]
		public void DecreaseRating_FromShortcuts_NoRating_Initialy()
		{
			_rateItViewModel.DecreaseRating();
			_rateItViewModel.DecreaseRating();

			Assert.Equal(0, _rateItViewModel.Rating);
		}

		[Theory]
		[InlineData(0)]
		public void DecreaseRating_FromShortcuts_OneStar_Initialy(int ratingValue)
		{
			_rateItViewModel.Rating = 1;
			_rateItViewModel.DecreaseRating();

			Assert.Equal(ratingValue, _rateItViewModel.Rating);
		}

		[Theory]
		[InlineData(1)]
		public void DecreaseRating_FromShortcuts_TwoStars_Initialy(int ratingValue)
		{
			_rateItViewModel.Rating = 2;
			_rateItViewModel.DecreaseRating();

			Assert.Equal(ratingValue, _rateItViewModel.Rating);
		}

	}
}
