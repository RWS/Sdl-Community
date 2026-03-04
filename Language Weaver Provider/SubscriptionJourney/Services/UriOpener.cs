using LanguageWeaverProvider.Model.Interface;
using System.Diagnostics;

namespace LanguageWeaverProvider.SubscriptionJourney.Services
{
    class UriOpener : IUriOpener
    {
        public void OpenUri(string uri)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true
            });
        }
    }
}
