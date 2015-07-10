using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sdl.Community.Productivity.API;

namespace Sdl.Community.Productivity.Services
{
    public class VersioningService
    {
        private readonly LeaderboardApi _leaderboardApi;
        public string PluginVersion { get; set; }
        public string LeaderboardVersion { get; set; }
        public VersioningService(LeaderboardApi leaderboardApi)
        {
            _leaderboardApi = leaderboardApi;

            Initialize();
        }

        private void Initialize()
        {
            PluginVersion = GetPluginVersion();
            LeaderboardVersion = GetLeaderboardVersion();
        }

        private string GetLeaderboardVersion()
        {
            if (!_leaderboardApi.IsAlive()) return GetPluginVersion();
            var request = new RestRequest(Method.GET) { Resource = "version", RequestFormat = DataFormat.Json };
            dynamic dVersion = JObject.Parse(_leaderboardApi.Execute(request));
            return dVersion.version;
        }

        private string GetPluginVersion()
        {
            var assembly = typeof(ShareService).Assembly;
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fullVersion = versionInfo.FileVersion;
            return fullVersion;
        }

        public Version GetStudioVersion()
        {
            var currentDomain = AppDomain.CurrentDomain;
            var assembly = Assembly.LoadFile(string.Format(@"{0}\{1}", currentDomain.BaseDirectory, "SDLTradosStudio.exe"));
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(versionInfo.FileVersion);
        }

        public string GetStudioVersionFriendly()
        {
            var version = GetStudioVersion();
            return version.Major == 11 ? "studio2014" : "studio2015";
        }

        public bool IsPluginVersionCompatibleWithLeaderboardVersion()
        {
            return String.Compare(LeaderboardVersion, PluginVersion, StringComparison.Ordinal) <= 0;
        }
    }
}
