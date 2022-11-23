using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleTranslatorProvider.Models
{
	public class ProjectLocations
	{
		public List<Location> Locations { get; private set; }
		public ProjectLocations()
		{
			Locations = new()
			{
				new Location()
				{
					DisplayName = "Global",
					Key = "global"
				},
				
				new Location()
				{
					DisplayName = "Europe",
					Key = "europe-west1"
				},

				new Location()
				{
					DisplayName = "US",
					Key = "us-central1"
				}
			};
		}
	}
}