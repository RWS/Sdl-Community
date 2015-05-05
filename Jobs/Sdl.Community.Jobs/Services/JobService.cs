using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.Services
{
    public delegate void SearchCompletedEventHandler(object sender, SearchCompletedEventArgs e);
    public class JobService
    {
        public event SearchCompletedEventHandler SearchCompleted;

        protected virtual void OnSearchCompleted(SearchCompletedEventArgs e)
        {
            SearchCompletedEventHandler handler = SearchCompleted;
            if (handler != null) handler(this, e);
        }

        public JobService()
        {
            
        }

        public void Search(SearchCriteria criteria)
        {
            
            var results = new List<JobSearchResult>();
            //TODO implement search
            OnSearchCompleted(new SearchCompletedEventArgs {JobSearchResults = results, Criteria = criteria});

        }
    }
}
