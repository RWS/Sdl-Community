using Rws.StudioAssemblyResolver.PathResolver;

namespace StudioAssemblyResolverTest
{
    public class Studio2022PathResolver : IPathResolver
    {
        public string Resolve() => "C:\\Program Files (x86)\\Trados\\Trados Studio\\Studio17";
    }
}