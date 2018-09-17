using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEFExample.Website
{
    public static class Constants
    {
        public static readonly string TagsFolderId = Sitecore.Configuration.Settings.GetSetting("DEFExample.GUIDS.TagsFolder");
        public static readonly string PostsFolderId = Sitecore.Configuration.Settings.GetSetting("DEFExample.GUIDS.PostsFolder");
        public static readonly string CategoriesFolderId = Sitecore.Configuration.Settings.GetSetting("DEFExample.GUIDS.CategoriesFolder");
    }
}