using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public static class HtmlHelperExtensions
    {
        public static IEnumerable<SelectListItem> SelectListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string selectListId = null, bool selectedOnly = false)
        {
            var result = htmlHelper.SelectListForAsync(expression, selectListId, selectedOnly).GetAwaiter().GetResult();
            return result;
        }

        public static async Task<IEnumerable<SelectListItem>> SelectListForAsync<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string selectListId = null, bool selectedOnly = false)
        {
            //pass modelexplorer
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return (await GenerateSelectListAsync(htmlHelper, modelExpression.ModelExplorer, modelExpression.Name, selectListId, selectedOnly)) ?? GetSelectListItems(htmlHelper.ViewContext, modelExpression.Name);
        }

        private static ModelExpression GetModelExpression<TModel,TResult>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
        {
            var modelExpressionProvider = new ModelExpressionProvider(htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>(),
                htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ExpressionTextCache>());
            return modelExpressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);
        }

        public static IEnumerable<SelectListItem> SelectList(this IHtmlHelper htmlHelper, string expression, string selectListId = null, bool selectedOnly = false)
        {
            //dont pass modelexplorer
            return htmlHelper.SelectListAsync(expression, selectListId, selectedOnly).GetAwaiter().GetResult();
        }

        public static async Task<IEnumerable<SelectListItem>> SelectListAsync(this IHtmlHelper htmlHelper, string expression, string selectListId = null, bool selectedOnly = false)
        {
            //dont pass modelexplorer
            return (await GenerateSelectListAsync(htmlHelper, null, expression, selectListId, selectedOnly)) ?? GetSelectListItems(htmlHelper.ViewContext, expression);
        }

        private static async Task<IEnumerable<SelectListItem>> GenerateSelectListAsync(IHtmlHelper htmlHelper, ModelExplorer modelExplorer, string expression, string selectListId = null, bool selectedOnly = false)
        {
            var modelExplorerToExtractAttribute = ExpressionMetadataProvider.FromStringExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);

            if (!(modelExplorerToExtractAttribute.Metadata is DefaultModelMetadata defaultModelMetadata))
                return null;

            var selectListAttribute = defaultModelMetadata.Attributes.PropertyAttributes.OfType<SelectListAttribute>().Where(a => a.SelectListId == selectListId).FirstOrDefault();

            if (selectListAttribute == null)
                return null;

            return await selectListAttribute.GetSelectListAsync(new SelectListContext(htmlHelper.ViewContext, modelExplorer, expression, selectedOnly));
        }

        private static IEnumerable<SelectListItem> GetSelectListItems(
            ViewContext viewContext,
            string expression)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            // Method is called only if user did not pass a select list in. They must provide select list items in the
            // ViewData dictionary and definitely not as the Model. (Even if the Model datatype were correct, a
            // <select> element generated for a collection of SelectListItems would be useless.)
            var value = viewContext.ViewData.Eval(expression);

            // First check whether above evaluation was successful and did not match ViewData.Model.
            if (value == null || value == viewContext.ViewData.Model)
            {
                throw new InvalidOperationException($"Must be IEnumerable<{nameof(SelectListItem)}>");
            }

            // Second check the Eval() call returned a collection of SelectListItems.
            if (!(value is IEnumerable<SelectListItem> selectList))
            {
                throw new InvalidOperationException($"Must be IEnumerable<{nameof(SelectListItem)}>");
            }

            return selectList;
        }
    }
}
