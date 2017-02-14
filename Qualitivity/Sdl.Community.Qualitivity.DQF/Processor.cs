using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Sdl.Community.DQF.Core;

namespace Sdl.Community.DQF
{
    public class Processor
    {

        public List<T> GetDqfListItems<T>(Uri uri)
        {
            var listItems = new List<T>();

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.KeepAlive = false;
            webRequest.Method = @"GET";
            webRequest.ContentLength = 0;
            webRequest.Proxy = null;
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            var result = string.Empty;
            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse != null)
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    result = reader.ReadToEnd();
                }
            }

            if (result.Trim()!=string.Empty)
                listItems = JsonHelper.JsonDeserialize<List<T>>(result);


            return listItems;
        }

        //http://dqf.taus.net/api/v1/productivityProject?name=My+first+DQF+productivity+project&quality_level=1&process=2&source_language=en-US&contentType=1&industry=1

        public ProductivityProject PostDqfProject(string dqfPmanagerKey, ProductivityProject productivityProject)
        {


            var url = Configuration.DqfServerRoot + Configuration.DqfApiVersion;
            url += "productivityProject?";
            url += "name=" + productivityProject.Name.Replace(" ", "+");
            url += "&quality_level=" + productivityProject.QualityLevel;
            url += "&process=" + productivityProject.Process;
            url += "&source_language=" + productivityProject.SourceLanguage;
            url += "&contentType=" + productivityProject.ContentType;
            url += "&industry=" + productivityProject.Industry;


            var uri = new Uri(url);

            //Headers = {Access-Control-Allow-Headers: Content-Type, api_key, Authorization
            //Access-Control-Allow-Methods: GET, POST, DELETE, PUT
            //Access-Control-Allow-Origin: *
            //project_id: 1565
            //project_key: 8teu3289m3vmrb6jcvkq39g9g83u0ofqn67lvrobaumrqv0jp1l4
            //Connection: close...


            //Headers	{Access-Control-Allow-Headers: Content-Type, api_key, Authorization
            //Access-Control-Allow-Methods: GET, POST, DELETE, PUT
            //Access-Control-Allow-Origin: *
            //project_id: 1566
            //project_key: 4mrnet0dr5e7aemghps07rqbnvr2al01vanuv8c54vjnp6bni4rc
            //Connection: close
            //Content-Length: 79
            //Content-Type: application/json
            //Date: Sat, 30 May 2015 21:36:52 GMT
            //Server: Apache



            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.KeepAlive = false;
            webRequest.Method = @"POST";
            webRequest.ContentLength = 0;
            webRequest.Proxy = null;
            webRequest.Headers.Add("DQF_PMANAGER_KEY", dqfPmanagerKey);
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse == null) return productivityProject;
                productivityProject.ProjectId = Convert.ToInt32(webResponse.Headers["project_id"]);
                productivityProject.ProjectKey = webResponse.Headers["project_key"];

                var reader = new StreamReader(webResponse.GetResponseStream());
                reader.ReadToEnd();
            }

            return productivityProject;

        }

        public ProductivityProjectTask PostDqfProjectTask(ProductivityProjectTask productivityProjectTask)
        {
            var url = Configuration.DqfServerRoot + Configuration.DqfApiVersion;
            url += "productivityProject/"+productivityProjectTask.Projectid+"/task?";
            url += "target_language=" + productivityProjectTask.TargetLanguage;
            url += "&file_name=" + productivityProjectTask.FileName.Replace(" ", "+");
            

            var uri = new Uri(url);

            //Headers	{Access-Control-Allow-Headers: Content-Type, api_key, Authorization
            //Access-Control-Allow-Methods: GET, POST, DELETE, PUT
            //Access-Control-Allow-Origin: *
            //task_id: 3737
            //Connection: close
            //Content-Length: 76
            //Content-Type: application/json
            //Date: Sat, 30 May 2015 21:57:38 GMT
            //Server: Apache

            //}	System.Net.WebHeaderCollection
    

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.KeepAlive = false;
            webRequest.Method = @"POST";
            webRequest.ContentLength = 0;
            webRequest.Proxy = null;
            //webRequest.Headers.Add("DQF_PMANAGER_KEY", productivityProjectTask.DQF_PMANAGER_KEY);
            webRequest.Headers.Add("DQF_PROJECT_KEY", productivityProjectTask.DqfProjectKey);
            webRequest.Headers.Add("DQF_TRANSLATOR_KEY", productivityProjectTask.DqfTranslatorKey);

            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse == null) return productivityProjectTask;
                productivityProjectTask.ProjectTaskId = Convert.ToInt32(webResponse.Headers["task_id"]);
                   
                var reader = new StreamReader(webResponse.GetResponseStream());
                reader.ReadToEnd();
            }

            return productivityProjectTask;

        }

        public ProjectTaskSegment PostDqfProjectTaskSegment(ProjectTaskSegment projectTaskSegment)
        {
            //System.Web.HttpContext.Current.Server.UrlEncode

           

            var url = Configuration.DqfServerRoot + Configuration.DqfApiVersion;
            url += "productivityProject/" + projectTaskSegment.Projectid + "/task/" + projectTaskSegment.Taskid + "/segment?";
            url += "source_segment=" + Uri.EscapeDataString(projectTaskSegment.SourceSegment);
            url += "&target_segment=" + Uri.EscapeDataString(projectTaskSegment.TargetSegment);
            url += "&new_target_segment=" + Uri.EscapeDataString(projectTaskSegment.NewTargetSegment);
            url += "&time=" + projectTaskSegment.Time;
            url += "&tm_match=" + projectTaskSegment.TmMatch;
            url += "&cattool=" + projectTaskSegment.Cattool;
            url += "&mtengine=" + projectTaskSegment.Mtengine;
            url += "&mt_engine_version=" + "";

            var uri = new Uri(url);

            
            //Headers	{Access-Control-Allow-Headers: Content-Type, api_key, Authorization
            //Access-Control-Allow-Methods: GET, POST, DELETE, PUT
            //Access-Control-Allow-Origin: *
            //task_id: 3737
            //Connection: close
            //Content-Length: 76
            //Content-Type: application/json
            //Date: Sat, 30 May 2015 21:57:38 GMT
            //Server: Apache

            //}	System.Net.WebHeaderCollection


            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.KeepAlive = false;
            webRequest.Method = @"POST";
            webRequest.ContentLength = 0;
            webRequest.Proxy = null;
            //webRequest.Headers.Add("DQF_PMANAGER_KEY", productivityProjectTask.DQF_PMANAGER_KEY);
            webRequest.Headers.Add("DQF_TRANSLATOR_KEY", projectTaskSegment.DqfTranslatorKey);
            webRequest.Headers.Add("DQF_PROJECT_KEY", projectTaskSegment.DqfProjectKey);
           

            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse == null) return projectTaskSegment;
                projectTaskSegment.Segmentid = webResponse.Headers["segment_id"];

                var reader = new StreamReader(webResponse.GetResponseStream());
                reader.ReadToEnd();
            }

            return projectTaskSegment;

        }

        public bool SetupTranslatorInfo(Uri uri, string dqfPmanagerKey)
        {
            var success = false;
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.KeepAlive = false;
            webRequest.Method = @"POST";
            webRequest.ContentLength = 0;
            webRequest.Proxy = null;
            //webRequest.Headers.Add("DQF_PMANAGER_KEY", DQF_PMANAGER_KEY);


            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729;)";
            string result = null;
            using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse != null)
                {
                    var reader = new StreamReader(webResponse.GetResponseStream());
                    result = reader.ReadToEnd();
                }
            }
            //"{\"code\":200,\"type\":\"ok\",\"message\":\"The translator was succefully added to the database\"}"
            if (result != null && result.ToLower().IndexOf("\"code\":200", StringComparison.Ordinal) > -1 && result.ToLower().IndexOf("\"type\":\"ok\"", StringComparison.Ordinal) > -1)
                success = true;

            return success;
          

        }
    }
}
