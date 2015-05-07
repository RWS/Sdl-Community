using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.Services
{
    public class SearchCompletedEventArgs : EventArgs
    {
        public SearchCriteria Criteria { get; set; }

        public List<JobViewModel> JobSearchResults { get; set; }

    }
}
