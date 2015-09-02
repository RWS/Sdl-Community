using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AddSourceTM.Source_Configurtion
{
    public class AddSourceTmConfigurations
    {
        public AddSourceTmConfiguration Default
        {
            get
            {
                return new AddSourceTmConfiguration()
                {
                    TmSourceFieldName = "Source File",
                    StoreFullPath = true
                };
            }
        }

        public List<AddSourceTmConfiguration> Configurations { get; set; }
    }
}
