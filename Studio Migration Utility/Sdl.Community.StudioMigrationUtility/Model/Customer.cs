using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StudioMigrationUtility.Model
{
    [Serializable]
    public class Customer
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public Customer(Guid guid, string name, string email)
        {
            Guid = guid;
            Name = name;
            Email = email;
        }
    }

}
