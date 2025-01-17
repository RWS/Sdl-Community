using System;
using System.Net;

namespace Sdl.Community.PostEdit.Compare.Core.ActionsFromReport
{
    public static class Listener
    {
        private static Controller Controller { get; } = new();
        private static HttpListener HttpListener { get; } = new();

        public static void StartHttpListener()
        {
            HttpListener.Prefixes.Add("http://localhost:5000/");
            HttpListener.Start();
            HttpListener.BeginGetContext(OnRequestReceived, null);
        }

        private static void OnRequestReceived(IAsyncResult result)
        {
            try
            {
                var context = HttpListener.EndGetContext(result);
                HttpListener.BeginGetContext(OnRequestReceived, null);

                var request = context.Request;
                var response = context.Response;

                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                switch (request.HttpMethod)
                {
                    case "POST" when request.Url.AbsolutePath == "/navigate":
                        Controller.HandleNavigateRequest(request, response);
                        break;

                    case "OPTIONS":
                        Controller.HandleOptionsRequest(response);
                        break;

                    default:
                        Controller.HandleNotFound(response);
                        break;
                }

                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
        }
    }
}