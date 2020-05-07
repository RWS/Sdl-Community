using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
	interface IProviderSettings
	{
		List<DomainModel> Domains { get; set; }
		List<TermTypeModel> TermTypes { get; set; }
	}
}
