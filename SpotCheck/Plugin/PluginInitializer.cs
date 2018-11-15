#region "PluginInitializer"

using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.StudioInitializer.Sample
{    
    /// <summary>
    /// Implements a SDL Trados Studio application initializer which will keep track of the application startup and closing time.    
    /// </summary>
    [ApplicationInitializer]
    class PluginInitializer : IApplicationInitializer
    {
        private const int MinimumWorkTime = 4;
        
        /// <summary>
        /// This method is executed when SDL Trados Studio application is starting.
        /// </summary>
        public void Execute()
        {
            StudioTracking.Start();

            // Setting up a check at SDL Trados Studio application closure verifying if the user has worked less then the minimum amount of time 
            // If the time passed since Studio opening and Studio closing is less than the MinimumWorkTime(4h) than
            // request a confirmation from the user for the application closure otherwise just exit.
            SdlTradosStudio.Application.Closing += (s, e) =>
                {
                    TimeSpan elapsedTime = StudioTracking.Elapsed;
                    if (elapsedTime.Hours < MinimumWorkTime)
                    {
                        DialogResult dialogResult =
                            MessageBox.Show(
                                string.Format("You have been working for {0:dd\\.hh\\:mm\\:ss} and spending less than {1} hours. Are you sure you want to quit Trados Studio?", StudioTracking.Elapsed, MinimumWorkTime)
                                , string.Format("Total work time is {0} minutes", elapsedTime.Minutes)
                                , MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult == DialogResult.No)
                        {
                            //Cancel the SDL Trados Studio application closing
                            e.Cancel = true;
                            return;
                        }
                    }
                    StudioTracking.Stop();
                };
        }
    }    
}

#endregion