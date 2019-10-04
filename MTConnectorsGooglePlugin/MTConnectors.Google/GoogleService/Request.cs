using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class Request
    {
        // this is the limit from Google, generally it should not  a problem for normal translation. 
        private const int RequestSizeLimit = (5 * 1024) - 128;
        private const int RequestTimeOut = 10 * 1000;

        public Request(string baseUri, RequestType rt)
        {
            BaseURI = baseUri;
            Method = rt;
            Fields = new List<RequestField>();
        }

        public string BaseURI { get; set; }

        public RequestType Method { get; set; }

        public List<RequestField> Fields { get; set; }

        public void AddField(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                throw new ArgumentNullException();
            }

            if (Fields == null)
            {
                Fields = new List<RequestField>();
            }

            Fields.Add(new RequestField(key, value));
        }


        public HttpWebResponse Send()
        {
            var uri = GetUri();

            var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
            if (httpWebRequest == null)
            {
                throw new Exception(PluginResources.EMSG_CannotCreateWebRequest);
            }

            httpWebRequest.Timeout = RequestTimeOut;

            // 10s for stream r/w as well
            httpWebRequest.ReadWriteTimeout = RequestTimeOut;
            httpWebRequest.Referer = URLs.SdlWebsite;

            if (WebRequest.DefaultWebProxy != null)
            {
                if (WebRequest.DefaultWebProxy.Credentials == null)
                {
                    WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                }
            }

            AnnotateRequest(httpWebRequest);
            return httpWebRequest.GetResponse() as HttpWebResponse;
        }

        private static string UrlEncode(string text)
        {
            return System.Web.HttpUtility.UrlEncode(text);
        }

        /// <summary>
        /// Returns the Uri - for GET requests, the Uri contains all the fields. For POST requests,
        /// the Uri only contains the Base URI. Caller must ensure to set the request's .Method
        /// correctly, or call <see cref="AnnotateRequest"/> on the HTTP request.
        /// </summary>
        /// <returns></returns>
        private Uri GetUri()
        {
            switch (Method)
            {
                case RequestType.Get:
                    {
                        var sb = new StringBuilder(BaseURI);

                        sb.Append("?");
                        if (Fields != null && Fields.Count > 0)
                        {
                            bool first = true;
                            foreach (RequestField rf in Fields)
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                else
                                {
                                    sb.Append("&");
                                }

                                sb.AppendFormat("{0}={1}", rf.Key, UrlEncode(rf.Value));
                            }
                        }

                        return new Uri(sb.ToString());
                    }

                case RequestType.Post:
                    {
                        return new Uri(BaseURI);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        private void AnnotateRequest(System.Net.HttpWebRequest httpRequest)
        {
            switch (Method)
            {
                case RequestType.Get:
                    httpRequest.Method = "GET";
                    break;
                case RequestType.Post:
                    {
                        System.Diagnostics.Debug.Assert(Method == RequestType.Post);

                        // setup protocol and header
                        httpRequest.Method = "POST";
                        httpRequest.ContentType = "application/x-www-form-urlencoded";
                        httpRequest.Headers["X-HTTP-Method-Override"] = "GET";

                        // prepare the data to send
                        var sb = new StringBuilder();
                        foreach (var pair in Fields)
                        {
                            sb.AppendFormat("{0}={1}&", UrlEncode(pair.Key), UrlEncode(pair.Value));
                        }

                        if (sb.Length > 0)
                        {
                            sb.Length--;
                        }

                        byte[] data = Encoding.ASCII.GetBytes(sb.ToString());

                        if (data.Length > RequestSizeLimit)
                        {
                            throw new ApplicationException(PluginResources.Google_Segment_Too_Big);
                        }

                        httpRequest.ContentLength = data.Length;
                        using (var stream = httpRequest.GetRequestStream())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }

                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
