using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using DEFExample.Website.Models;
using Examples.FileSystem;
using Newtonsoft.Json;
using Sitecore.Services.Core.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using DEFExample.Models.Master.sitecore.templates.Example.Content_Type.Blogs;
using DEFExample.Website.Helpers.Factories;
using DEFExample.Website.Helpers.Services;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace DEFExample.Website.Processors.PipelineSteps
{

    [RequiredEndpointPlugins(typeof(WordpressSettings))]
    public class WriteCategoriesStepProcessor : BasePipelineStepProcessor
    {
        // protected static readonly string TotalNumberOfPages = "X-WP-TotalPages";
        protected static readonly Database Db = Sitecore.Configuration.Factory.GetDatabase("master");
        private static IWordpressService _wordpressService;

        public WriteCategoriesStepProcessor()
        {
            _wordpressService = WordpressServiceFactory.Build();
        }
        protected override void ProcessPipelineStep(PipelineStep pipelineStep, PipelineContext pipelineContext,
            ILogger logger)
        {
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
                var dataSet = pipelineContext.GetPlugin<IterableDataSettings>();
                var categories = dataSet?.Data?.Cast<Models.Category>().ToList();
                _wordpressService.CreateCategoriesInSitecore(categories, logger);
            }
            catch (Exception ex)
            {
                logger.Error($"Error in WriteCategoriesStepProcessor: {ex.InnerException}");
                pipelineContext.CriticalError = true;
            }
            
        }
    }
}