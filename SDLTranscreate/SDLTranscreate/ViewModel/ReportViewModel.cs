using Trados.Transcreate.Model;

namespace Trados.Transcreate.ViewModel
{
	public class ReportViewModel : BaseModel
	{				
		private string _htmlUri;
		private string _windowTitle;	
		
		public string HtmlUri
		{
			get => _htmlUri;
			set
			{
				_htmlUri = value;				
				OnPropertyChanged(nameof(HtmlUri));
			}
		}
		
		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}	
	}
}
