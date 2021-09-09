using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SettingsModel
	{
		public SettingsModel()
		{
			Domains = new List<DomainModel>();
			TermTypes = new List<TermTypeModel>();
			Collections = new List<CollectionModel>();
			Institutions = new List<InstitutionModel>();
			Primarities = new Primarities();
			SourceReliabilities = new Reliabilities();
			TargetReliabilities = new Reliabilities();
		}

		public List<CollectionModel> Collections { get; set; }

		public List<DomainModel> Domains { get; set; }

		public List<InstitutionModel> Institutions { get; set; }

		public Primarities Primarities { get; set; }
		public bool SearchInSubdomains { get; set; }

		public Reliabilities SourceReliabilities { get; set; }
		public Reliabilities TargetReliabilities { get; set; }

		public List<TermTypeModel> TermTypes { get; set; }
	}
}