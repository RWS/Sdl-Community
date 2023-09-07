using System;
using System.Windows.Input;
using LanguageWeaverProvider.Command;

namespace LanguageWeaverProvider.ViewModel
{
	public class FeedbackViewModel : BaseViewModel
    {
		string _feedbackMessage;
		string _segmentContent;
		int _rating;
		QualityEstimation _quality;

		public FeedbackViewModel()
		{
			InitializeCommands();
		}

		public string FeedbackMessage
		{
			get => _feedbackMessage;
			set
			{
				_feedbackMessage = value;
				OnPropertyChanged();
			}
		}

		public string SegmentContent
		{
			get => _segmentContent;
			set
			{
				_segmentContent = value;
				OnPropertyChanged();
			}
		}

		public int Rating
		{
			get => _rating;
			set
			{
				_rating = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(UserEstimation));
			}
		}

		public QualityEstimation UserEstimation => (QualityEstimation)Rating;

		public ICommand SendFeedbackCommand { get; private set; }

		private void InitializeCommands()
		{
			SendFeedbackCommand = new RelayCommand(SendFeedback);
		}

		private void SendFeedback(object parameter)
		{
			var estimation = UserEstimation switch
			{
				QualityEstimation.VeryPoor => QualityEstimation.Poor,
				QualityEstimation.VeryGood => QualityEstimation.Good,
				_ => UserEstimation,
			};
		}
	}
}