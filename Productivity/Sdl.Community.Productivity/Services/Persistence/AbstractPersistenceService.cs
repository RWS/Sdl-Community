using System;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace Sdl.Community.Productivity.Services.Persistence
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

                    using (var stream = new FileStream(PersistencePath, FileMode.Create))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            string json1 = JsonConvert.SerializeObject(toSave, Formatting.Indented);

                            writer.Write(json1);
                        }
                    }
                }
                string json = JsonConvert.SerializeObject(toSave, Formatting.Indented);

                File.WriteAllText(PersistencePath, json);
            }
            catch (Exception exception)
            {
                _logger.DebugException(@"Error appeared when Save tracking info", exception);
                
            }
        }

        public T Load()
        {
            var result = new T();
            try
            {

                if (!File.Exists(PersistencePath)) return new T();
                var json = File.ReadAllText(PersistencePath);
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception exception)
            {
                _logger.DebugException(@"Error appeared when Load tracking info", exception);

            }
            return result;
        }
    }
}
