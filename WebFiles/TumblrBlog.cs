using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaseAI_CE.WebFiles
{
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

        public URLFile CompileUrlFile()
        {
            URLFile urlFile = new URLFile();

            urlFile.PhotoUrls = TumblrCrawler.GetAllPhotoUrls(this);
            urlFile.VideoUrls = TumblrCrawler.GetAllVideoUrls(this);
            urlFile.AvatarUrl = TumblrCrawler.GetAvatarUrl(this);


            return urlFile;
        }

        public async Task<URLFile> CompileUrlFileAsync()
        {
            URLFile urlFile = new URLFile();

            urlFile.PhotoUrls = await TumblrCrawler.GetAllPhotoUrlsAsync(this);
            urlFile.VideoUrls = await TumblrCrawler.GetAllVideoUrlsAsync(this);
            urlFile.AvatarUrl = await TumblrCrawler.GetAvatarUrlAsync(this);


            return urlFile;
        }

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
