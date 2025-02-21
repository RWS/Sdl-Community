using System.Collections.Generic;

namespace Sdl.Community.InSource.Models
{
   public class Request
    {
       public List<ProjectRequest> ProjectRequest { get; set; }
       public TimerModel Timer { get; set; }
       public bool DeleteFolders { get; set; }
    }
}
