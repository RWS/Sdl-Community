using System;

namespace Sdl.Community.StarTransit.UI.Interfaces
{
	public interface IWindowActions
    {
        Action CloseAction { get; set; }
        Action<string, string> ShowWindowsMessage { get; set; }
    }
}