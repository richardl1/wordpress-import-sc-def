using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using DEFExample.Models.Master.sitecore.templates.Example.Content_Type.Blogs;
using DEFExample.Website.Models;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Services.Core.Diagnostics;
using Category = DEFExample.Website.Models.Category;

namespace DEFExample.Website.Helpers.Services
{
    public class WordpressService : IWordpressService
    {
        protected static readonly Database Db = Database.GetDatabase("master");

        public WordpressService()
        {
        }

        public void CreateBlogPostsInSitecore(List<BlogPost> posts, ILogger log)
        {
            log.Debug("Starting CreateBlogPostsInSitecore");
            using (new SecurityDisabler())
            {
                Item parentItem = Db.Items[Constants.PostsFolderId];

                TemplateItem template = Db.GetTemplate(IBlog_Constants.TemplateId);
                ChildList tags = Db.Items[Constants.TagsFolderId].GetChildren();
                ChildList categories = Db.Items[Constants.CategoriesFolderId].GetChildren();

                if (parentItem==null||template == null || tags == null || categories == null)
                {
                    log.Error("Empty mandatory parameter for the blog post items creation");
                    return;
                }

                List<Item> existentBlogs = parentItem.Axes.GetDescendants().Where(x => x.TemplateID.Equals(template.ID))
                    .ToList();

                DeleteMissingWordpressPosts(posts, existentBlogs);

                foreach (var post in posts)
                {

                    //Find if blog already exists
                    Item sitecoreBlog = existentBlogs
                        .FirstOrDefault(x => x[IBlog_Constants.Wordpress_Id_FieldName].Equals(post.id.ToString()));

                    if (sitecoreBlog == null)
                    {
                        Regex regex = new Regex("[-,_]");
                        sitecoreBlog = parentItem.Add(regex.Replace(post.slug, " "), template);
                    }
                    using (new EditContext(sitecoreBlog))
                    {
                        log.Debug($"WordPress blog to be created/updated {post.id}");
                        sitecoreBlog[IBlog_Constants.Wordpress_Id_FieldName] = post.id.ToString();
                        sitecoreBlog[IBlog_Constants.Date_FieldName] = DateUtil.ToIsoDate(post.date);
                        sitecoreBlog[IBlog_Constants.Date_Gmt_FieldName] = DateUtil.ToIsoDate(post.date_gmt);
                        sitecoreBlog[IBlog_Constants.Modified_Date_FieldName] = DateUtil.ToIsoDate(post.modified);
                        sitecoreBlog[IBlog_Constants.Modified_Date_Gmt_FieldName] =
                            DateUtil.ToIsoDate(post.modified_gmt);
                        sitecoreBlog[IBlog_Constants.Slug_FieldName] = post.slug;
                        sitecoreBlog[IBlog_Constants.Status_FieldName] = post.status;
                        sitecoreBlog[IBlog_Constants.Type_FieldName] = post.type;
                        sitecoreBlog[IBlog_Constants.Link_FieldName] = post.link;
                        sitecoreBlog[IBlog_Constants.Title_FieldName] = post.title.rendered;
                        sitecoreBlog[IBlog_Constants.Content_FieldName] = post.content.rendered;
                        sitecoreBlog[IBlog_Constants.Excerpt_FieldName] = post.excerpt.rendered;
                        sitecoreBlog[IBlog_Constants.Comment_Status_FieldName] = post.comment_status;
                        sitecoreBlog[IBlog_Constants.Ping_Status_FieldName] = post.ping_status;
                        sitecoreBlog[IBlog_Constants.Sticky_FieldName] = post.sticky.ToString();

                        if (post._embedded != null && post._embedded.wpfeaturedmedia != null)
                        {
                            EmbeddedWpFeaturedmedia media = post._embedded.wpfeaturedmedia.FirstOrDefault();
                            sitecoreBlog[IBlog_Constants.Image_Link_FieldName] =
                                media != null && media.source_url != null ? media.source_url : string.Empty;
                        }

                        string categoryIds = string.Empty;
                        foreach (var postCategory in post.categories)
                        {
                            ID postCategoryId = categories.Where(x =>
                                    x[ICategory_Constants.Wordpress_Id_FieldName].Equals(postCategory.ToString()))
                                .Select(x => x.ID).FirstOrDefault();
                            categoryIds += string.Concat(postCategoryId, "|");
                        }
                        sitecoreBlog[IBlog_Constants.Categories_FieldName] = categoryIds;
                        string tagIds = string.Empty;
                        foreach (var postTag in post.tags)
                        {
                            ID postTagId = tags
                                .Where(x => x[ITag_Constants.Wordpress_Id_FieldName].Equals(postTag.ToString()))
                                .Select(x => x.ID).FirstOrDefault();
                            tagIds += string.Concat(postTagId, "|");
                        }
                        sitecoreBlog[IBlog_Constants.Tags_FieldName] = tagIds;
                    }
                }
            }
        }

        public void CreateTagsInSitecore(List<Models.Tag> tags, ILogger log)
        {
            log.Debug("Starting CreateTagsInSitecore");
            using (new SecurityDisabler())
            {
                Item parentItem = Db.Items[Constants.TagsFolderId];

                TemplateItem template = Db.GetTemplate(ITag_Constants.TemplateId);
                if (parentItem == null || template == null)
                {
                    log.Error("Empty mandatory parameter for the tag items creation");
                    return;
                }

                List<Item> existentTags = parentItem.GetChildren().Where(x => x.TemplateID.Equals(template.ID)).ToList();

                foreach (var tag in tags)
                {
                    //Find if tag already exists
                    Item sitecoreTag = existentTags
                        .FirstOrDefault(x => x[ITag_Constants.Wordpress_Id_FieldName].Equals(tag.id.ToString()));

                    if (sitecoreTag == null)
                    {
                        sitecoreTag = parentItem.Add(tag.name, template);
                    }
                    
                    using (new EditContext(sitecoreTag))
                    {
                        log.Debug($"WordPress tag to be created/updated {tag.id}");
                        sitecoreTag[ITag_Constants.Name_FieldName] = tag.name;
                        sitecoreTag[ITag_Constants.Wordpress_Id_FieldName] = tag.id.ToString();
                    }


                }
            }

        }

        public void CreateCategoriesInSitecore(List<Models.Category> categories, ILogger log)
        {
            log.Debug("Starting CreateCategoriesInSitecore");
            using (new SecurityDisabler())
            {
                Item parentItem = Db.Items[Constants.CategoriesFolderId];

                TemplateItem template = Db.GetTemplate(ICategory_Constants.TemplateId);

                if (parentItem == null || template == null)
                {
                    log.Error("Empty mandatory parameter for the category items creation");
                    return;
                }

                List<Item> existentCategories = parentItem.GetChildren().Where(x => x.TemplateID.Equals(template.ID)).ToList();

                foreach (var category in categories)
                {
                    //Find if category already exists
                    Item sitecoreCategory = existentCategories
                        .FirstOrDefault(x => x[ICategory_Constants.Wordpress_Id_FieldName].Equals(category.id.ToString()));

                    if (sitecoreCategory == null)
                    {
                        sitecoreCategory = parentItem.Add(category.name, template);
                    }

                    using (new EditContext(sitecoreCategory))
                    {
                        log.Debug($"WordPress categories to be created/updated {category.id}");
                        sitecoreCategory[ICategory_Constants.Name_FieldName] = category.name;
                        sitecoreCategory[ICategory_Constants.Wordpress_Id_FieldName] = category.id.ToString();
                    }

                }
            }

        }

        //public List<BlogPost> ReadBlogPosts(string postsUrl, ILogger log)
        //{
        //    log.Debug("Starting ReadBlogPosts");
        //    using (WebClient wc = new WebClient())
        //    {
        //        string response = wc.DownloadString(postsUrl);
        //        var blogPosts = JsonConvert.DeserializeObject<IEnumerable<BlogPost>>(response);
        //        var blogPostsList = blogPosts?.ToList() ?? Enumerable.Empty<BlogPost>().ToList();
        //        log.Debug($"Number of Posts Retrieved from API: {blogPostsList.Count}");
        //        return blogPostsList;
        //    }
        //}

        //public List<Models.Category> ReadCategories(string tagsUrl, ILogger log)
        //{
        //    log.Debug("Starting ReadCategories");
        //    using (WebClient wc = new WebClient())
        //    {
        //        string response = wc.DownloadString(tagsUrl);
        //        var categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(response);
        //        var categoriesList = categories?.ToList() ?? Enumerable.Empty<Category>().ToList();
        //        log.Debug($"Number of Categories Retrieved from API: {categoriesList.Count}");
        //        return categoriesList;
        //    }
        //}

        //public List<Models.Tag> ReadTags(string tagsUrl, ILogger log)
        //{
        //    log.Debug("Starting ReadTags");
        //    using (WebClient wc = new WebClient())
        //    {
        //        string response = wc.DownloadString(tagsUrl);
        //        var tags = JsonConvert.DeserializeObject<IEnumerable<Models.Tag>>(response);
        //        var tagsList =  tags?.ToList() ?? Enumerable.Empty<Models.Tag>().ToList();
        //        log.Debug($"Number of Tags Retrieved from API: {tagsList.Count}");
        //        return tagsList;
        //    }
        //}

        public List<T> Read<T>(string url, ILogger log)
        {
            log.Debug($"Starting Read {typeof(T)}");
            using (WebClient wc = new WebClient())
            {
                string response = wc.DownloadString(url);
                var tags = JsonConvert.DeserializeObject<IEnumerable<T>>(response);
                var tagsList = tags?.ToList() ?? Enumerable.Empty<T>().ToList();
                log.Debug($"Number of {typeof(T)} Retrieved from API: {tagsList.Count}");
                return tagsList;
            }
        }

        private void DeleteMissingWordpressPosts(List<BlogPost> postsList, List<Item> sitecorePostsList)
        {
            var itemsToDelete = sitecorePostsList.Where(x =>
                !postsList.Any(y => y.id.ToString().Equals(x[IBlog_Constants.Wordpress_Id_FieldName])));
            using (new SecurityDisabler())
            {
                foreach (var item in itemsToDelete)
                {
                    try
                    {
                        item.Editing.BeginEdit();
                        item.Delete();
                    }
                    finally
                    {
                        item.Editing.EndEdit();
                    }
                }
            }
        }
    }
}