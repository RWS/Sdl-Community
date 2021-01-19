using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SettingsModel
	{
		public List<DomainModel> Domains { get; set; }
		public List<TermTypeModel> TermTypes { get; set; }
		public bool SearchInSubdomains { get; set; }
	}
}
