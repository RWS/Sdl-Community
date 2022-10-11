using System.Collections.Generic;
using System.Linq;

namespace MTEnhancedMicrosoftProvider.Model
{
	public class RegionsProvider
	{
		public List<RegionSubscription> GetSubscriptionRegions()
		{
			List<RegionSubscription> regions = new List<RegionSubscription>
			{
				new RegionSubscription {Key = "", Name = "None", Location = ""},
				new RegionSubscription {Key = "australiaeast", Name = "Australia East", Location = "Australia"},
				new RegionSubscription {Key = "brazilsouth	", Name = "Brazil South", Location = "Brazil"},
				new RegionSubscription {Key = "canadacentral", Name = "Canada Central", Location = "Canada"},
				new RegionSubscription {Key = "centralindia", Name = "Central India", Location = "India"},
				new RegionSubscription {Key = "centralus", Name = "Central US", Location = "US"},
				new RegionSubscription {Key = "centraluseuap", Name = "Central US EUAP", Location = "US"},
				new RegionSubscription {Key = "eastasia", Name = "East Asia", Location = "Asia"},
				new RegionSubscription {Key = "eastus", Name = "East US", Location = "US"},
				new RegionSubscription {Key = "eastus2", Name = "East US 2", Location = "US"},
				new RegionSubscription {Key = "francecentral", Name = "France Central", Location = "France"},
				new RegionSubscription {Key = "japaneast", Name = "Japan East", Location = "Japan"},
				new RegionSubscription {Key = "japanwest", Name = "Japan West", Location = "Japan"},
				new RegionSubscription {Key = "koreacentral", Name = "Korea Central", Location = "Korea"},
				new RegionSubscription {Key = "northcentralus	", Name = "North Central US", Location = "US"},
				new RegionSubscription {Key = "northeurope", Name = "North Europe", Location = "Europe"},
				new RegionSubscription {Key = "southafricanorth", Name = "South Africa North", Location = "South Africa"},
				new RegionSubscription {Key = "southcentralus", Name = "South Central US", Location = "US"},
				new RegionSubscription {Key = "southeastasia", Name = "South East Asia", Location = "Asia"},
				new RegionSubscription {Key = "uksouth", Name = "UK South", Location = "UK"},
				new RegionSubscription {Key = "westcentralus", Name = "West Central US", Location = "US"},
				new RegionSubscription {Key = "westeurope", Name = "West Europe", Location = "Europe"},
				new RegionSubscription {Key = "westus", Name = "West US", Location = "US"},
				new RegionSubscription {Key = "westus2", Name = "West US 2", Location = "US"}
			};

			return regions.OrderBy(a => a.Key).ToList();
		}
	}
}