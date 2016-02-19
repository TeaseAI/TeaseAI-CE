using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            APIResponse response = new APIResponse();

            throw new NotImplementedException();

            return response;
        }
    }
}
