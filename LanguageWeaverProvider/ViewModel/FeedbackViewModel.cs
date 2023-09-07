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
		QualityEstimations _quality;

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

		public QualityEstimations UserEstimation => (QualityEstimations)Rating;

		public ICommand SendFeedbackCommand { get; private set; }

		private void InitializeCommands()
		{
			SendFeedbackCommand = new RelayCommand(SendFeedback);
		}

		private void SendFeedback(object parameter)
		{
			var estimation = UserEstimation switch
			{
				QualityEstimations.VeryPoor => QualityEstimations.Poor,
				QualityEstimations.VeryGood => QualityEstimations.Good,
				_ => UserEstimation,
			};
		}
	}
}