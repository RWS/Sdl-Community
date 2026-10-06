using Sdl.Community.IATETerminologyProvider.Model;
using System.Collections.Generic;
using TradosStudio.API.Projects;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
    public interface ISqliteDatabaseProvider
    {
        void CloseConnection();
        void Connect(IProject project);
        List<SearchCache> Get();
        SearchCache Get(string sourceText, string targetLanguage, string queryString);
        int Insert(SearchCache searchCache);
        bool IsConnected();
        void RemoveAll();
        void Update(SearchCache searchCache);
    }
}