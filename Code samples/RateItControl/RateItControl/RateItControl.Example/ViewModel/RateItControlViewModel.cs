namespace RateItControl.Example.ViewModel
{
	public class RateItControlViewModel: BaseModel
	{
		private int _rating;
		private int _maxRating;
		private long _imageHeight;

		public RateItControlViewModel()
		{			
			Rating = 1;
			MaxRating = 5;
			ImageHeight = 32;
		}

		public int Rating
		{
			get => _rating;
			set
			{
				_rating = value;
				OnPropertyChanged(nameof(Rating));
				OnPropertyChanged(nameof(SelectedMessage));
			}
		}

		public int MaxRating
		{
			get => _maxRating;
			set
			{
				_maxRating = value;
				OnPropertyChanged(nameof(MaxRating));
			}
		}

		public long ImageHeight
		{
			get => _imageHeight;
			set
			{
				_imageHeight = value;
				OnPropertyChanged(nameof(ImageHeight));
			}
		}

		public string SelectedMessage => string.Format(Resources.StatusMessage_RatingOf, Rating, MaxRating);
	}
}
