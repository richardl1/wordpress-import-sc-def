using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange;

namespace DEFExample.Website.Models
{
    public class WordpressSettings : IPlugin
    {
        public WordpressSettings(){}
        public string PostsUrl { get; set; }
        public string TagsUrl { get; set; }
        public string CategoriesUrl { get; set; }
    }
}