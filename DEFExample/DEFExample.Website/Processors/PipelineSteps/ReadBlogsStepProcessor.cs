using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using System;
using System.Collections.Generic;
using DEFExample.Website.Models;
using Examples.FileSystem;
using Newtonsoft.Json;
using Sitecore.Services.Core.Diagnostics;
using System.Net;
using DEFExample.Website.Helpers.Factories;
using DEFExample.Website.Helpers.Services;
using Sitecore.DataExchange;

namespace DEFExample.Website.Processors.PipelineSteps
{

    [RequiredEndpointPlugins(typeof(WordpressSettings))]
    public class ReadBlogsStepProcessor : BaseReadDataStepProcessor
    {
        // protected static readonly string TotalNumberOfPages = "X-WP-TotalPages";
        private static IWordpressService _wordpressService;

        public ReadBlogsStepProcessor()
        {
            _wordpressService = WordpressServiceFactory.Build();
        }
        protected override void ReadData(Endpoint endpoint, PipelineStep pipelineStep, PipelineContext pipelineContext,
            ILogger logger)
        {

            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (pipelineStep == null)
            {
                throw new ArgumentNullException(nameof(pipelineStep));
            }
            if (pipelineContext == null)
            {
                throw new ArgumentNullException(nameof(pipelineContext));
            }
            try
            {
                var settings = endpoint.GetWordpressSettings();
                if (settings == null)
                {
                    logger.Error("Empty WordPress settings");
                    return;
                }
                List<BlogPost> blogs = _wordpressService.Read<BlogPost>(settings.PostsUrl, logger);
                var blogData = new IterableDataSettings(blogs);
                pipelineContext.AddPlugin(blogData);
            }
            catch (Exception ex)
            {
                logger.Error($"Error in ReadBlogsStepProcessor: {ex.InnerException}");
                pipelineContext.CriticalError = true;
            }
        }



    }
}