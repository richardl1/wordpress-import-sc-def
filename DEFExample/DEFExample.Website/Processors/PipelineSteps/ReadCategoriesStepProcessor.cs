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

namespace DEFExample.Website.Processors.PipelineSteps
{

    [RequiredEndpointPlugins(typeof(WordpressSettings))]
    public class ReadCategoriesStepProcessor : BaseReadDataStepProcessor
    {
        // protected static readonly string TotalNumberOfPages = "X-WP-TotalPages";
        private static IWordpressService _wordpressService;

        public ReadCategoriesStepProcessor()
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
                List<Category> categories = _wordpressService.Read<Category>(settings.CategoriesUrl, logger);
                var categoriesData = new IterableDataSettings(categories);
                pipelineContext.AddPlugin(categoriesData);
            }
            catch (Exception ex)
            {
                logger.Error($"Error in ReadCategoriesStepProcessor: {ex.InnerException}");
                pipelineContext.CriticalError = true;
            }
        }


        
    }
}