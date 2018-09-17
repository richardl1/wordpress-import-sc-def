using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Services.Core.Model;

namespace DEFExample.Website.Models.ItemModels
{
    public class WordPressEndpointItemModel :ItemModel
    {
        public const string PostsUrl = "Posts URL";
        public const string TagsUrl = "Tags URL";
        public const string CategoriesUrl = "Categories URL";
    }
}