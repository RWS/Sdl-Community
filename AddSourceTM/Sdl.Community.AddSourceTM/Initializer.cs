using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.AddSourceTM.Source_Configurtion;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.AddSourceTM
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
