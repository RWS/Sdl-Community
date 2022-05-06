using CustomViewExample.Model;

namespace CustomViewExample.ViewModel
{
	public class ContentViewModel : BaseViewModel
	{
		private CustomViewProject _project;

		public ContentViewModel(CustomViewProject project)
		{
			Name = "Content View";

			Project = project;
		}

		public CustomViewProject Project
		{
			get => _project;
			set
			{
				if (_project == value)
				{
					return;
				}

				_project = value;
				OnPropertyChanged(nameof(Project));
			}
		}
	}
}
