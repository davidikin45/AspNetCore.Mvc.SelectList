using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Mvc.SelectList
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSelectListAttributes(this IServiceCollection services)
        {
            return services.Decorate<IHtmlGenerator, SelectListHtmlGenerator>();
        }
    }
}
