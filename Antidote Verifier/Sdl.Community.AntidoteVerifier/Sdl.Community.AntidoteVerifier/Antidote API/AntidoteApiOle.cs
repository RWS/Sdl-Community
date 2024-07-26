using antido32Lib;
using Microsoft.Win32;
using Serilog;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Sdl.Community.AntidoteVerifier.Antidote_API
{
    sealed public class AntidoteApiOle
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern void BringWindowToTop(IntPtr hwnd);

        private readonly IAntidoteClient _antidoteClient;

        public AntidoteApiOle(IAntidoteClient antidoteClient)
        {
            _antidoteClient = antidoteClient;
        }

        public void CallAntidote(string parameter)
        {
            var thread = new Thread(CallAntidoteInternal);
            thread.Start(parameter);
        }

        private void CallAntidoteInternal(object parameter)
        {
            try
            {
                var api = GetAntidoteInstance();
	            api?.LanceOutilDispatch(_antidoteClient, (string)parameter);
            }catch(Exception ex)
            {
                Log.Error(ex, "An error appeared while starting antidote!");
                MessageBox.Show(ex.Message, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
        }

        private ApiOle GetAntidoteInstance()
        {
	        if(IsAntidoteRuning(out var hwndAntidote))
            {
                SetForegroundWindow(hwndAntidote);
                BringWindowToTop(hwndAntidote);
            }
            else
            {
                if (!LaunchAntidote()) return null;
            }

            return (ApiOle)Marshal.GetActiveObject(Constants.ProgIDAntidoteApiOle);
        }

        private bool IsAntidoteRuning(out IntPtr hwndAntidote)
        {
            hwndAntidote = FindWindow(Constants.AntidoteApp, null);
            return hwndAntidote.ToInt32() != 0;
        }

	    private bool LaunchAntidote()
	    {
		    try
		    {
			    var pRegKey = Registry.LocalMachine;
			    pRegKey = pRegKey.OpenSubKey(Constants.RegistryInstallLocation);
			    var obj = pRegKey?.GetValue(Constants.RegistryInstallLocationValue, "Invalide");
			    if (obj is null || obj.ToString() == "Invalide")
			    {
				    MessageBox.Show(PluginResources.AntidoteRegistryKeyNotFound_Error, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				    return false;
			    }
			    var antidotePath = obj?.ToString();
			    if (!antidotePath.EndsWith("\\")) antidotePath += "\\";
			    antidotePath += Constants.AntidoteExecutable;
			    System.Diagnostics.Process.Start(antidotePath, Constants.ApiAntidote);

			    // get a hold of the newly launched Antidote instance
			    long i = 0;

			    while (!IsAntidoteRuning(out _))
			    {
				    Thread.Sleep(250);
				    i++;
				    // limit the number of attempts
				    if (i > 48)
				    {
						MessageBox.Show(PluginResources.AntidoteCannotBeStarted_Error, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
				    }
			    }
		    }
		    catch (Exception ex)
		    {
			    Log.Error(ex, "An error appeared while starting antidote!");
			    MessageBox.Show(ex.Message, PluginResources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
		    }

		    return true;
	    }
    }

}
