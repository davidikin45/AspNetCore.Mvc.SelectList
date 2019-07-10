using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Mvc.SelectList
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MVC select list attribute services to the application.
        /// </summary>
        public static IMvcBuilder AddMvcSelectListAttributes(this IMvcBuilder builder)
        {
            var services = builder.Services;

            services.Decorate<IHtmlGenerator, SelectListHtmlGenerator>();

            return builder;
        }
    }
}
