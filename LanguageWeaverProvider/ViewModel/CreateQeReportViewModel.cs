namespace LanguageWeaverProvider.ViewModel
{
	public class CreateQeReportViewModel : BaseViewModel
	{
		bool _excludeLockedSegments;

		public bool ExcludeLockedSegments
		{
			get => _excludeLockedSegments;
			set
			{
				_excludeLockedSegments = value;
				OnPropertyChanged();
			}
		}
	}
}