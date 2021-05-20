using System.Drawing;

namespace Sdl.Community.StarTransit.Model
{
	public class TmSummaryOptions : BaseModel
	{
		public Image SourceFlag { get; set; }
		public Image TargetFlag { get; set; }
		public string SelectedOption { get; set; }
	}
}
