using System;

namespace Sdl.Community.IATETerminologyProvider.Model
{
    [Flags]
    public enum CheckState
    {
        AllNotChecked = 0,
        SomeChecked = 1,
        AllChecked = 2
    }
}