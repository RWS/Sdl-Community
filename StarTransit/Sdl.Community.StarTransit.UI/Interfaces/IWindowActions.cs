using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.UI.Interfaces
{
    public interface IWindowActions
    {
        Action CloseAction { get; set; }
        Action<string,string> ShowWindowsMessage { get; set; }
    }
}
