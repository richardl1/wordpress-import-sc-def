using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEFExample.Models.Master.sitecore.templates.Data_Exchange.Providers.Wordpress.Endpoints;
using DEFExample.Website.Models;
using DEFExample.Website.Models.ItemModels;
using Sitecore.DataExchange.Converters;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Converters.Endpoints;
using Sitecore.DataExchange;
using Sitecore.Web.UI.HtmlControls;

namespace DEFExample.Website.Converters.Endpoints
{
    //By inheriting from BaseEndpointConverter you get access to a number of methods that facilitate reading values from fields on a Sitecore item.
    public class WordpressEndpointConverter : BaseEndpointConverter
    {
        private static readonly Guid TemplateId = Guid.Parse(IWordPress_Endpoint_Constants.TemplateIdString);
        public WordpressEndpointConverter(IItemModelRepository repository) : base(repository)
        {
            //
            //identify the template an item must be based
            //on in order for the converter to be able to
            //convert the item
            this.SupportedTemplateIds.Add(TemplateId);
        }
        protected override void AddPlugins(ItemModel source, Endpoint endpoint)
        {
            //
            //create the plugin
            var settings = new WordpressSettings
            {
                PostsUrl = GetStringValue(source, IWordPress_Endpoint_Constants.Posts_URL_FieldName),
                TagsUrl = GetStringValue(source, IWordPress_Endpoint_Constants.Tags_URL_FieldName),
                CategoriesUrl = GetStringValue(source, IWordPress_Endpoint_Constants.Categories_URL_FieldName)
            };
            //
            //populate the plugin using values from the item
            //WordPressEndpointItemModel.PostsUrl);
            //WordPressEndpointItemModel.TagsUrl);
            //WordPressEndpointItemModel.CategoriesUrl);
            //add the plugin to the endpoint
            endpoint.AddPlugin(settings);
        }
        
    }
}