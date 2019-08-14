using System;
using System.Collections.Generic;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.Services
{
    public class SearchCompletedEventArgs : EventArgs
    {
        public SearchCriteria Criteria { get; set; }

        public List<JobViewModel> JobSearchResults { get; set; }

    }
}
