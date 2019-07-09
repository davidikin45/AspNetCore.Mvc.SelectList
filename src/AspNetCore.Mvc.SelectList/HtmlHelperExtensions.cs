using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Html;
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
        #region SelectList
        public static IEnumerable<ModelSelectListItem<TModel>> SelectListForModelType<TModel>(this IHtmlHelper htmlHelper) where TModel : class, new()
        {
            return htmlHelper.SelectListForModelType<TModel>(new object[] { }, null);
        }

        public static IEnumerable<ModelSelectListItem<TModel>> SelectListForModelType<TModel>(this IHtmlHelper htmlHelper, object[] keys, string selectListId = null) where TModel : class, new()
        {
            var newhtmlHelper = htmlHelper.For<List<TModel>>();

            return newhtmlHelper.SelectListForModelTypeAsync<TModel>(keys, selectListId).GetAwaiter().GetResult();
        }

        public static async Task<IEnumerable<ModelSelectListItem<TModel>>> SelectListForModelTypeAsync<TModel>(this IHtmlHelper htmlHelper) where TModel : class, new()
        {
            return await htmlHelper.SelectListForModelTypeAsync<TModel>(new object[] { }, null);
        }

        public static async Task<IEnumerable<ModelSelectListItem<TModel>>> SelectListForModelTypeAsync<TModel>(this IHtmlHelper htmlHelper, object[] keys, string selectListId = null) where TModel : class, new()
        {
            var newhtmlHelper = htmlHelper.For<List<TModel>>();

            var fullName = NameAndIdProvider.GetFullHtmlFieldName(newhtmlHelper.ViewContext, "");
            var keyArray = keys.Select(key => key.ToString()).ToArray();
            newhtmlHelper.ViewContext.ViewData.ModelState.SetModelValue(fullName, keyArray, String.Join(",", keyArray));

            var result = await newhtmlHelper.SelectListAsync("", selectListId, keyArray.Length > 0 ? true : false);

            if (result != null)
                return result.Cast<ModelSelectListItem>().Select(item => new ModelSelectListItem<TModel>(item));

            return null;
        }

        public static IEnumerable<ModelSelectListItem> SelectListForModel(this IHtmlHelper htmlHelper, string selectListId = null)
        {
            return htmlHelper.SelectListForModelAsync(selectListId).GetAwaiter().GetResult();
        }

        public static async Task<IEnumerable<ModelSelectListItem>> SelectListForModelAsync(this IHtmlHelper htmlHelper, string selectListId = null)
        {
            var result = await htmlHelper.SelectListAsync("", selectListId, false);

            if (result != null)
                return result.Cast<ModelSelectListItem>();

            return null;
        }

        public static IEnumerable<SelectListItem> SelectListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string selectListId = null, bool selectedOnly = false)
        {
            var result = htmlHelper.SelectListForAsync(expression, selectListId, selectedOnly).GetAwaiter().GetResult();
            return result;
        }

        public static async Task<IEnumerable<SelectListItem>> SelectListForAsync<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string selectListId = null, bool selectedOnly = false)
        {
            //pass modelexplorer
            var modelExpression = GetModelExpression(htmlHelper, expression);
            if (selectListId == null)
            {
                return GetSelectListItems(htmlHelper.ViewContext, modelExpression.Name) ?? (await GenerateSelectListAsync(htmlHelper, modelExpression.ModelExplorer, modelExpression.Name, selectListId, selectedOnly));
            }
            else
            {
                return await GenerateSelectListAsync(htmlHelper, modelExpression.ModelExplorer, modelExpression.Name, selectListId, selectedOnly);
            }
        }

        private static ModelExpression GetModelExpression<TModel, TResult>(IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
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
            if(selectListId == null)
            {
                return GetSelectListItems(htmlHelper.ViewContext, expression) ?? (await GenerateSelectListAsync(htmlHelper, null, expression, selectListId, selectedOnly));
            }
            else
            {
                return await GenerateSelectListAsync(htmlHelper, null, expression, selectListId, selectedOnly);
            }
        }

        private static async Task<IEnumerable<SelectListItem>> GenerateSelectListAsync(IHtmlHelper htmlHelper, ModelExplorer modelExplorer, string expression, string selectListId = null, bool selectedOnly = false)
        {
            var modelExplorerToExtractAttribute = ExpressionMetadataProvider.FromStringExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);

            if (!(modelExplorerToExtractAttribute.Metadata is DefaultModelMetadata defaultModelMetadata))
                return null;

            SelectListAttribute selectListAttribute = null;
            if (modelExplorerToExtractAttribute.Metadata.MetadataKind == ModelMetadataKind.Property)
            {
                selectListAttribute = defaultModelMetadata.Attributes.PropertyAttributes.OfType<SelectListAttribute>().Where(a => a.SelectListId == selectListId).FirstOrDefault();
            }
            else if (modelExplorerToExtractAttribute.Metadata.MetadataKind == ModelMetadataKind.Type)
            {
                if(modelExplorerToExtractAttribute.Metadata.IsEnumerableType)
                {
                    if (!(modelExplorerToExtractAttribute.Metadata.ElementMetadata is DefaultModelMetadata collectionModelMetadata))
                        return null;

                    selectListAttribute = collectionModelMetadata.Attributes.TypeAttributes.OfType<SelectListAttribute>().Where(a => a.SelectListId == selectListId).FirstOrDefault();
                }
                else
                {
                    selectListAttribute = defaultModelMetadata.Attributes.TypeAttributes.OfType<SelectListAttribute>().Where(a => a.SelectListId == selectListId).FirstOrDefault();
                }
            }

            if (selectListAttribute == null)
                return null;

            return await selectListAttribute.GetSelectListAsync(new SelectListContext(htmlHelper, htmlHelper.ViewContext, modelExplorer, expression, selectedOnly));
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
        #endregion

        #region Boolean Checkbox
        public static IHtmlContent CheckboxBooleanFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string text, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxBoolean(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, text, null, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent CheckboxBoolean(this IHtmlHelper htmlHelper, string expression, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            return SelectListHtmlGenerator.GenerateCheckboxBoolean(htmlHelper.ViewContext, null, expression, text, isChecked, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Boolean Checkbox Button
        public static IHtmlContent CheckboxBooleanButtonFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string text, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxBooleanButton(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, text, null, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        public static IHtmlContent CheckboxBooleanButton(this IHtmlHelper htmlHelper, string expression, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            return SelectListHtmlGenerator.GenerateCheckboxBooleanButton(htmlHelper.ViewContext, null, expression, text, isChecked, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Checkbox List
        public static IHtmlContent CheckboxListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool inline = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxValueList(htmlHelper.ViewContext, modelExpression.ModelExplorer,  modelExpression.Name, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent CheckboxList(this IHtmlHelper htmlHelper, string expression, bool inline = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateCheckboxValueList(htmlHelper.ViewContext, null, expression, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent CheckboxValueFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool inline, string value, string text, bool isChecked, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxValue(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, inline, value, text, isChecked, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent CheckboxValue(this IHtmlHelper htmlHelper, string expression, bool inline, string value, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            return SelectListHtmlGenerator.GenerateCheckboxValue(htmlHelper.ViewContext, null, expression, inline, value, text, isChecked, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Checkbox Button List
        public static IHtmlContent CheckboxButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool group, IEnumerable<SelectListItem> items = null, object spanHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxValueButtonList(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, items, group, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        public static IHtmlContent CheckboxButtonList(this IHtmlHelper htmlHelper, string expression, bool group, IEnumerable<SelectListItem> items = null, object spanHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateCheckboxValueButtonList(htmlHelper.ViewContext, null, expression, items, group, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        public static IHtmlContent CheckboxValueButtonFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string value, string text, bool isChecked, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateCheckboxValueButton(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, value, text, isChecked, inputHtmlAttributes, labelHtmlAttributes);
        }
        public static IHtmlContent CheckboxValueButton(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateCheckboxValueButton(htmlHelper.ViewContext, null, expression, value, text, isChecked, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Radio List
        public static IHtmlContent RadioListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool inline = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateRadioValueList(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioList(this IHtmlHelper htmlHelper, string expression, bool inline = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateRadioValueList(htmlHelper.ViewContext, null, expression, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioValueFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool inline, string value, string text, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateRadioValue(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, inline, value, text, null, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        public static IHtmlContent RadioValue(this IHtmlHelper htmlHelper, string expression, bool inline, string value, string text, bool isChecked, object divHtmlAttributes, object inputHtmlAttributes, object labelHtmlAttributes)
        {
            return SelectListHtmlGenerator.GenerateRadioValue(htmlHelper.ViewContext, null, expression, inline, value, text, isChecked, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Radio Button List
        public static IHtmlContent RadioYesNoButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("Yes", "true", isChecked, false));
            items.Add(new SelectListItem("No", "false", !isChecked, false));

            return htmlHelper.RadioButtonListFor(expression, groupRadioButtons, items, divHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioYesNoButtonList(this IHtmlHelper htmlHelper, string expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("Yes", "true", isChecked, false));
            items.Add(new SelectListItem("No", "false", !isChecked, false));

            return htmlHelper.RadioButtonList(expression, groupRadioButtons, items, divHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioTrueFalseButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("True", "true", isChecked, false));
            items.Add(new SelectListItem("False", "false", !isChecked, false));

            return htmlHelper.RadioButtonListFor(expression, groupRadioButtons, items, divHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioTrueFalseButtonList(this IHtmlHelper htmlHelper, string expression, bool isChecked, bool groupRadioButtons, object divHtmlAttributes, object labelHtmlAttributes)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem("True", "true", isChecked, false));
            items.Add(new SelectListItem("False", "false", !isChecked, false));

            return htmlHelper.RadioButtonList(expression, groupRadioButtons, items, divHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool group = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateRadioValueButtonList(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, items, group, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioButtonList(this IHtmlHelper htmlHelper, string expression, bool groupRadioButtons = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateRadioValueButtonList(htmlHelper.ViewContext, null, expression, items, groupRadioButtons, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioValueButtonFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, string value, string text, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);
            return SelectListHtmlGenerator.GenerateRadioValueButton(htmlHelper.ViewContext, modelExpression.ModelExplorer, modelExpression.Name, value, text, null, inputHtmlAttributes, labelHtmlAttributes);
        }

        public static IHtmlContent RadioValueButton(this IHtmlHelper htmlHelper, string expression, string value, string text, bool isChecked = false, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            return SelectListHtmlGenerator.GenerateRadioValueButton(htmlHelper.ViewContext, null, expression, value, text, isChecked, inputHtmlAttributes, labelHtmlAttributes);
        }
        #endregion

        #region Dropdown or Listbox
        public static IHtmlContent DropDownListOrListBoxFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, IEnumerable<SelectListItem> items = null, object htmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);

            if (modelExpression.Metadata.IsEnumerableType)
            {
                return htmlHelper.ListBoxFor(expression, items, htmlAttributes);
            }
            else
            {
                return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            }
        }

        public static IHtmlContent DropDownListOrListBox(this IHtmlHelper htmlHelper, string propertyName, IEnumerable<SelectListItem> items = null, object htmlAttributes = null)
        {
            ModelMetadata metadata = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider).Metadata;

            if (metadata.IsEnumerableType)
            {
                return htmlHelper.ListBox(propertyName, items, htmlAttributes);
            }
            else
            {
                return htmlHelper.DropDownList(propertyName, items, htmlAttributes);
            }
        }
        #endregion

        #region Radio List or Checkbox List
        public static IHtmlContent RadioOrCheckboxListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool inline = true, IEnumerable<SelectListItem> items = null, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);

            if (modelExpression.Metadata.IsEnumerableType)
            {
                return htmlHelper.CheckboxListFor(expression, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
            else
            {
                return htmlHelper.RadioListFor(expression, inline, items);
            }
        }

        public static IHtmlContent RadioOrCheckboxList(this IHtmlHelper htmlHelper, string propertyName, bool inline = true, object divHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var items = htmlHelper.SelectList(propertyName);

            ModelMetadata metadata = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider).Metadata;

            if (metadata.IsEnumerableType)
            {
                return htmlHelper.CheckboxList(propertyName, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
            else
            {
                return htmlHelper.RadioList(propertyName, inline, items, divHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
        }

        #endregion

        #region  Radio Button List or Checkbox Button List
        public static IHtmlContent RadioOrCheckboxButtonListFor<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression, bool group, IEnumerable<SelectListItem> items = null, object spanHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            var modelExpression = GetModelExpression(htmlHelper, expression);

            if (modelExpression.Metadata.IsEnumerableType)
            {
                return htmlHelper.CheckboxButtonListFor(expression, group, items, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
            else
            {
                return htmlHelper.RadioButtonListFor(expression, group, items, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
        }

        public static IHtmlContent RadioOrCheckboxButtonList(this IHtmlHelper htmlHelper, string propertyName, bool group, IEnumerable<SelectListItem> items = null, object spanHtmlAttributes = null, object inputHtmlAttributes = null, object labelHtmlAttributes = null)
        {
            ModelMetadata metadata = ExpressionMetadataProvider.FromStringExpression(propertyName, htmlHelper.ViewData, htmlHelper.MetadataProvider).Metadata;

            if (metadata.IsEnumerableType)
            {
                return htmlHelper.CheckboxButtonList(propertyName, group, items, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
            else
            {
                return htmlHelper.RadioButtonList(propertyName, group, items, spanHtmlAttributes, inputHtmlAttributes, labelHtmlAttributes);
            }
        }
        #endregion
    }
}
