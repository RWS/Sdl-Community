using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trados.Interop.TMAccess;

namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    public class TP2007Pull: IDisposable
    {
        private Dictionary<string, TranslationMemoryClass> providersDictionary = new Dictionary<string, TranslationMemoryClass>();

        private bool disposed = false;

        public TranslationMemoryClass GetProvider(string Location, string UserId, tmaTmAccessMode Mode, string Password = null, int TargetLocale = 0)
        {
            if (providersDictionary.ContainsKey(Location))
            {
                return providersDictionary[Location];
            }
            else
            {
                var tradosProvider = new TranslationMemoryClass();

                // Windows authentication allows this fields be as empty strings
                if (UserId == null)
                    UserId = "";
                if (Password == null)
                    Password = "";

                tradosProvider.Open(
                    Location, UserId,
                    Mode, Password, TargetLocale);

                providersDictionary.Add(Location, tradosProvider);

                return tradosProvider;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    foreach (var pair in providersDictionary)
                    {
                        pair.Value.Close(); //Should we really invoke Close method?
                    }
                }
            }
        }

        ~TP2007Pull()
        {
            this.Dispose(false);
        }
    }
}