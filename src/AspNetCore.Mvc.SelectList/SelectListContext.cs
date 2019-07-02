using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListContext
    {
        public IHtmlHelper Html { get;}

        public dynamic Model { get { return ModelExplorer.Model; } }

        public HttpContext HttpContext { get { return Html.ViewContext.HttpContext; } }

        public Type ModelType { get { return ModelExplorer.Metadata.UnderlyingOrModelType; } }

        public ICollection<string> CurrentValues { get; }

        public ModelExplorer ModelExplorer { get; }

        public ModelMetadata Metadata { get { return ModelExplorer.Metadata; } }

        public bool SelectedOnly { get; }

        public SelectListContext(IHtmlHelper html, ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool selectedOnly)
        :this(html, viewContext, modelExplorer, expression, GetCurrentValues(viewContext, modelExplorer, expression), selectedOnly)
        {

        }

         public SelectListContext(IHtmlHelper html, ViewContext viewContext, ModelExplorer modelExplorer, string expression, ICollection<string> currentValues, bool selectedOnly)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            CurrentValues = currentValues;
            //IdFor
            ModelExplorer = modelExplorer ?? ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>()); ;
            Html = html ?? CreateHtmlHelper(viewContext);
            SelectedOnly = selectedOnly;
        }

        private static IHtmlHelper CreateHtmlHelper(ViewContext viewContext)
        {
            var helper = new HtmlHelper(
                viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<IViewBufferScope>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<HtmlEncoder>(),
                viewContext.HttpContext.RequestServices.GetRequiredService<UrlEncoder>());

            helper.Contextualize(viewContext);

            return helper;
        }

        //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.ViewFeatures/src/DefaultHtmlGenerator.cs
        private static ICollection<string> GetCurrentValues(
           ViewContext viewContext,
           ModelExplorer modelExplorer,
           string expression)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var modelExplorerForMetadata = modelExplorer ?? ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>());

            if(modelExplorerForMetadata.Metadata.IsEnumerableType)
            {
                return htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, true);
            }
            else
            {
                return htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, false);
            }
        }

        public string Display(dynamic obj, string displayExpression)
        {
            string value = displayExpression;

            if (!value.Contains("{") && !value.Contains(" "))
            {
                value = "{" + value + "}";
            }

            var replacementTokens = GetReplacementTokens(value);
            foreach (var token in replacementTokens)
            {
                var propertyName = token.Substring(1, token.Length - 2);
                var displayString = ((IHtmlContent)ModelHelperExtensions.Display(Html, obj, propertyName)).Render();
                value = value.Replace(token, displayString);
            }

            return value;
        }

        private List<String> GetReplacementTokens(String str)
        {
            Regex regex = new Regex(@"{(.*?)}", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(str);

            // Results include braces (undesirable)
            return matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }
    }
}
