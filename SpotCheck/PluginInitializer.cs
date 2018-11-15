using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.ProjectAutomation.Core;

using Sdl.Studio.SpotCheck.Helpers;
using Sdl.Studio.SpotCheck.SdlXliff;

namespace Sdl.Studio.SpotCheck
{    
    /// <summary>
    /// Implements a SDL Trados Studio application initializer. 
    /// </summary>
    [ApplicationInitializer]
    class PluginInitializer : IApplicationInitializer
    {
        public static List<string> FilesWithSpotcheckMarkers = new List<string>();

        /// <summary>
        /// This method is executed when SDL Trados Studio application is starting.
        /// </summary>
        public void Execute()
        {
            SdlTradosStudio.Application.Closing += Application_Closing;
        }

        void Application_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // todo: crashes Studio when something is open in the editor, disabled for now

//            if (FilesWithSpotcheckMarkers.Count > 0)
            if (false)
                {
                dlgSpotcheckWarning warning = new dlgSpotcheckWarning();
                warning.FillList(FilesWithSpotcheckMarkers);
                DialogResult dr = warning.ShowDialog();
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }    
}
