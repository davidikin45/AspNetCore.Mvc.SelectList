using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#if NETCOREAPP3_0
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
#else
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
#endif
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Encodings.Web;

namespace AspNetCore.Mvc.SelectList.Internal
{
    internal static class HtmlHelperExtensions
    {
        public static IHtmlHelper<TModel> For<TModel>(this IHtmlHelper helper) where TModel : class, new()
        {
            return For<TModel>(helper.ViewContext, helper.ViewData);
        }

        public static IHtmlHelper<TModel> For<TModel>(this IHtmlHelper helper, TModel model)
        {
            return For<TModel>(helper.ViewContext, helper.ViewData, model);
        }

        public static IHtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData) where TModel : class, new()
        {
            TModel model = new TModel();
            return For<TModel>(viewContext, viewData, model);
        }

        public static IHtmlHelper<TModel> For<TModel>(ViewContext viewContext, ViewDataDictionary viewData, TModel model)
        {
            var newViewData = new ViewDataDictionary<TModel>(viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>(), new ModelStateDictionary()) { Model = model };

            ViewContext newViewContext = new ViewContext(
                viewContext,
                viewContext.View,
                newViewData,
                viewContext.Writer);

#if NETCOREAPP3_0
            var helper = new HtmlHelper<TModel>(
                viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IViewBufferScope>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<HtmlEncoder>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<UrlEncoder>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<ModelExpressionProvider>());
#else
              var helper = new HtmlHelper<TModel>(
                viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IViewBufferScope>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<HtmlEncoder>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<UrlEncoder>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<ExpressionTextCache>());
#endif

            helper.Contextualize(newViewContext);

            return helper;
        }
    }
}
