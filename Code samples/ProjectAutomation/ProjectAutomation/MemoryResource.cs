using System;

namespace ProjectAutomation
{
    public class MemoryResource
    {
        public string Path { get; set; }

        public string State { get; set; }

        public Uri Uri { get; set; }

        public string UserNameOrClientId { get; set; }

        public string UserPasswordOrClientSecret { get; set; }

        public string Credential { get; set; }

        public bool IsWindowsUser { get; set; }
    }
}
