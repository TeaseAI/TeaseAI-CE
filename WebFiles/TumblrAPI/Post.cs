using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    [DataContract]
    public class Post
    {
        Photo[] photos;
        string[] tags;
        string video_url;
        string type;
        string video_type;

        [DataMember(Name = "photos", IsRequired = false, EmitDefaultValue = false)]
        public Photo[] Photos
        {
            get
            {
                return photos;
            }

            set
            {
                photos = value;
            }
        }

        [DataMember(Name = "tags", IsRequired = false, EmitDefaultValue = false)]
        public string[] Tags
        {
            get
            {
                return tags;
            }

            set
            {
                tags = value;
            }
        }

        [DataMember(Name = "video_url", IsRequired = false, EmitDefaultValue = false)]
        public string Video_url
        {
            get
            {
                return video_url;
            }

            set
            {
                video_url = value;
            }
        }

        [DataMember(Name = "type")]
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        [DataMember(Name = "video_type", IsRequired = false, EmitDefaultValue = false)]
        public string Video_type
        {
            get
            {
                return video_type;
            }

            set
            {
                video_type = value;
            }
        }
    }
}
