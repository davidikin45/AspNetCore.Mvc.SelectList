using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

        #region Helpers
        private static void AddValidationAttributes(
            ViewContext viewContext,
            TagBuilder tagBuilder,
            ModelExplorer modelExplorer,
            string expression)
        {
            var metadataProvider = viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            var validationAttributeProvider = viewContext.HttpContext.RequestServices.GetRequiredService<ValidationHtmlAttributeProvider>();

            modelExplorer = modelExplorer ?? Internal.ExpressionMetadataProvider.FromStringExpression(
                expression,
                viewContext.ViewData,
                metadataProvider);

            validationAttributeProvider.AddAndTrackValidationAttributes(
                viewContext,
                modelExplorer,
                expression,
                tagBuilder.Attributes);
        }

        internal static object GetModelStateValue(ViewContext viewContext, string key, Type destinationType)
        {
            if (viewContext.ViewData.ModelState.TryGetValue(key, out var entry) && entry.RawValue != null)
            {
                return ModelBindingHelper.ConvertTo(entry.RawValue, destinationType, culture: null);
            }

            return null;
        }

        private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributeDictionary = null;
            if (htmlAttributes != null)
            {
                htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
                if (htmlAttributeDictionary == null)
                {
                    htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                }
            }

            return htmlAttributeDictionary;
        }
        private static object GetAnonymousObject(IDictionary<string, object> dict)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            if (dict != null)
            {
                foreach (var kvp in dict)
                {
                    eoColl.Add(kvp);
                }
            }

            return eo;
        }
        #endregion

        #region Checkbox Boolean
        public static TagBuilder GenerateCheckboxBoolean(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string text, bool? isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();

            var div = new TagBuilder("div");
            div.MergeAttributes(GetHtmlAttributeDictionaryOrNull(divHtmlAttributes));

            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
            var id = NameAndIdProvider.CreateSanitizedId(viewContext, name, htmlGenerator.IdAttributeDotReplacement);

            var input = htmlGenerator.GenerateCheckBox(viewContext, modelExplorer, expression, isChecked, inputHtmlAttributes);

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));
            label.InnerHtml.AppendHtml(text);

            div.InnerHtml.AppendHtml(input);
            if (!string.IsNullOrEmpty(text))
            {
                div.InnerHtml.AppendHtml(label);
            }

            return div;
        }
        #endregion

        #region Checkbox Boolean Button
        public static TagBuilder GenerateCheckboxBooleanButton(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string text, bool? isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);

            var div = new TagBuilder("div");
            div.MergeAttributes(GetHtmlAttributeDictionaryOrNull(divHtmlAttributes));

            var input = htmlGenerator.GenerateCheckBox(viewContext, modelExplorer, expression, isChecked, inputHtmlAttributes);
            var inputChecked = input.Render().Contains("checked");

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));

            if (inputChecked)
            {
                label.AddCssClass("active");
            }

            label.InnerHtml.AppendHtml(input);
            label.InnerHtml.AppendHtml(text);

            div.InnerHtml.AppendHtml(label);

            return div;
        }
        #endregion

        #region Checkbox Value List
        public static HtmlContentBuilder GenerateCheckboxValueList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, IEnumerable<SelectListItem> selectList, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var currentValues = htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, true);
            return GenerateCheckboxValueList(viewContext, modelExplorer, expression, inline, selectList, currentValues, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static HtmlContentBuilder GenerateCheckboxValueList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {

            selectList = GetSelectListItems(viewContext, modelExplorer, expression, selectList, currentValues);

            var listItemBuilder = new HtmlContentBuilder();
            foreach (var item in selectList)
            {
                var selected = item.Selected;
                if (currentValues != null)
                {
                    var value = item.Value ?? item.Text;
                    selected = currentValues.Contains(value);
                }
                listItemBuilder.AppendHtml(GenerateCheckboxValue(viewContext, null, expression, inline, item.Value, item.Text, selected, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes));
            }

            return listItemBuilder;
        }

        public static TagBuilder GenerateCheckboxValue(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, string value, string text, bool? isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var div = new TagBuilder("div");
            div.MergeAttributes(GetHtmlAttributeDictionaryOrNull(divHtmlAttributes));

            if (inline)
            {
                div.AddCssClass("form-check-inline");
            }
            else
            {
                div.AddCssClass("form-check");
            }

            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
            var id = NameAndIdProvider.CreateSanitizedId(viewContext, name, htmlGenerator.IdAttributeDotReplacement);

            var input = new TagBuilder("input");
            input.MergeAttributes(GetHtmlAttributeDictionaryOrNull(inputHtmlAttributes));
            input.MergeAttribute("id", id, true);
            input.MergeAttribute("name", name, true);
            input.MergeAttribute("type", "checkbox", true);
            input.MergeAttribute("value", value, false);

            input.AddCssClass("form-check-input");

            if (modelExplorer != null)
            {
                if (modelExplorer.Model != null)
                {
                    isChecked = string.Equals(modelExplorer.Model.ToString(), value, StringComparison.Ordinal);
                }
            }

            if (GetModelStateValue(viewContext, name, typeof(string[])) is string[] modelStateValues)
            {
                isChecked = modelStateValues.Any(v => string.Equals(v, value, StringComparison.Ordinal));
            }
            else if (GetModelStateValue(viewContext, name, typeof(string)) is string modelStateValue)
            {
                isChecked = string.Equals(modelStateValue, value, StringComparison.Ordinal);
            }

            if (isChecked.HasValue && isChecked.Value)
            {
                input.MergeAttribute("checked", "checked");
            }

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));
            label.AddCssClass("form-check-label");

            label.InnerHtml.AppendHtml(text);

            div.InnerHtml.AppendHtml(input);
            if (!string.IsNullOrEmpty(text))
            {
                div.InnerHtml.AppendHtml(label);
            }


            // If there are any errors for a named field, we add the CSS attribute.
            if (viewContext.ViewData.ModelState.TryGetValue(name, out var entry) && entry.Errors.Count > 0)
            {
                input.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }

            AddValidationAttributes(viewContext, input, modelExplorer, expression);

            return div;
        }
        #endregion

        #region Checkbox Value Button List
        public static TagBuilder GenerateCheckboxValueButtonList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, IEnumerable<SelectListItem> selectList, bool group, object spanHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var currentValues = htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, true);
            return GenerateCheckboxValueButtonList(viewContext, modelExplorer, expression, selectList, currentValues, group, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static TagBuilder GenerateCheckboxValueButtonList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, bool group, object spanHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            selectList = GetSelectListItems(viewContext, modelExplorer, expression, selectList, currentValues);

            var span = new TagBuilder("span");
            span.MergeAttributes(GetHtmlAttributeDictionaryOrNull(spanHtmlAttributes));
            span.AddCssClass("btn-group-toggle");
            span.MergeAttribute("data-toggle", "buttons", true);

            if (group)
            {
                span.AddCssClass("btn-group");
            }

            var listItemBuilder = new HtmlContentBuilder();

            foreach (var item in selectList)
            {
                var selected = item.Selected;
                if (currentValues != null)
                {
                    var value = item.Value ?? item.Text;
                    selected = currentValues.Contains(value);
                }

                listItemBuilder.AppendHtml(GenerateCheckboxValueButton(viewContext, null, expression, item.Value, item.Text, selected, inputHtmlAttributes, labelHtmlAttributes));
            }

            span.InnerHtml.SetHtmlContent(listItemBuilder);

            return span;
        }

        public static TagBuilder GenerateCheckboxValueButton(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string value, string text, bool? isChecked, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
            var id = NameAndIdProvider.CreateSanitizedId(viewContext, name, htmlGenerator.IdAttributeDotReplacement);

            var input = new TagBuilder("input");
            input.MergeAttributes(GetHtmlAttributeDictionaryOrNull(inputHtmlAttributes));
            input.MergeAttribute("autocomplete", "off", false);
            input.MergeAttribute("id", id, true);
            input.MergeAttribute("name", name, false);
            input.MergeAttribute("type", "checkbox", true);
            input.MergeAttribute("value", value, false);

            if (modelExplorer != null)
            {
                if (modelExplorer.Model != null)
                {
                    isChecked = string.Equals(modelExplorer.Model.ToString(), value, StringComparison.Ordinal);
                }
            }

            if (GetModelStateValue(viewContext, name, typeof(string[])) is string[] modelStateValues)
            {
                isChecked = modelStateValues.Any(v => string.Equals(v, value, StringComparison.Ordinal));
            }
            else if (GetModelStateValue(viewContext, name, typeof(string)) is string modelStateValue)
            {
                isChecked = string.Equals(modelStateValue, value, StringComparison.Ordinal);
            }

            if (isChecked.HasValue && isChecked.Value)
            {
                input.MergeAttribute("checked", "checked");
            }

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));
            label.MergeAttribute("class", "btn btn-outline-secondary mr-2 mb-2 btn-sm", false);

            if (isChecked.HasValue && isChecked.Value)
            {
                label.AddCssClass("active");
            }
            label.InnerHtml.AppendHtml(input);
            label.InnerHtml.AppendHtml(text);

            // If there are any errors for a named field, we add the CSS attribute.
            if (viewContext.ViewData.ModelState.TryGetValue(name, out var entry) && entry.Errors.Count > 0)
            {
                input.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }

            AddValidationAttributes(viewContext, input, modelExplorer, expression);

            return label;
        }
        #endregion

        #region Radio Value List
        public static HtmlContentBuilder GenerateRadioValueList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, IEnumerable<SelectListItem> selectList, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var currentValues = htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, false);
            return GenerateRadioValueList(viewContext, modelExplorer, expression, inline, selectList, currentValues, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static HtmlContentBuilder GenerateRadioValueList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            selectList = GetSelectListItems(viewContext, modelExplorer, expression, selectList, currentValues);

            var listItemBuilder = new HtmlContentBuilder();
            foreach (var item in selectList)
            {
                var selected = item.Selected;
                if (currentValues != null)
                {
                    var value = item.Value ?? item.Text;
                    selected = currentValues.Contains(value);
                }
                listItemBuilder.AppendHtml(GenerateRadioValue(viewContext, null, expression, inline, item.Value, item.Text, selected, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes));
            }

            return listItemBuilder;
        }

        public static TagBuilder GenerateRadioValue(ViewContext viewContext, ModelExplorer modelExplorer, string expression, bool inline, string value, string text, bool? isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);
            // var radioHtml = htmlHelper.RadioButton(expression, value, isChecked, htmlAttributes).Render();

            var div = new TagBuilder("div");
            div.MergeAttributes(GetHtmlAttributeDictionaryOrNull(divHtmlAttributes));

            if (inline)
            {
                div.AddCssClass("form-check-inline");
            }
            else
            {
                div.AddCssClass("form-check");
            }

            var inputHtmlAttributesDict = GetHtmlAttributeDictionaryOrNull(inputHtmlAttributes) ?? new Dictionary<string, object>();
            if (!inputHtmlAttributesDict.ContainsKey("class"))
                inputHtmlAttributesDict.Add("class", "form-check-input");
            else
                inputHtmlAttributesDict["class"] = inputHtmlAttributesDict["class"] + " " + "form-check-input";

            inputHtmlAttributes = GetAnonymousObject(inputHtmlAttributesDict);

            var input = htmlGenerator.GenerateRadioButton(viewContext, modelExplorer, expression, value, isChecked, inputHtmlAttributes);

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));
            label.InnerHtml.AppendHtml(text);

            label.AddCssClass("form-check-label");

            div.InnerHtml.AppendHtml(input);

            if (!string.IsNullOrEmpty(text))
            {
                div.InnerHtml.AppendHtml(label);
            }

            return div;
        }
        #endregion

        #region Radio Value Button List
        public static TagBuilder GenerateRadioValueButtonList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, IEnumerable<SelectListItem> selectList, bool group, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var currentValues = htmlGenerator.GetCurrentValues(viewContext, modelExplorer, expression, false);
            return GenerateRadioValueButtonList(viewContext, modelExplorer, expression, selectList, currentValues, group, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static TagBuilder GenerateRadioValueButtonList(ViewContext viewContext, ModelExplorer modelExplorer, string expression, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, bool group, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();

            selectList = GetSelectListItems(viewContext, modelExplorer, expression, selectList, currentValues);

            var div = new TagBuilder("span");
            div.MergeAttributes(GetHtmlAttributeDictionaryOrNull(divHtmlAttributes));
            div.AddCssClass("btn-group-toggle");
            div.MergeAttribute("data-toggle", "buttons", true);

            if (group)
            {
                div.AddCssClass("btn-group");
            }

            foreach (var item in selectList)
            {
                var selected = item.Selected;
                if (currentValues != null)
                {
                    var value = item.Value ?? item.Text;
                    selected = currentValues.Contains(value);
                }
                div.InnerHtml.AppendHtml(GenerateRadioValueButton(viewContext, null, expression, item.Value, item.Text, selected, inputHtmlAttributes, labelHtmlAttributes));
            }

            return div;
        }

        public static TagBuilder GenerateRadioValueButton(ViewContext viewContext, ModelExplorer modelExplorer, string expression, string value, string text, bool? isChecked, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var htmlGenerator = viewContext.HttpContext.RequestServices.GetRequiredService<IHtmlGenerator>();
            var name = NameAndIdProvider.GetFullHtmlFieldName(viewContext, expression);

            var inputHtmlAttributesDict = GetHtmlAttributeDictionaryOrNull(inputHtmlAttributes) ?? new Dictionary<string, object>();
            if (!inputHtmlAttributesDict.ContainsKey("autocomplete"))
                inputHtmlAttributesDict.Add("autocomplete", "off");
            if (!inputHtmlAttributesDict.ContainsKey("class"))
                inputHtmlAttributesDict.Add("class", "");

            inputHtmlAttributes = GetAnonymousObject(inputHtmlAttributesDict);

            var input = htmlGenerator.GenerateRadioButton(viewContext, modelExplorer, expression, value, isChecked, inputHtmlAttributes);
            var inputChecked = input.Render().Contains("checked");

            //var checkboxHtml = htmlHelper.CheckBox(expression, isChecked, htmlAttributes).Render().Replace("true", value);

            var label = new TagBuilder("label");
            label.MergeAttributes(GetHtmlAttributeDictionaryOrNull(labelHtmlAttributes));


            label.MergeAttribute("class", "btn btn-outline-secondary mr-2 mb-2 btn-sm", false);

            if (inputChecked)
            {
                label.AddCssClass("active");
            }

            label.InnerHtml.AppendHtml(input);
            label.InnerHtml.AppendHtml(text);

            return label;
        }
        #endregion

        public TagBuilder GenerateSelect(ViewContext viewContext, ModelExplorer modelExplorer, string optionLabel, string expression, IEnumerable<SelectListItem> selectList, bool allowMultiple, object htmlAttributes)
        {
            return _htmlGenerator.GenerateSelect(viewContext, modelExplorer, optionLabel, expression, selectList, allowMultiple, htmlAttributes);
        }

        public TagBuilder GenerateSelect(ViewContext viewContext, ModelExplorer modelExplorer, string optionLabel, string expression, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues, bool allowMultiple, object htmlAttributes)
        {
            selectList = GetSelectListItems(viewContext, modelExplorer, expression, selectList, currentValues);

            return _htmlGenerator.GenerateSelect(viewContext, modelExplorer, optionLabel, expression, selectList, currentValues, allowMultiple, htmlAttributes);
        }

        public static IEnumerable<SelectListItem> GetSelectListItems(ViewContext viewContext, ModelExplorer modelExplorer, string expression, IEnumerable<SelectListItem> selectList, ICollection<string> currentValues)
        {
            //Emtpty List when coming from Select TagHelper
            if (selectList == null || selectList.ToList().Count == 0)
            {

                bool selectFromAttribute = true;
                if (selectList == null || selectList.ToList().Count == 0)
                {
                    selectList = GetSelectListItems(viewContext, expression);
                    selectFromAttribute = selectList == null;
                }

                if (selectFromAttribute)
                {
                    modelExplorer = modelExplorer ?? Internal.ExpressionMetadataProvider.FromStringExpression(expression, viewContext.ViewData, viewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>());
                    var selectListItems = GetSelectListItems(viewContext, modelExplorer, expression, currentValues);
                    if (selectListItems != null)
                    {
                        selectList = selectListItems;
                    }
                }
            }

            return selectList;
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
                return null;
            }

            // Second check the Eval() call returned a collection of SelectListItems.
            if (!(value is IEnumerable<SelectListItem> selectList))
            {
                return null;
            }

            return selectList;
        }

        private static IEnumerable<SelectListItem> GetSelectListItems(ViewContext viewContext, ModelExplorer modelExplorer, string expression, ICollection<string> currentValues)
        {
            if (modelExplorer.Metadata is DefaultModelMetadata defaultModelMetadata)
            {
                if (defaultModelMetadata.Attributes.PropertyAttributes != null)
                { 
                    var selectListAttribute = defaultModelMetadata.Attributes.PropertyAttributes.OfType<SelectListAttribute>().Where(a => string.IsNullOrEmpty(a.SelectListId)).FirstOrDefault();

                    if (selectListAttribute != null)
                    {
                        var selectListItems = selectListAttribute.GetSelectListAsync(new SelectListContext(null, viewContext, modelExplorer, expression, currentValues, false)).GetAwaiter().GetResult();

                        return selectListItems;
                    }
                }
            }

            return null;
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
