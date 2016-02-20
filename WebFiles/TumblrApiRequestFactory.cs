using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaseAI_CE.WebFiles.TumblrAPI
{
    public enum Command
    {
        user,
        blog
    }
    public enum Method
    {
        info,
        avatar,
        posts
    }

    public enum PostType
    {
        text,
        photo,
        video
    }

    /// <summary>
    /// Class which is used to build TumblrApiRequests from a given TumblrBlog instance
    /// </summary>
    public static class TumblrApiRequestFactory
    {
        const string APIKEY = "uWbnYYnVd2yLJRmuqwGXDCFH5RoX5nlvQwWdHRhqCsoM7W9Bqn";

        /// <summary>
        /// Build a request which queries for the blog info
        /// </summary>
        /// <param name="blog">Blog whose info is requested</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_Info(TumblrBlog blog)
        {
            return BuildRequest(blog, Method.info);
        }

        /// <summary>
        /// Build request which queries for the blog-avatar
        /// </summary>
        /// <param name="blog">Blog whose avatar is requested</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_Avatar(TumblrBlog blog)
        {
            return BuildRequest(blog, Method.avatar);
        }

        #region No type specified

        /// <summary>
        /// Builds a request which queries for the newest 20 posts
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_Posts(TumblrBlog blog)
        {
            return BuildRequest(blog, Method.posts);
        }

        /// <summary>
        /// Builds a request which queries for the first 20 posts starting from a given post offset
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="offset">Offset to to start from</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_PostsFrom(TumblrBlog blog, ulong offset)
        {
            return BuildRequest(blog, Method.posts, new string[] { $"offset={offset}" });
        }

        /// <summary>
        /// Builds a request which queries for a limited amount of posts starting from the newest post
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="limit">How many posts are queried for</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_PostsLimit(TumblrBlog blog, int limit)
        {
            if(limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Method.posts, new string[] { $"limit={limit}" });
        }

        /// <summary>
        /// Builds a request which queries for a limited amount of posts starting from the offset given
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="offset">Offset to to start from</param>
        /// <param name="limit">How many posts are queried for</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_PostsFromLimit(TumblrBlog blog, ulong offset, int limit)
        {
            if (limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Method.posts, new string[] { $"limit={limit}", $"offset={offset}" });
        }

        #endregion

        #region type specified

        /// <summary>
        /// Builds a request which queries for the newest 20 posts of a specific type
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="postType">Type of posts which are requested</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_TypePosts(TumblrBlog blog, PostType postType)
        {
            string postType_str = "";

            switch(postType)
            {
                case PostType.photo:
                    postType_str = "photo";
                    break;
                case PostType.video:
                    postType_str = "video";
                    break;
                default:
                    throw new Exception("Invalid post type specified!");
            }

            return BuildRequest(blog, Method.posts, new string[] { $"type={postType_str}" });
        }

        /// <summary>
        /// Builds a request which queries for the next 20 posts of a specific type
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="postType">Type of posts which are requested</param>
        /// <param name="offset">Offset to to start from</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_TypePostsFrom(TumblrBlog blog, PostType postType, ulong offset)
        {
            string postType_str = "";

            switch (postType)
            {
                case PostType.photo:
                    postType_str = "photo";
                    break;
                case PostType.video:
                    postType_str = "video";
                    break;
                default:
                    throw new Exception("Invalid post type specified!");
            }

            return BuildRequest(blog, Method.posts, new string[] { $"type={postType_str}", $"offset={offset}" });
        }

        /// <summary>
        /// Builds a request which queries for a limited amount of posts of a specific type starting from the newest post
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="postType">Type of posts which are requested</param>
        /// <param name="limit">How many posts are queried for</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_TypePostsLimit(TumblrBlog blog, PostType postType, int limit)
        {
            string postType_str = "";

            switch (postType)
            {
                case PostType.photo:
                    postType_str = "photo";
                    break;
                case PostType.video:
                    postType_str = "video";
                    break;
                default:
                    throw new Exception("Invalid post type specified!");
            }

            if (limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Method.posts, new string[] { $"type={postType_str}", $"limit={limit}" });
        }

        /// <summary>
        /// Builds a request which queries for a limited amount of posts of a specific type starting from the given offset
        /// </summary>
        /// <param name="blog">Blog whose posts are requested</param>
        /// <param name="postType">>Type of posts which are requested</param>
        /// <param name="offset">Offset to to start from<</param>
        /// <param name="limit">How many posts are queried for</param>
        /// <returns></returns>
        public static TumblrApiRequest Request_TypePostsFromToLimit(TumblrBlog blog, PostType postType, ulong offset, int limit)
        {
            string postType_str = "";

            switch (postType)
            {
                case PostType.photo:
                    postType_str = "photo";
                    break;
                case PostType.video:
                    postType_str = "video";
                    break;
                default:
                    throw new Exception("Invalid post type specified!");
            }

            if (limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Method.posts, new string[] { $"type={postType_str}", $"limit={limit}", $"offset={offset}" });
        }

        #endregion

        /// <summary>
        /// Builds a request which queries a given blog
        /// </summary>
        /// <param name="blog">Blog to query data from</param>
        /// <param name="method">Method to query blog with</param>
        /// <returns></returns>
        public static TumblrApiRequest BuildRequest(TumblrBlog blog, Method method)
        {
            return BuildRequest(blog, Method.posts, null);
        }

        /// <summary>
        /// Builds a request which queries a given blog
        /// </summary>
        /// <param name="blog">Blog to query data from</param>
        /// <param name="method">Method to query blog with</param>
        /// <param name="options">API specific options to query with</param>
        /// <returns></returns>
        public static TumblrApiRequest BuildRequest(TumblrBlog blog, Method method, string[] options)
        {
            string requestUrl = "api.tumblr.com/v2/";
            string method_str = "";

            switch(method)
            {
                case Method.info:
                    method_str = "info";
                    break;
                case Method.posts:
                    method_str = "posts";
                    break;
                case Method.avatar:
                    method_str = "avatar";
                    break;
                default:
                    throw new Exception("Invalid method specified!");
            }

            requestUrl += $"blog/{blog.Url.Host}/{method_str}/?";

            foreach(string option in options)
            {
                requestUrl += $"{option}&";
            }

            requestUrl += $"api_key={APIKEY}";

            return new TumblrApiRequest(requestUrl);
        }
    }
}
