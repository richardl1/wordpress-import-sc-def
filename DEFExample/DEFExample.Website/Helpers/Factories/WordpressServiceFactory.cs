using DEFExample.Website.Helpers.Services;

namespace DEFExample.Website.Helpers.Factories
{
    public class WordpressServiceFactory
    {
        /// <summary>
        /// Builds a concrete instance of the Wordpresservice repository.
        /// </summary>
        public static IWordpressService Build()
        {
            return new WordpressService();
        }
    }
}