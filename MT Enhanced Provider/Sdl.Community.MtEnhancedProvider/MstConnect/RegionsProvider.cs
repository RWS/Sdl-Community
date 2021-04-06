using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.MstConnect
{
	public class RegionsProvider
	{
		public List<SubscriptionRegion> GetSubscriptionRegions()
		{
			var regions = new List<SubscriptionRegion>
			{
				new SubscriptionRegion {Key = "", Name = "None", Location = ""},
				new SubscriptionRegion {Key = "australiaeast", Name = "Australia East", Location = "Australia"},
				new SubscriptionRegion {Key = "brazilsouth	", Name = "Brazil South", Location = "Brazil"},
				new SubscriptionRegion {Key = "canadacentral", Name = "Canada Central", Location = "Canada"},
				new SubscriptionRegion {Key = "centralindia", Name = "Central India", Location = "India"},
				new SubscriptionRegion {Key = "centralus", Name = "Central US", Location = "US"},
				new SubscriptionRegion {Key = "centraluseuap", Name = "Central US EUAP", Location = "US"},
				new SubscriptionRegion {Key = "eastasia", Name = "East Asia", Location = "Asia"},
				new SubscriptionRegion {Key = "eastus", Name = "East US", Location = "US"},
				new SubscriptionRegion {Key = "eastus2", Name = "East US 2", Location = "US"},
				new SubscriptionRegion {Key = "francecentral", Name = "France Central", Location = "France"},
				new SubscriptionRegion {Key = "japaneast", Name = "Japan East", Location = "Japan"},
				new SubscriptionRegion {Key = "japanwest", Name = "Japan West", Location = "Japan"},
				new SubscriptionRegion {Key = "koreacentral", Name = "Korea Central", Location = "Korea"},
				new SubscriptionRegion {Key = "northcentralus	", Name = "North Central US", Location = "US"},
				new SubscriptionRegion {Key = "northeurope", Name = "North Europe", Location = "Europe"},
				new SubscriptionRegion {Key = "southcentralus", Name = "South Central US", Location = "US"},
				new SubscriptionRegion {Key = "southeastasia", Name = "South East Asia", Location = "Asia"},
				new SubscriptionRegion {Key = "uksouth", Name = "UK South", Location = "UK"},
				new SubscriptionRegion {Key = "westcentralus", Name = "West Central US", Location = "US"},
				new SubscriptionRegion {Key = "westeurope", Name = "West Europe", Location = "Europe"},
				new SubscriptionRegion {Key = "westus", Name = "West US", Location = "US"},
				new SubscriptionRegion {Key = "westus2", Name = "West US 2", Location = "US"},
				new SubscriptionRegion {Key = "southafricanorth", Name = "South Africa North", Location = "South Africa"}
			};

			return regions.OrderBy(a => a.Location).ThenBy(b => b.Name).ToList();
		}
	}
}
