using System;
using System.IO;
using RestSharp;

namespace Sdl.Community.YourProductivity.Services
{
    public class EmailService
    {
        private readonly bool _enabled;
        public EmailService(bool enabled)
        {
            _enabled = enabled;
        }
        public void SendLogFile()
        {
            //    if (!_enabled) return;
            //    var client = new RestClient
            //    {
            //        BaseUrl = new Uri("https://api.mailgun.net/v3"),
            //        Authenticator = new HttpBasicAuthenticator("api",
            //            "key-408bd67495e848f19193808788a9b97f")
            //    };
            //    var request = new RestRequest();
            //    request.AddParameter("domain",
            //                         "sandbox3280ff42a045437d9d190757b3b5caf2.mailgun.org", ParameterType.UrlSegment);
            //    request.Resource = "sandbox3280ff42a045437d9d190757b3b5caf2.mailgun.org/messages";
            //    request.AddParameter("from", "cromica@gmail.com");
            //    request.AddParameter("to", "rocrisan@sdl.com");
            //    request.AddParameter("subject", "Log file");
            //    request.AddParameter("text", string.Format("An error has appeared on machine{0} used by {1}!", Environment.MachineName, Environment.UserName));
            //    request.AddFile("attachment", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //        @"SDL Community\Productivity\Log\community-productivity.log"));
            //    request.Method = Method.POST;
            //    client.ExecuteAsync(request, MailResponse);
        }

        //private void MailResponse(IRestResponse response, RestRequestAsyncHandle handle)
        //{
        //    //Do nothing if mail failed
        //}
    }
}
