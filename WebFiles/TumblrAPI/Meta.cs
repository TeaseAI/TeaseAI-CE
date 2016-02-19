using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    [DataContract]
    public class Meta
    {
        int status;
        string msg;

        [DataMember(Name = "status")]
        public int Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        [DataMember(Name = "msg")]
        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }
    }
}
