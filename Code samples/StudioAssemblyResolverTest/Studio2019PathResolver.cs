using Rws.StudioAssemblyResolver.PathResolver;

namespace StudioAssemblyResolverTest
{
    public class Studio2021PathResolver : IPathResolver
    {
        public string Resolve() => "C:\\Program Files (x86)\\SDL\\SDL Trados Studio\\Studio16";
    }
}