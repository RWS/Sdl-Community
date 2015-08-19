using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using NLog;

namespace Sdl.Community.YourProductivity.Services.Persistence
{
    public abstract class AbstractPersistenceService<T> where T : new()
    {
        private readonly Logger _logger;

        public abstract string PersistencePath { get; }

        protected AbstractPersistenceService(Logger logger)
        {
            _logger = logger;

        }

        public void Save(T toSave)
        {
            try
            {

                if (!File.Exists(PersistencePath))
                {
                    var directory = Path.GetDirectoryName(PersistencePath);
                    if (directory != null && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                using (var stream = new FileStream(PersistencePath, FileMode.OpenOrCreate))
                {
                    byte[] entropy = GetEntropy();
                    string json1 = JsonConvert.SerializeObject(toSave, Formatting.Indented);
                    var toEncrypt = Encoding.ASCII.GetBytes(json1);
                    byte[] encryptedData = ProtectedData.Protect(toEncrypt, entropy, DataProtectionScope.CurrentUser);

                    stream.Write(encryptedData, 0, encryptedData.Length);
                }
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when Save tracking info");

            }
        }

        public T Load()
        {
            var result = new T();
            try
            {

                if (!File.Exists(PersistencePath)) return new T();
                var encryptedData = File.ReadAllBytes(PersistencePath);
                var rawData = ProtectedData.Unprotect(encryptedData, GetEntropy(), DataProtectionScope.CurrentUser);

                var json = Encoding.ASCII.GetString(rawData);
                
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when Load tracking info");

            }
            return result;
        }

        private byte[] GetEntropy()
        {
            return Encoding.ASCII.GetBytes("Sdl Studio Productivity");
        }
    }
}
