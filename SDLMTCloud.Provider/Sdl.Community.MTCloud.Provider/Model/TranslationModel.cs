using System.Collections.Generic;
using System.Runtime.Serialization;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	[DataContract]
	public class TranslationModel : BaseViewModel
	{
		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string Model { get; set; }

		[DataMember]
		public MTCloudLanguagePair MTCloudLanguagePair { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public string Target { get; set; }

		[DataMember]
		public List<LinguisticOption> LinguisticOptions { get; set; }
	}
}