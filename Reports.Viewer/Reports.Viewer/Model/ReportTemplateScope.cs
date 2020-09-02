using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.Model
{
	public class ReportTemplateScope
	{
		public ReportTemplate.TemplateScope Scope { get; set; }

		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
