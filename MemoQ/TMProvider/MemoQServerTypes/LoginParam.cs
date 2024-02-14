using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider.MemoQServerTypes
{
    internal class LoginParam
    {
        public LoginParam(string user, string pwd, string ltst, LoginMode loginMode)
        {
            this.UserName = user;
            this.Password = pwd;
            this.LoginMode = loginMode;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginMode LoginMode { get; set; }
    }
    internal enum LoginMode
    {
        MemoQServerUser,
        WindowsUser

        // WindowsIntegrated - currently removed
    }
}
