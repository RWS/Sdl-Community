using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using TMProvider;

namespace TradosPlugin
{
    public class ExceptionHelper
    {
        public enum ExceptionTypes
        {
            TimeOut,
            Cas,
            Lookup
        }

        private static List<ExceptionTypes> hideExceptionList = new List<ExceptionTypes>();

        public static string GetExceptionMessage(Exception e, string providerName = "")
        {
            if (e is TimeoutException || e is TokenTimedOutException)
            {
                return PluginResources.Error_TimeOutExceptionText;
            }
            else if (e is System.ServiceModel.ProtocolException)
            {
                return PluginResources.Error_ProtocolException;
            }
            else if ( e is AuthenticationException)
            {
                return String.Format(PluginResources.Error_InvalidCredentialsText, providerName);
            }
            else if (e is System.ServiceModel.FaultException)
            {
                return PluginResources.Error_General + "\n" + e.Message; // +"\n" + e.StackTrace;
            }
            else if (e is LookupException)
            {
                return PluginResources.Error_Lookup;
            }
            else if (e is AggregateException)
            {
                return e.Message;
            }
            else if (e is TooFrequentLoginException)
            {
                return PluginResources.Error_TooFrequentLogin;
            }
            else if (e is NoLicenseException)
            {
                return String.Format(PluginResources.Error_NoLicense, providerName);
            }
            else if (e is ReverseLookupException)
            {
                return String.Format(PluginResources.Error_NoReverselookup, providerName);
            }
            else if (e is TMNotFoundException)
            {
                string tmName = e.Data.Contains("tm") ? e.Data["tm"].ToString() : "";
                return String.Format(PluginResources.Error_TMNotFound, tmName, providerName);
            }
            else if (e is UnauthorizedTMReadException)
            { 
                string tmName = e.Data.Contains("tm") ? e.Data["tm"].ToString() : "";
                return String.Format(PluginResources.Error_UnauthorizedTMRead, tmName, providerName);
            }
            else if (e is UnauthorizedTMWriteException)
            {
                string tmName = e.Data.Contains("tm") ? e.Data["tm"].ToString() : "";
                return String.Format(PluginResources.Error_UnauthorizedTMWrite, tmName, providerName);
            }
            else if (e is GeneralServerException || e is ServerException)
            {
                return String.Format(PluginResources.Error_GeneralServerError + "\n" + e.Message, providerName);
            }
            else if (e is HttpRequestException)
            {
                if (e.InnerException is WebException)
                {
                    return GetExceptionMessage(e.InnerException);
                }
                else if (e.InnerException is System.IO.IOException)
                {
                    return GetExceptionMessage(e.InnerException);
                }
                else return e.Message;
            }
            else if (e.InnerException is WebException)
            {
                if (e.InnerException is System.Net.Sockets.SocketException)
                {
                    return GetExceptionMessage(e.InnerException);
                }
                if (e.InnerException is System.IO.IOException)
                {
                    return GetExceptionMessage(e.InnerException);
                }
                else return String.Format(PluginResources.Error_CantAccessServer, providerName);
            }
            if (e.InnerException is System.Net.Sockets.SocketException)
            {
                return String.Format(PluginResources.Error_CantAccessServer, providerName) + " " + PluginResources.Error_CheckPort;
            }
            else if (e.InnerException is System.IO.IOException)
            {
                return String.Format(PluginResources.Error_CantAccessServer, providerName) + " " + PluginResources.Error_CheckPort;
            }
            else if (e is UriFormatException)
            {
                return PluginResources.Error_URIFormat;
            }
            else
            {
                if (!String.IsNullOrEmpty(providerName)) return String.Format(PluginResources.Error_GeneralServerError, providerName) + "\n" + e.Message; // + "\n" + e.StackTrace;
                else return PluginResources.Error_General + "\n" + e.Message; // +"\n" + e.StackTrace;

            }
        }

        /// <summary>
        /// Creates one message from the messages of all the exceptions.
        /// </summary>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public static string GetAllExceptionsMessage(List<Exception> exceptions, string providerName = "")
        {
            if (exceptions == null || exceptions.Count == 0) return "";
            // to filter out identical messages create a list
            List<string> messages = new List<string>();
            foreach (Exception e in exceptions)
            {
                // these messages will be ignored
                if (e is TokenTimedOutException || e is TimeoutException || e is ReverseLookupException) continue;

                string m = GetExceptionMessage(e, providerName);
                if (!messages.Contains(m)) messages.Add(m);
            }

            StringBuilder sb = new StringBuilder();
            foreach (string m in messages)
            {
                sb.Append(m + "\n");
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static void ShowMessage(string message)
        {
            MessageBox.Show(message, PluginResources.Error_PluginError);
        }

        public static void WriteExceptionsToLog(List<Exception> exceptions, string providerName = "")
        {
            foreach (Exception e in exceptions)
            {
                string m = providerName + " " + GetExceptionMessage(e, providerName) + " " + (e.Data.Contains("stack") ? e.Data["stack"].ToString() : "");
                Log.WriteToLog(m);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="messageParam">The parameter we want to add to the message (for placeholders). Eg. provider name.</param>
        public static void ShowMessage(Exception e, string messageParam = "")
        {
            string message = GetExceptionMessage(e, messageParam);

            if (e is TimeoutException)
            {
                if (!hideExceptionList.Contains(ExceptionTypes.TimeOut))
                {
                    if (ShowMsgWithHideOption(PluginResources.Error_Error, message)) hideExceptionList.Add(ExceptionTypes.TimeOut);
                }
            }
            else if (e is LookupException)
            {
                if (!hideExceptionList.Contains(ExceptionTypes.Lookup))
                {
                    if (ShowMsgWithHideOption(PluginResources.Error_Error, PluginResources.Error_TimeOutExceptionText))
                        hideExceptionList.Add(ExceptionTypes.Lookup);
                }
            }
            else if (e is AggregateException)
            {
                AggregateException ae = e as AggregateException;
                ae.Handle(handleAggregateException);
            }
            else if (e is HttpRequestException)
            {
                if (e.InnerException is WebException)
                {
                    MessageBox.Show(GetExceptionMessage(e.InnerException), PluginResources.Error_Error);
                }
                else MessageBox.Show(e.Message + "\n" + e.StackTrace, PluginResources.Error_Error);
            }
            else
            {
                MessageBox.Show(message, PluginResources.Error_Error);
            }
        }

        public static void ShowMessages(List<Exception> exceptions, string messageParam)
        {
            foreach (Exception ex in exceptions)
            {
                ShowMessage(ex, messageParam);
            }
        }

        private static bool handleAggregateException(Exception e)
        {
            ShowMessage(e);
            return true;
        }

        public static bool ShowMsgWithHideOption(string title, string message)
        {
            using (MessageBoxWithCheckboxForm msg = new MessageBoxWithCheckboxForm(title, message))
            {
                msg.ShowDialog();
                return msg.DontShowAgain;
            }
        }

        public static bool ShowMsgWithHideOption(string title, string message, string linkText, string linkURL, bool showChk)
        {
            using (MessageBoxWithCheckboxForm msg = new MessageBoxWithCheckboxForm(title, message, linkText, linkURL, showChk))
            {
                msg.ShowDialog();
                return msg.DontShowAgain;
            }
        }
    }
}
