using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class LinguisticOption
	{
		private string _selectedValue;

		public string SelectedValue
		{
			get => _selectedValue ??= SystemDefault;
			set => _selectedValue = value;
		}

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("systemDefault")]
		public string SystemDefault { get; set; }

		[JsonProperty("values")]
		public List<string> Values { get; set; }
	}
}