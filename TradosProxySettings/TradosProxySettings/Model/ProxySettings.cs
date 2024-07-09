using System;

namespace TradosProxySettings.Model
{
    public class ProxySettings : ICloneable
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool BypassProxyOnLocal { get; set; }
        public bool IsEnabled { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
