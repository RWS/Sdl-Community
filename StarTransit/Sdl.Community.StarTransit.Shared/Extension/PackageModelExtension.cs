using Sdl.Community.StarTransit.Shared.Models;
using System.IO;
using System.Linq;

namespace Sdl.Community.StarTransit.Shared.Extension
{
    public static class PackageModelExtension
    {
        public static string GetTempProjectName(this PackageModel packageModel) =>
            Path.GetDirectoryName(packageModel.PathToPrjFile)?.Split('\\').LastOrDefault();
    }
}