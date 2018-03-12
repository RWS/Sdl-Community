using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.Trados2007.UI.ServersDialog
{
    class ServersFormModel
    {
        private IList<Trados2007ServerAccount> servers;
        
        public IList<Trados2007ServerAccount> Servers 
        {
            get
            {
                if (this.servers == null)
                {
                    this.servers = new List<Trados2007ServerAccount>();
                }
                return this.servers;
            }
            set
            {
                this.servers = value;
            }
        }

        public ServersFormModel(IList<Trados2007ServerAccount> servers)
        {
            // TODO: Complete member initialization
            this.Servers = servers;
        }

        internal string GetServerStatus(Trados2007ServerAccount el)
        {
            return el.IsServerUp ? "Available" : "Not available";
        }
    }
}
