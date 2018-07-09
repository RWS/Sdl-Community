using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HockeyApp;

namespace Sdl.Community.StarTransit.Shared.Utils
{
    public class TelemetryService
    {
        private  static readonly Lazy<TelemetryService> _lazy = new Lazy<TelemetryService>(() => new TelemetryService());

        public TelemetryService()
        {
            var version = typeof (TelemetryService)
                .Assembly
                .GetName()
                .Version
                .ToString();
            HockeyClient.Current.Configure("455696958de74739b2922d67c796e0a3");
            var client = (HockeyClient) HockeyClient.Current;

            ((HockeyPlatformHelperWPF) client.PlatformHelper).AppVersion = version;
            client.UserID = Environment.UserName;

        }

        public void Init()
        {

        }
        public static TelemetryService Instance => _lazy.Value;

        public  void HandleException(Exception ex)
        {

            Task.Run(()=> ((HockeyClient)HockeyClient.Current).HandleExceptionAsync(ex)) ;
        }

        public async Task CheckForUpdates(bool checkForUpdates)
        {
            
            await HockeyClient.Current.CheckForUpdatesAsync(false, null, (newVersion) =>
            {
               var result= MessageBox.Show($@"A new version of the application is available {newVersion.Version}. Would you like to download this new version?", " ",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Process.Start(@"http://appstore.sdl.com/app/transitpackage-handler/573/");
                }
            });

        }

        public  void SendCrashes(bool sendCrashes)
        {
            Task.Run(()=> HockeyClient.Current.SendCrashesAsync(true));
        }
    }
}
