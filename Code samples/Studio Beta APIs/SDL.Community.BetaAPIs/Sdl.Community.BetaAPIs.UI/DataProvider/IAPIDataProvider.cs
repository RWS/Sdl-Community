using Sdl.Community.BetaAPIs.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.BetaAPIs.UI.DataProvider
{
    public interface IAPIDataProvider
    {
        IEnumerable<API> LoadAPIs();
    }
}
