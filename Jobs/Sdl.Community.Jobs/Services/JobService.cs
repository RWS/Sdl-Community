using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.Jobs.API;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.Services
{
    public delegate void SearchCompletedEventHandler(object sender, SearchCompletedEventArgs e);
    public class JobService
    {
        private readonly List<Discipline> _disciplineList;
        private readonly List<Language> _languages;
        private readonly List<LanguageService> _languageServices;

        public int CurrentPage { get; set; }

        public List<Discipline> Disciplines
        {
            get { return _disciplineList; }
        }

        public List<Language> Languages
        {
            get { return _languages; }
        }

        public List<LanguageService> LanguageServices
        {
            get { return _languageServices; }
        }

        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }

        public event SearchCompletedEventHandler SearchCompleted;

        public JobService()
        {
            _disciplineList = JsonConvert.DeserializeObject<List<Discipline>>(PluginResources.discipline);
            _languages = JsonConvert.DeserializeObject<List<Language>>(PluginResources.languages);
            _languageServices = JsonConvert.DeserializeObject<List<LanguageService>>(PluginResources.languageServices);

        }

      

        private List<JobViewModel> TransformToJobsView(JobPostingResponse response)
        {
            if(response.JobPostings == null) return new List<JobViewModel>();
            return response.JobPostings.Select(jobPosting => new JobViewModel
            {
                Id = jobPosting.Id,
                Summary = jobPosting.Summary,
                Url = jobPosting.WebUrl,
                Description = jobPosting.Description,
                Discipline = _disciplineList.FirstOrDefault(x => x.DiscSpecId == jobPosting.DiscSpecId),
                LanguagePairs = GetLanguagePairs(jobPosting),
                LanguageServices = GetLanguageServices(jobPosting.LanguageServiceIds),
                OtherDiscipline = jobPosting.OtherDiscipline,
                TimePosted = GetTime(jobPosting),
                Type = jobPosting.Type,
                Volume = GetVolume(jobPosting)
            }).ToList();
        }

        private string GetTime(JobPosting jobPosting)
        {
            if (jobPosting.TimePosted.Day == DateTime.Now.Day)
            {
                return jobPosting.TimePosted.ToString("t");
            }
            else
            {
                return string.Format("{0} {1}", jobPosting.TimePosted.ToString("t"), jobPosting.TimePosted.ToString("m"));
            }
        }

        private string GetVolume(JobPosting jobPosting)
        {
            if (jobPosting.VolumeAmount == 0) return "No Amount";
            return string.Format("{0} {1}", jobPosting.VolumeAmount, jobPosting.VolumeUnit);
        }

        private string GetLanguagePairs(JobPosting jobPosting)
        {
            var languagePairs = jobPosting.LanguagePairs.Count > 0
                ? jobPosting.LanguagePairs.Select(lp => LanguagePair.CreateLanguagePair(lp, _languages))
                    .Where(lp => lp != null)
                    .ToList()
                : new List<LanguagePair>();

            var result = new StringBuilder();
            for (var i = 0; i < languagePairs.Count; i++)
            {
                var languagePair = languagePairs[i];
                if (i == languagePairs.Count - 1)
                {
                    result.Append(string.Format("{0} to {1}", languagePair.Source.LanguageName,
                   languagePair.Target.LanguageName));
                }
                else
                {
                    result.AppendLine(string.Format("{0} to {1}", languagePair.Source.LanguageName,
                   languagePair.Target.LanguageName));
                }
            }
            return result.ToString();
        }

        private string GetLanguageServices(List<int> languageServiceIds)
        {
            var langServices = languageServiceIds.Count == 0
                ? new List<LanguageService>()
                : languageServiceIds.Select(
                    languageServiceId => _languageServices.FirstOrDefault(x => x.Id == languageServiceId))
                    .Where(lgs => lgs != null)
                    .ToList();
            var result = new StringBuilder();
            for (int i = 0; i < langServices.Count; i++)
            {
                var langService = langServices[i];
                if (i == langServices.Count - 1)
                {
                    result.Append(langService.Name);
                    
                }
                else
                {
                    result.AppendLine(langService.Name);
                }
            }
            return result.ToString();
        }

        public void Search()
        {
            Search(null);
        }
        
        public void Search(SearchCriteria criteria, int page = 1)
        {
            var request = new RestRequest(Method.GET) { Resource = "job-postings", RequestFormat = DataFormat.Json };
            request.Parameters.AddRange(CreateParameters(criteria, page));

            var pApi = new ProzApi();

            var response = pApi.Execute<JobPostingResponse>(request);

            List<JobViewModel> results = TransformToJobsView(response);

            CurrentPage = page;

            HasNext = response.Links.Next != null;
            HasPrevious = CurrentPage > 1;


            OnSearchCompleted(new SearchCompletedEventArgs {JobSearchResults = results, Criteria = criteria});

        }

        private IEnumerable<Parameter> CreateParameters(SearchCriteria criteria, int page)
        {
            var result = new List<Parameter>();
            if (criteria == null) return result;

            if (criteria.Translation || criteria.Interpreting || criteria.Potential)
            {
                var value = new StringBuilder();
                if (criteria.Translation)
                {
                    value.Append(value.Length == 0 ? "translation" : ",translation");
                }
                if (criteria.Interpreting)
                {
                    value.Append(value.Length == 0 ? "interpret" : ",interpret");
                }
                if (criteria.Potential)
                {
                    value.Append(value.Length == 0 ? "potential" : ",potential");
                }
                var typeParameter = new Parameter
                {
                    Type = ParameterType.QueryString,
                    Value = value.ToString(),
                    Name = "type"
                };
                result.Add(typeParameter);
            }

            if (criteria.LanguagePair.Source.LanguageCode != "all" && criteria.LanguagePair.Target.LanguageCode != "all")
            {
                var languagePairParameter = new Parameter
                {
                    Type = ParameterType.QueryString,
                    Value = criteria.LanguagePair.Serialize(),
                    Name = "language_pairs"
                };

                result.Add(languagePairParameter);
            }

            if (criteria.Discipline.DiscSpecId != 0)
            {
                var disciplineParameter = new Parameter
                {
                    Type = ParameterType.QueryString,
                    Value = criteria.Discipline.DiscSpecId,
                    Name = "disc_spec_ids"
                };

                result.Add(disciplineParameter);
            }

            if (!string.IsNullOrEmpty(criteria.SearchTerm))
            {
                var searchParameter = new Parameter
                {
                    Type = ParameterType.QueryString,
                    Value = criteria.SearchTerm,
                    Name = "q"
                };

                result.Add(searchParameter);
            }

           
                var pageParameter = new Parameter
                {
                    Type = ParameterType.QueryString,
                    Value = page,
                    Name = "page"
                };

                result.Add(pageParameter);
            
            return result;
        }

        

        protected virtual void OnSearchCompleted(SearchCompletedEventArgs e)
        {
            SearchCompletedEventHandler handler = SearchCompleted;
            if (handler != null) handler(this, e);
        }
    }
}
