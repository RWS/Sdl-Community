using System;
using Sdl.Community.YourProductivity.Model;

namespace Sdl.Community.YourProductivity.Services
{
    public class ShareService
    {
        private readonly ProductivityService _productivityService;
        private readonly VersioningService _versioningService;

        public Version LeaderboardVersion;
        public Version PluginVersion;

		public ShareService(ProductivityService productivityService,
			VersioningService versioningService)
		{
			_productivityService = productivityService;
			_versioningService = versioningService;
		}		
	}
}
