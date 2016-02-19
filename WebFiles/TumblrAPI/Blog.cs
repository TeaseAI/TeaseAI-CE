using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    [Serializable]
    [DataContract]
    public class Blog
    {
        string title;
        string name;
        ulong posts;
        string description;

        [DataMember(Name = "title")]
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
        
        [DataMember(Name = "posts")]
        public ulong Posts
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

        [DataMember(Name = "description")]
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }
    }
}
