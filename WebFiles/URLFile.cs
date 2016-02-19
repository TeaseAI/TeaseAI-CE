using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaseAI_CE.WebFiles
{
    [Serializable]
    public class URLFile
    {
        string avatarUrl;
        private List<string> photoUrls = new List<string>();
        private List<string> videoUrls = new List<string>();

        public string AvatarUrl
        {
            get
            {
                return avatarUrl;
            }

            set
            {
                avatarUrl = value;
            }
        }

        public List<string> PhotoUrls
        {
            get
            {
                return photoUrls;
            }

            set
            {
                photoUrls = value;
            }
        }

        public List<string> VideoUrls
        {
            get
            {
                return videoUrls;
            }

            set
            {
                videoUrls = value;
            }
        }
    }
}
