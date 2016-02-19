using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaseAI_CE.WebFiles.TumblrAPI;

namespace TeaseAI_CE.WebFiles
{
    public class TumblrCrawler
    {
        public static List<string> GetAllPhotoUrls(TumblrBlog blog)
        {
            ulong postTotal = 0;

            List<string> urls = new List<string>();

            //initial request to get first 20 posts + blogInfo
            APIResponse response = TumblrApiRequestFactory.Request_TypePosts(blog, PostType.photo).GetResponse();

            if (response.Meta.Status != 200)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            foreach (Post post in response.Response.Posts)
            {
                foreach (Photo photo in post.Photos)
                {
                    urls.Add(photo.Original_size.Url);
                }
            }

            //get total posts from blogInfo
            postTotal = response.Response.Blog.Posts;

            if (postTotal == 20)
            {
                return urls;
            }

            //get remaining posts
            for (ulong i = 0; i <= postTotal && response.Response.Posts.Length >= 20; i += 20)
            {
                response = TumblrApiRequestFactory.Request_TypePostsFrom(blog, PostType.photo, i).GetResponse();

                foreach (Post post in response.Response.Posts)
                {
                    foreach (Photo photo in post.Photos)
                    {
                        urls.Add(photo.Original_size.Url);
                    }
                }
            }

            return urls;
        }

        public static async Task<List<string>> GetAllPhotoUrlsAsync(TumblrBlog blog)
        {
            ulong postTotal = 0;

            List<string> urls = new List<string>();

            //initial request to get first 20 posts + blogInfo
            APIResponse response = await TumblrApiRequestFactory.Request_TypePosts(blog, PostType.photo).GetResponseAsync();
            
            if(response.Meta.Status != 200)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            foreach(Post post in response.Response.Posts)
            {
                foreach(Photo photo in post.Photos)
                {
                    urls.Add(photo.Original_size.Url);
                }
            }

            //get total posts from blogInfo
            postTotal = response.Response.Blog.Posts;
            
            if(postTotal == 20)
            {
                return urls;
            }

            //get remaining posts
            for (ulong i = 0; i <= postTotal && response.Response.Posts.Length >= 20; i += 20)
            {
                response = await TumblrApiRequestFactory.Request_TypePostsFrom(blog, PostType.photo, i).GetResponseAsync();

                foreach (Post post in response.Response.Posts)
                {
                    foreach (Photo photo in post.Photos)
                    {
                        urls.Add(photo.Original_size.Url);
                    }
                }
            }

            return urls;
        }

        public static List<string> GetAllVideoUrls(TumblrBlog blog)
        {
            ulong postTotal = 0;

            List<string> urls = new List<string>();

            //initial request to get first 20 posts + blogInfo
            APIResponse response = TumblrApiRequestFactory.Request_TypePosts(blog, PostType.video).GetResponse();

            if (response.Meta.Status != 200)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            foreach (Post post in response.Response?.Posts)
            {
                //dunno which would be better yet
                //if(post.Video_type == "tumblr")
                //{
                //    urls.Add(post.Video_url);
                //}

                urls.Add(post?.Video_url);
            }

            //get total posts from blogInfo
            postTotal = response.Response.Blog.Posts;

            if (postTotal == 20)
            {
                return urls;
            }

            //get remaining posts
            for (ulong i = 0; i <= postTotal && response.Response.Posts.Length >= 20; i += 20)
            {
                response = TumblrApiRequestFactory.Request_TypePostsFrom(blog, PostType.video, i).GetResponse();

                foreach (Post post in response.Response.Posts)
                {
                    //dunno which would be better yet
                    //if(post.Video_type == "tumblr")
                    //{
                    //    urls.Add(post.Video_url);
                    //}

                    urls.Add(post?.Video_url);
                }
            }

            return urls;
        }

        public static async Task<List<string>> GetAllVideoUrlsAsync(TumblrBlog blog)
        {
            ulong postTotal = 0;

            List<string> urls = new List<string>();

            //initial request to get first 20 posts + blogInfo
            APIResponse response = await TumblrApiRequestFactory.Request_TypePosts(blog, PostType.video).GetResponseAsync();

            if (response.Meta.Status != 200)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            foreach (Post post in response.Response?.Posts)
            {
                //dunno which would be better yet
                //if(post.Video_type == "tumblr")
                //{
                //    urls.Add(post.Video_url);
                //}

                urls.Add(post?.Video_url);
            }

            //get total posts from blogInfo
            postTotal = response.Response.Blog.Posts;

            if (postTotal == 20)
            {
                return urls;
            }

            //get remaining posts
            for (ulong i = 0; i <= postTotal && response.Response.Posts.Length >= 20; i += 20)
            {
                response = await TumblrApiRequestFactory.Request_TypePostsFrom(blog, PostType.video, i).GetResponseAsync();

                foreach (Post post in response.Response.Posts)
                {
                    //dunno which would be better yet
                    //if(post.Video_type == "tumblr")
                    //{
                    //    urls.Add(post.Video_url);
                    //}

                    urls.Add(post?.Video_url);
                }
            }

            return urls;
        }

        public static string GetAvatarUrl(TumblrBlog blog)
        {
            APIResponse response = TumblrApiRequestFactory.Request_Avatar(blog).GetResponse();

            if (response.Meta.Status != 200 && response.Meta.Status != 301)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            return response.Response.Avatar_url;
        }

        public static async Task<string> GetAvatarUrlAsync(TumblrBlog blog)
        {
            APIResponse response = await TumblrApiRequestFactory.Request_Avatar(blog).GetResponseAsync();

            if (response.Meta.Status != 200 && response.Meta.Status != 301)
            {
                throw new Exception($"Something went wrong while accessing the API. Maybe the API key is outdated. STATUS:{response.Meta.Status} # MSG:{response.Meta.Msg}");
            }

            return response.Response.Avatar_url;
        }
    }
}
