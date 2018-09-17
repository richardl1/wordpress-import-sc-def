using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEFExample.Models.Master.sitecore.templates.Data_Exchange.Providers.Wordpress.Pipeline_Steps;
using DEFExample.Website.Models.ItemModels;
using Sitecore.DataExchange.Converters.PipelineSteps;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;

namespace DEFExample.Website.Converters.PipelineSteps
{
    public class WordPressStepConverter : BasePipelineStepConverter
    {
        private static readonly Guid TemplateId = Guid.Parse(IWordpress_Pipeline_Step_Constants.TemplateIdString);
        public WordPressStepConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }
        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            AddEndpointSettings(source, pipelineStep);
        }
        private void AddEndpointSettings(ItemModel source, PipelineStep pipelineStep)
        {
            var settings = new EndpointSettings();
            var endpointFrom =
                base.ConvertReferenceToModel<Endpoint>(source,
                    IWordpress_Pipeline_Step_Constants.EndpointFrom_FieldName);//WordPressStepItemModel.EndpointFrom);
            if (endpointFrom != null)
            {
                settings.EndpointFrom = endpointFrom;
            }
            pipelineStep.AddPlugin(settings);
        }
    }
}