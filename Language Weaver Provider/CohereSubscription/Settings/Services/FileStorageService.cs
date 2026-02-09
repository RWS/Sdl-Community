using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace LanguageWeaverProvider.CohereSubscription.Settings.Services
{
    public class FileStorageService : IStorageService
    {
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public T Load<T>(string filePath) where T : class
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Save<T>(string filePath, T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
