using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider.MemoQServerTypes
{
    internal class LoginResult
    {
        public string Name { get; set; }
        public string Sid { get; set; }
        public string AccessToken { get; set; }

        public LoginResult(string name, string sid, string accessToken)
        {
            Name = name;
            Sid = sid;
            AccessToken = accessToken;
        }
    }
}
