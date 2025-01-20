using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace Sdl.Community.PostEdit.Compare.Core.ActionsFromReport
{
    public class DataHandler
    {
        public JObject GetData(Stream requestInputStream)
        {
            using var reader = new StreamReader(requestInputStream);
            var requestBody = reader.ReadToEnd();
            var data = JObject.Parse(requestBody);
            return data;
        }

        public void WriteResponse(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            using var writer = new StreamWriter(response.OutputStream);
            writer.Write(message);
        }
    }
}