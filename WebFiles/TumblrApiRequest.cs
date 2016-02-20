using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using TeaseAI_CE.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    /// <summary>
    /// Represents a Request to the TumblrAPI
    /// </summary>
    public class TumblrApiRequest
    {
        string requestUrl;
        public TumblrApiRequest(string requestUrl)
        {
            this.RequestUrl = requestUrl;
        }

        /// <summary>
        /// URL to make the WebRequest to the API with
        /// </summary>
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

        /// <summary>
        /// Get the response from the TumblrAPI
        /// </summary>
        /// <returns>Response from TumblrAPI</returns>
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

        /// <summary>
        /// Get the response from the TumblrAPI asynchronosly
        /// </summary>
        /// <returns>Response from TumblrAPI</returns>
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
