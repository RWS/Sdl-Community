using antido32Lib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        private IAntidoteClient _antidoteClient;

        public AntidoteApiOle(IAntidoteClient antidoteClient)
        {
            _antidoteClient = antidoteClient;
        }

        public void CallAntidote(string parameter)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(CallAntidoteInternal));
            thread.Start(parameter);
        }

        private void CallAntidoteInternal(object parameter)
        {
            ApiOle api = GetAntidoteInstance();
            if(api != null)
            {
                api.LanceOutilDispatch(_antidoteClient, (string)parameter);
            }
        }

        private ApiOle GetAntidoteInstance()
        {
            IntPtr hwndAntidote;
            if(IsAntidoteRuning(out hwndAntidote))
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
            RegistryKey pRegKey = Registry.LocalMachine;
            pRegKey = pRegKey.OpenSubKey(Constants.RegistryInstallLocation);
            object obj = pRegKey.GetValue(Constants.RegistryInstallLocationValue, "Invalide");
            if (obj.ToString() == "Invalide")
            {
                //TODO message if the registry values could not be find - this should be displayed
                //to the user
                return false;
            }
            string antidotePath = obj.ToString();
            if (!antidotePath.EndsWith("\\")) antidotePath += "\\";
            antidotePath += Constants.AntidoteExecutable;
            System.Diagnostics.Process.Start(antidotePath, Constants.ApiAntidote);

            // get a hold of the newly launched Antidote instance
            long i = 0;
            IntPtr hwndAntidote;
            
            while (!IsAntidoteRuning(out hwndAntidote))
            {
                System.Threading.Thread.Sleep(250);
                i++;
                // limit the number of attempts
                if (i > 48)
                {
                    //TODO display a message to the user if this didn't work and we could find the window
                    //probably didn't launched
                    return false;
                }
            }

            return true;
        }
    }

}
