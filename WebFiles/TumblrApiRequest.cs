using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using TeaseAI_CE.Serialization;

namespace WebFiles.TumblrAPI
{
    public class TumblrApiRequest
    {
        string requestUrl;
        public TumblrApiRequest(string requestUrl)
        {
            this.RequestUrl = requestUrl;
        }

        public string RequestUrl
        {
            get
            {
                return requestUrl;
            }

            protected set
            {
                requestUrl = value;
            }
        }

        public APIResponse GetResponse()
        {
            APIResponse apiResponse = new APIResponse();
            
            WebRequest request;
            WebResponse response;
            StreamReader sr;
            string responseStr;
            
            request = WebRequest.Create(this.RequestUrl);

            response = request.GetResponse();
            sr = new StreamReader(response.GetResponseStream());

            responseStr = sr.ReadToEnd();
            responseStr = responseStr.Replace("\\/", "/");

            apiResponse = Serializer.DeserializeFromJson<APIResponse>(responseStr);

            return apiResponse;
        }

        public async Task<APIResponse> GetResponseAsync()
        {
            APIResponse apiResponse = new APIResponse();

            WebRequest request;
            WebResponse response;
            StreamReader sr;
            string responseStr;

            request = WebRequest.Create(this.RequestUrl);

            response = await request.GetResponseAsync();
            sr = new StreamReader(response.GetResponseStream());

            responseStr = await sr.ReadToEndAsync();
            responseStr = responseStr.Replace("\\/", "/");

            apiResponse = Serializer.DeserializeFromJson<APIResponse>(responseStr);

            return apiResponse;
        }
    }
}
