using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    [DataContract]
    public class Photo
    {
        string caption;
        Size original_size;
        Size[] alt_sizes;

        [DataMember(Name = "original_size")]
        public Size Original_size
        {
            get
            {
                return original_size;
            }

            set
            {
                original_size = value;
            }
        }

        [DataMember(Name = "alt_sizes")]
        public Size[] Alt_sizes
        {
            get
            {
                return alt_sizes;
            }

            set
            {
                alt_sizes = value;
            }
        }

        [DataMember(Name = "caption")]
        public string Caption
        {
            get
            {
                return caption;
            }

            set
            {
                caption = value;
            }
        }

        [DataContract]
        public class Size
        {
            int width;
            int height;
            string url;

            [DataMember(Name = "width")]
            public int Width
            {
                get
                {
                    return width;
                }

                set
                {
                    width = value;
                }
            }

            [DataMember(Name = "height")]
            public int Height
            {
                get
                {
                    return height;
                }

                set
                {
                    height = value;
                }
            }

            [DataMember(Name = "url")]
            public string Url
            {
                get
                {
                    return url;
                }

                set
                {
                    url = value;
                }
            }
        }
    }
}
