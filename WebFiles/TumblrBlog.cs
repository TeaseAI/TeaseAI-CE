using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaseAI_CE.WebFiles
{
    /// <summary>
    /// Represenents a TumblrBlog with all needed information to compile a URLFile from it
    /// </summary>
    public class TumblrBlog
    {
        private Uri url;
        public TumblrBlog(string URL)
        {
            if(!Uri.TryCreate(URL, UriKind.Absolute, out url))
            {
                throw new UriFormatException($"{URL} is not a valid/parseable URI");
            }
        }

        /// <summary>
        /// Compiles the URLFile from itself
        /// </summary>
        /// <returns></returns>
        public URLFile CompileUrlFile()
        {
            URLFile urlFile = new URLFile();

            urlFile.PhotoUrls = TumblrCrawler.GetAllPhotoUrls(this);
            urlFile.VideoUrls = TumblrCrawler.GetAllVideoUrls(this);
            urlFile.AvatarUrl = TumblrCrawler.GetAvatarUrl(this);

            return urlFile;
        }


        /// <summary>
        /// Compiles the URLFile from itself asynchronosly
        /// </summary>
        /// <returns></returns>
        public async Task<URLFile> CompileUrlFileAsync()
        {
            URLFile urlFile = new URLFile();

            urlFile.PhotoUrls = await TumblrCrawler.GetAllPhotoUrlsAsync(this);
            urlFile.VideoUrls = await TumblrCrawler.GetAllVideoUrlsAsync(this);
            urlFile.AvatarUrl = await TumblrCrawler.GetAvatarUrlAsync(this);


            return urlFile;
        }

        /// <summary>
        /// URL of the blog
        /// </summary>
        public Uri Url
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
