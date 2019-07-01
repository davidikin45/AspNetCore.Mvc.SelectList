using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListHtmlGenerator : IHtmlGenerator
    {
        private readonly IHtmlGenerator _htmlGenerator;
        public SelectListHtmlGenerator(IHtmlGenerator htmlGenerator)
        {
            _htmlGenerator = htmlGenerator;
        }

        public string IdAttributeDotReplacement => _htmlGenerator.IdAttributeDotReplacement;

        public string Encode(string value)
        {
            return _htmlGenerator.Encode(value);
        }

        public string Encode(object value)
        {
            return _htmlGenerator.Encode(value);
        }

        public string FormatValue(object value, string format)
        {
            return _htmlGenerator.FormatValue(value, format);
        }

        public TagBuilder GenerateActionLink(ViewContext viewContext, string linkText, string actionName, string controllerName, string protocol, string hostname, string fragment, object routeValues, object htmlAttributes)
        {
            return _htmlGenerator.GenerateActionLink(viewContext, linkText, actionName, controllerName, protocol, hostname, fragment, routeValues, htmlAttributes);
        }

        public IHtmlContent GenerateAntiforgery(ViewContext viewContext)
        {
            return _htmlGenerator.GenerateAntiforgery(viewContext);
        }

        public TagBuilder GenerateCheckBox(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool? isChecked, object htmlAttributes)
        {
            return _htmlGenerator.GenerateCheckBox(viewContext, modelExplorer, expression, isChecked, htmlAttributes);
        }

        public TagBuilder GenerateForm(ViewContext viewContext, string actionName, string controllerName, object routeValues, string method, object htmlAttributes)
        {
            return _htmlGenerator.GenerateForm(viewContext, actionName, controllerName, routeValues, method, htmlAttributes);
        }

        public IHtmlContent GenerateGroupsAndOptions(string optionLabel, IEnumerable<SelectListItem> selectList)
        {
            return _htmlGenerator.GenerateGroupsAndOptions(optionLabel, selectList);
        }

        public TagBuilder GenerateHidden(ViewContext viewContext, ModelExplorer modelExplorer, string expression, object value, bool useViewData, object htmlAttributes)
        {
            return _htmlGenerator.GenerateHidden(viewContext, modelExplorer, expression, value, useViewData, htmlAttributes);
        }

        public TagBuilder GenerateHiddenForCheckbox(ViewContext viewContext, ModelExplorer modelExplorer, string expression)
        {
            return _htmlGenerator.GenerateHiddenForCheckbox(viewContext, modelExplorer, expression);
        }

        public TagBuilder GenerateLabel(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string labelText, object htmlAttributes)
        {
            return _htmlGenerator.GenerateLabel(viewContext, modelExplorer, expression, labelText, htmlAttributes);
        }

        public TagBuilder GeneratePageForm(ViewContext viewContext, string pageName, string pageHandler, object routeValues, string fragment, string method, object htmlAttributes)
        {
            return _htmlGenerator.GeneratePageForm(viewContext, pageName, pageHandler, routeValues, fragment, method, htmlAttributes);
        }

        public TagBuilder GeneratePageLink(ViewContext viewContext, string linkText, string pageName, string pageHandler, string protocol, string hostname, string fragment, object routeValues, object htmlAttributes)
        {
            return _htmlGenerator.GeneratePageLink(viewContext, linkText, pageName, pageHandler, protocol, hostname, fragment, routeValues, htmlAttributes);
        }

        public TagBuilder GeneratePassword(ViewContext viewContext, ModelExplorer modelExplorer, string expression, object value, object htmlAttributes)
        {
            return _htmlGenerator.GeneratePassword(viewContext, modelExplorer, expression, value, htmlAttributes);
        }

        public TagBuilder GenerateRadioButton(ViewContext viewContext, ModelExplorer modelExplorer, string expression, object value, bool? isChecked, object htmlAttributes)
        {
            return _htmlGenerator.GenerateRadioButton(viewContext, modelExplorer, expression, value, isChecked, htmlAttributes);
        }

        public TagBuilder GenerateRouteForm(ViewContext viewContext, string routeName, object routeValues, string method, object htmlAttributes)
        {
            return _htmlGenerator.GenerateRouteForm(viewContext, routeName, routeValues, method, htmlAttributes);
        }

        public TagBuilder GenerateRouteLink(ViewContext viewContext, string linkText, string routeName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
        {
            return _htmlGenerator.GenerateRouteLink(viewContext, linkText, routeName, protocol, hostName, fragment, routeValues, htmlAttributes);
        }

        public TagBuilder GenerateSelect(ViewContext viewContext, ModelExplorer modelExplorer, string optionLabel, string expression, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, bool allowMultiple, object htmlAttributes)
        {
            if (selectList == null || selectList.ToList().Count == 0)
            {
                modelExplorer = modelExplorer ?? ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>());
                var selectListItems = GetSelectListItems(viewContext, modelExplorer, expression, currentValues);
                if(selectListItems != null)
                {
                    selectList = selectListItems;
                }
            }

            return _htmlGenerator.GenerateSelect(viewContext, modelExplorer, optionLabel, expression, selectList, currentValues, allowMultiple, htmlAttributes);
        }

        private IEnumerable<SelectListItem> GetSelectListItems(ViewContext viewContext, ModelExplorer modelExplorer, string expression, ICollection<string> currentValues)
        {
            if (modelExplorer.Metadata is DefaultModelMetadata defaultModelMetadata)
            {
                var selectListAttribute = defaultModelMetadata.Attributes.PropertyAttributes.OfType<SelectListAttribute>().Where(a => string.IsNullOrEmpty(a.SelectListId)).FirstOrDefault();

                if (selectListAttribute != null)
                {
                    var selectListItems = selectListAttribute.GetSelectListAsync(new SelectListContext(viewContext, modelExplorer, expression, currentValues, false)).GetAwaiter().GetResult();

                    return selectListItems;
                }

            }

            return null;
        }

        public TagBuilder GenerateSelect(ViewContext viewContext, ModelExplorer modelExplorer, string optionLabel, string expression, IEnumerable<SelectListItem> selectList, bool allowMultiple, object htmlAttributes)
        {
            return _htmlGenerator.GenerateSelect(viewContext, modelExplorer, optionLabel, expression, selectList, allowMultiple, htmlAttributes);
        }

        public TagBuilder GenerateTextArea(ViewContext viewContext, ModelExplorer modelExplorer, string expression, int rows, int columns, object htmlAttributes)
        {
            return _htmlGenerator.GenerateTextArea(viewContext, modelExplorer, expression, rows, columns, htmlAttributes);
        }

        public TagBuilder GenerateTextBox(ViewContext viewContext, ModelExplorer modelExplorer, string expression, object value, string format, object htmlAttributes)
        {
            return _htmlGenerator.GenerateTextBox(viewContext, modelExplorer, expression, value, format, htmlAttributes);
        }

        public TagBuilder GenerateValidationMessage(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string message, string tag, object htmlAttributes)
        {
            return _htmlGenerator.GenerateValidationMessage(viewContext, modelExplorer, expression, message, tag, htmlAttributes);
        }

        public TagBuilder GenerateValidationSummary(ViewContext viewContext, bool excludePropertyErrors, string message, string headerTag, object htmlAttributes)
        {
            return _htmlGenerator.GenerateValidationSummary(viewContext, excludePropertyErrors, message, headerTag, htmlAttributes);
        }

        public ICollection<string> GetCurrentValues(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool allowMultiple)
        {
            return _htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, allowMultiple);
        }

    }
}
