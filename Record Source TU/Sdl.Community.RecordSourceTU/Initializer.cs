using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.RecordSourceTU
{
    [ApplicationInitializer]
    public class Initializer : IApplicationInitializer
    {
        public void Execute()
        {
            var persistance = new Persistance();

            var addSourceTmConfiguration = persistance.Load();
            if (addSourceTmConfiguration != null) return;

            persistance.Save(new AddSourceTmConfigurations() {Configurations = new List<AddSourceTmConfiguration>()});
        }
    }
}
