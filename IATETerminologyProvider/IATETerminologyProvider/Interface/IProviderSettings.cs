using System.Collections.Generic;
using IATETerminologyProvider.Model;

namespace IATETerminologyProvider.Interface
{
	interface IProviderSettings
	{
		List<DomainModel> Domains { get; set; }
		List<TermTypeModel> TermTypes { get; set; }
	}
}
