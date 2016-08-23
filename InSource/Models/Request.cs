using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.InSource.Models
{
   public class Request
    {
       public List<ProjectRequest> ProjectRequest { get; set; }
       public TimerModel Timer { get; set; }
       public bool DeleteFolders { get; set; }
    
    }
}
