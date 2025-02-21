namespace Reports.Viewer.Api.Model
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
