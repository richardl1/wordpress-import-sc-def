using System.Collections.Generic;
using DEFExample.Website.Models;
using Sitecore.Services.Core.Diagnostics;

namespace DEFExample.Website.Helpers.Services
{
    /// <summary>
    /// Wordpress writer interface.
    /// </summary>
    public interface IWordpressService
    {
        /// <summary>
        /// Create the Wordpress blog posts in sitecore.
        /// </summary>
        void CreateBlogPostsInSitecore(List<BlogPost> posts, ILogger log);

        /// <summary>
        /// Create the Wordpress tags in sitecore.
        /// </summary>
        void CreateTagsInSitecore(List<Tag> tags, ILogger log);

        /// <summary>
        /// Create the Wordpress categories in sitecore.
        /// </summary>
        void CreateCategoriesInSitecore(List<Category> categories, ILogger log);

        ///// <summary>
        ///// Read posts using wp api
        ///// </summary>
        //List<BlogPost> ReadBlogPosts(string url, ILogger log);

        ///// <summary>
        ///// Read tags using wp api
        ///// </summary>
        //List<Tag> ReadTags(string url, ILogger log);

        ///// <summary>
        ///// Read categories using wp api
        ///// </summary>
        //List<Category> ReadCategories(string url, ILogger log);

        /// <summary>
        /// Read any type using wp api
        /// </summary>
        List<T> Read<T>(string url, ILogger log);
    }
}