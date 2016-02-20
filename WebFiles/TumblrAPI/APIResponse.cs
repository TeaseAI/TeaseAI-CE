using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    /// <summary>
    /// Root of response from a TumblrApiRequest
    /// </summary>
    [DataContract]
    public class APIResponse
    {
        Meta meta;
        Response response;

        [DataMember(Name = "meta")]
        public Meta Meta
        {
            get
            {
                return meta;
            }

            set
            {
                meta = value;
            }
        }

        [DataMember(Name = "response")]
        public Response Response
        {
            get
            {
                return response;
            }

            set
            {
                response = value;
            }
        }
    }
}
