namespace RateItControl.Example.ViewModel
{
	public class RateItControlViewModel: BaseModel
	{
		private int _rating;
		private int _maxRating;

		public RateItControlViewModel()
		{			
			Rating = 1;
			MaxRating = 5;
		}

		public int Rating
		{
			get => _rating;
			set
			{
				_rating = value;
				OnPropertyChanged(nameof(Rating));
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
	}
}
