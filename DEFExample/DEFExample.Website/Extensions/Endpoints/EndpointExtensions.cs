using Sitecore.DataExchange.Models;
using System;
using DEFExample.Website.Models;

namespace Examples.FileSystem
{
    public static class EndpointExtensions
    {
        public static WordpressSettings GetWordpressSettings(this Endpoint endpoint)
        {
            return endpoint.GetPlugin<WordpressSettings>();
        }
        public static bool HasTextFileSettings(this Endpoint endpoint)
        {
            return GetWordpressSettings(endpoint) != null;
        }
    }
}