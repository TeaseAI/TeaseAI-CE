using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaseAI_CE.WebFiles
{
    /// <summary>
    /// Represents a URL file
    /// These files hold URLs from online photos and videos
    /// </summary>
    [Serializable]
    public class URLFile
    {
        string avatarUrl;
        private List<string> photoUrls = new List<string>();
        private List<string> videoUrls = new List<string>();

        /// <summary>
        /// Can be used by the user to identify the URL more quickly
        /// </summary>
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

        /// <summary>
        /// Holds the URLs to the photos
        /// </summary>
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

        /// <summary>
        /// Holds the URLs to the videos
        /// </summary>
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
