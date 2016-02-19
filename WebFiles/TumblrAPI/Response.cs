using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    [DataContract]
    public class Response
    {
        Blog blog;
        Post[] posts;
        string avatar_url;

        [DataMember(Name = "blog", IsRequired = false, EmitDefaultValue = false)]
        public Blog Blog
        {
            get
            {
                return blog;
            }

            set
            {
                blog = value;
            }
        }

        [DataMember(Name = "posts", IsRequired = false, EmitDefaultValue = false)]
        public Post[] Posts
        {
            get
            {
                return posts;
            }

            set
            {
                posts = value;
            }
        }

        [DataMember(Name = "avatar_url", IsRequired = false, EmitDefaultValue = false)]
        public string Avatar_url
        {
            get
            {
                return avatar_url;
            }

            set
            {
                avatar_url = value;
            }
        }
    }
}
