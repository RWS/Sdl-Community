using System;
using System.IO;

namespace Sdl.Community.PostEdit.Compare.Core;

public class SharedStrings
{
    public static string FuzzyMatch = "Fuzzy Match";

    public static readonly string PostEditCompareSettingsFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore",
            "PostEdit.Compare");
}