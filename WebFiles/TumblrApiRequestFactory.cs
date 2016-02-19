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

    public static class TumblrApiRequestFactory
    {
        const string APIKEY = "uWbnYYnVd2yLJRmuqwGXDCFH5RoX5nlvQwWdHRhqCsoM7W9Bqn";

        public static TumblrApiRequest Request_Info(TumblrBlog blog)
        {
            return BuildRequest(blog, Command.blog, Method.info);
        }

        public static TumblrApiRequest Request_Avatar(TumblrBlog blog)
        {
            return BuildRequest(blog, Command.blog, Method.avatar);
        }

        #region No type specified

        public static TumblrApiRequest Request_Posts(TumblrBlog blog)
        {
            return BuildRequest(blog, Command.blog, Method.posts);
        }

        public static TumblrApiRequest Request_PostsFrom(TumblrBlog blog, ulong offset)
        {
            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"offset={offset}" });
        }

        public static TumblrApiRequest Request_PostsLimit(TumblrBlog blog, int limit)
        {
            if(limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"limit={limit}" });
        }

        public static TumblrApiRequest Request_PostsFromLimit(TumblrBlog blog, ulong offset, int limit)
        {
            if (limit > 20)
            {
                throw new Exception("Limit can't be greater than 20 which is max posts in one response by tumblrAPI");
            }


            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"limit={limit}", $"offset={offset}" });
        }

        #endregion

        #region type specified
       
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

            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"type={postType_str}" });
        }
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

            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"type={postType_str}", $"offset={offset}" });
        }

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


            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"type={postType_str}", $"limit={limit}" });
        }

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


            return BuildRequest(blog, Command.blog, Method.posts, new string[] { $"type={postType_str}", $"limit={limit}", $"offset={offset}" });
        }

        #endregion

        public static TumblrApiRequest BuildRequest(TumblrBlog blog, Command command, Method method)
        {
            return BuildRequest(blog, Command.blog, Method.posts, null);
        }

        public static TumblrApiRequest BuildRequest(TumblrBlog blog, Command command, Method method, string[] options)
        {
            string requestUrl = "api.tumblr.com/v2/";
            string command_str = "";
            string method_str = "";

            switch(command)
            {
                case Command.blog:
                    command_str = "blog";
                    break;
                case Command.user:
                    throw new NotImplementedException();                    
                default:
                    return null;
            }

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
                    return null;
            }

            requestUrl += $"{command_str}/{blog.Url.Host}/{method_str}/?";

            foreach(string option in options)
            {
                requestUrl += $"{option}&";
            }

            requestUrl += $"api_key={APIKEY}";

            return new TumblrApiRequest(requestUrl);
        }
    }
}
