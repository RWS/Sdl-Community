using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider
{
    public enum LoginTypes
    {
        Undefined,
        /// <summary>
        /// LT login for a memoQ server provider.
        /// </summary>
        MemoQServerLT,
        MemoQServer,
        Windows,
        WindowsIntegrated
    }
}
