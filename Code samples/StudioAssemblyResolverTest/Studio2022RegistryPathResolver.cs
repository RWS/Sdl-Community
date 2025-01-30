using Rws.StudioAssemblyResolver.PathResolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioAssemblyResolverTest
{
    public class RegistryStudio2022PathResolver : AbstractRegistryPathResolver
    {
        public override string GetStudioVersion() => "Studio17";
    }
}
