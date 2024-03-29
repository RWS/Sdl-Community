﻿using System.Collections.Generic;
using System.Linq;

namespace MicrosoftTranslatorProvider.Model
{
	public class RegionsProvider
	{
		public List<RegionSubscription> GetSubscriptionRegions()
		{
			var regions = new List<RegionSubscription>
			{
				new RegionSubscription {Key = "", Name = "None", Location = ""},
				new RegionSubscription {Key = "asia", Name = "Asia", Location = "Asia"},
				new RegionSubscription {Key = "asiapacific", Name = "Asia Pacific", Location = "Asia Pacific"},
				new RegionSubscription {Key = "australia", Name = "Australia", Location = "Australia"},
				new RegionSubscription {Key = "australiacentral", Name = "Australia Central", Location = "Australia Central"},
				new RegionSubscription {Key = "australiacentral2", Name = "Australia Central 2", Location = "Australia Central 2"},
				new RegionSubscription {Key = "australiaeast", Name = "Australia East", Location = "Australia East"},
				new RegionSubscription {Key = "australiasoutheast", Name = "Australia Southeast", Location = "Australia Southeast"},
				new RegionSubscription {Key = "brazil", Name = "Brazil", Location = "Brazil"},
				new RegionSubscription {Key = "brazilsouth", Name = "Brazil South", Location = "Brazil South"},
				new RegionSubscription {Key = "brazilsoutheast", Name = "Brazil Southeast", Location = "Brazil Southeast"},
				new RegionSubscription {Key = "canada", Name = "Canada", Location = "Canada"},
				new RegionSubscription {Key = "canadacentral", Name = "Canada Central", Location = "Canada Central"},
				new RegionSubscription {Key = "canadaeast", Name = "Canada East", Location = "Canada East"},
				new RegionSubscription {Key = "centralindia", Name = "Central India", Location = "Central India"},
				new RegionSubscription {Key = "centralus", Name = "Central US", Location = "Central US"},
				new RegionSubscription {Key = "centraluseuap", Name = "Central US EUAP", Location = "Central US EUAP"},
				new RegionSubscription {Key = "eastasia", Name = "East Asia", Location = "East Asia"},
				new RegionSubscription {Key = "eastus", Name = "East US", Location = "East US"},
				new RegionSubscription {Key = "eastus2", Name = "East US 2", Location = "East US 2"},
				new RegionSubscription {Key = "eastus2euap", Name = "East US 2 EUAP", Location = "East US 2 EUAP"},
				new RegionSubscription {Key = "eastusstg", Name = "East US STG", Location = "East US STG"},
				new RegionSubscription {Key = "europe", Name = "Europe", Location = "Europe"},
				new RegionSubscription {Key = "france", Name = "France", Location = "France"},
				new RegionSubscription {Key = "francecentral", Name = "France Central", Location = "France Central"},
				new RegionSubscription {Key = "francesouth", Name = "France South", Location = "France South"},
				new RegionSubscription {Key = "germany", Name = "Germany", Location = "Germany"},
				new RegionSubscription {Key = "germanynorth", Name = "Germany North", Location = "Germany North"},
				new RegionSubscription {Key = "germanywestcentral", Name = "Germany West Central", Location = "Germany West Central"},
				new RegionSubscription {Key = "global", Name = "Global", Location = "Global"},
				new RegionSubscription {Key = "india", Name = "India", Location = "India"},
				new RegionSubscription {Key = "japan", Name = "Japan", Location = "Japan"},
				new RegionSubscription {Key = "japaneast", Name = "Japan East", Location = "Japan East"},
				new RegionSubscription {Key = "japanwest", Name = "Japan West", Location = "Japan West"},
				new RegionSubscription {Key = "jioindiacentral", Name = "Jio India Central", Location = "Jio India Central"},
				new RegionSubscription {Key = "jioindiawest", Name = "Jio India West", Location = "Jio India West"},
				new RegionSubscription {Key = "korea", Name = "Korea", Location = "Korea"},
				new RegionSubscription {Key = "koreacentral", Name = "Korea Central", Location = "Korea Central"},
				new RegionSubscription {Key = "koreasouth", Name = "Korea South", Location = "Korea South"},
				new RegionSubscription {Key = "northcentralus", Name = "North Central US", Location = "North Central US"},
				new RegionSubscription {Key = "northeurope", Name = "North Europe", Location = "North Europe"},
				new RegionSubscription {Key = "norway", Name = "Norway", Location = "Norway"},
				new RegionSubscription {Key = "norwayeast", Name = "Norway East", Location = "Norway East"},
				new RegionSubscription {Key = "norwaywest", Name = "Norway West", Location = "Norway West"},
				new RegionSubscription {Key = "qatarcentral", Name = "Qatar Central", Location = "Qatar Central"},
				new RegionSubscription {Key = "singapore", Name = "Singapore", Location = "Singapore"},
				new RegionSubscription {Key = "southafrica", Name = "South Africa", Location = "South Africa"},
				new RegionSubscription {Key = "southafricanorth", Name = "South Africa North", Location = "South Africa North"},
				new RegionSubscription {Key = "southafricawest", Name = "South Africa West", Location = "South Africa West"},
				new RegionSubscription {Key = "southcentralus", Name = "South Central US", Location = "South Central US"},
				new RegionSubscription {Key = "southcentralusstg", Name = "South Central US STG", Location = "South Central US STG"},
				new RegionSubscription {Key = "southeastasia", Name = "Southeast Asia", Location = "Southeast Asia"},
				new RegionSubscription {Key = "southindia", Name = "South India", Location = "South India"},
				new RegionSubscription {Key = "swedencentral", Name = "Sweden Central", Location = "Sweden Central"},
				new RegionSubscription {Key = "switzerland", Name = "Switzerland", Location = "Switzerland"},
				new RegionSubscription {Key = "switzerlandnorth", Name = "Switzerland North", Location = "Switzerland North"},
				new RegionSubscription {Key = "switzerlandwest", Name = "Switzerland West", Location = "Switzerland West"},
				new RegionSubscription {Key = "uae", Name = "United Arab Emirates", Location = "United Arab Emirates"},
				new RegionSubscription {Key = "uaecentral", Name = "UAE Central", Location = "UAE Central"},
				new RegionSubscription {Key = "uaenorth", Name = "UAE North", Location = "UAE North"},
				new RegionSubscription {Key = "uk", Name = "United Kingdom", Location = "United Kingdom"},
				new RegionSubscription {Key = "uksouth", Name = "UK South", Location = "UK South"},
				new RegionSubscription {Key = "ukwest", Name = "UK West", Location = "UK West"},
				new RegionSubscription {Key = "unitedstates", Name = "United States", Location = "United States"},
				new RegionSubscription {Key = "unitedstateseuap", Name = "United States EUAP", Location = "United States EUAP"},
				new RegionSubscription {Key = "westcentralus", Name = "West Central US", Location = "West Central US"},
				new RegionSubscription {Key = "westeurope", Name = "West Europe", Location = "West Europe"},
				new RegionSubscription {Key = "westindia", Name = "West India", Location = "West India"},
				new RegionSubscription {Key = "westus", Name = "West US", Location = "West US"},
				new RegionSubscription {Key = "westus2", Name = "West US 2", Location = "West US 2"},
				new RegionSubscription {Key = "westus3", Name = "West US 3", Location = "West US 3"},
				new RegionSubscription {Key = "centralusstage", Name = "Central US (Stage)", Location = "Central US (Stage)"},
				new RegionSubscription {Key = "eastasiastage", Name = "East Asia (Stage)", Location = "East Asia (Stage)"},
				new RegionSubscription {Key = "eastus2stage", Name = "East US 2 (Stage)", Location = "East US 2 (Stage)"},
				new RegionSubscription {Key = "eastusstage", Name = "East US (Stage)", Location = "East US (Stage)"},
				new RegionSubscription {Key = "northcentralusstage", Name = "North Central US (Stage)", Location = "North Central US (Stage)"},
				new RegionSubscription {Key = "southcentralusstage", Name = "South Central US (Stage)", Location = "South Central US (Stage)"},
				new RegionSubscription {Key = "southeastasiastage", Name = "Southeast Asia (Stage)", Location = "Southeast Asia (Stage)"},
				new RegionSubscription {Key = "westus2stage", Name = "West US 2 (Stage)", Location = "West US 2 (Stage)"},
				new RegionSubscription {Key = "westusstage", Name = "West US (Stage)", Location = "West US (Stage)"},
			};

			return regions;
		}
	}
}