using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Interfaces
{
    public interface IStorageService
    {
        bool Exists(string filePath);
        T Load<T>(string filePath) where T : class;
        void Save<T>(string filePath, T data);
    }
}
