using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Mvc.SelectList.TagHelpers
{
    [HtmlTargetElement("radio-list", Attributes = ForAttributeName)]
    [HtmlTargetElement("radio-list", Attributes = ItemsAttributeName)]
    [HtmlTargetElement("radio-list", Attributes = "name")]
    public class RadioListTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string ItemsAttributeName = "asp-items";
        private bool _allowMultiple;
        private ICollection<string> _currentValues;

        /// <summary>
        /// Creates a new <see cref="SelectTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public RadioListTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order => -1000;

        /// <summary>
        /// Gets the <see cref="IHtmlGenerator"/> used to generate the <see cref="SelectTagHelper"/>'s output.
        /// </summary>
        protected IHtmlGenerator Generator { get; }

        /// <summary>
        /// Gets the <see cref="Rendering.ViewContext"/> of the executing view.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// A collection of <see cref="SelectListItem"/> objects used to populate the &lt;select&gt; element with
        /// &lt;optgroup&gt; and &lt;option&gt; elements.
        /// </summary>
        [HtmlAttributeName(ItemsAttributeName)]
        public IEnumerable<SelectListItem> Items { get; set; }

        [HtmlAttributeName("inline")]
        public bool Inline { get; set; } = true;

        /// <summary>
        /// The name of the &lt;input&gt; element.
        /// </summary>
        /// <remarks>
        /// Passed through to the generated HTML in all cases. Also used to determine whether <see cref="For"/> is
        /// valid with an empty <see cref="ModelExpression.Name"/>.
        /// </remarks>
        public string Name { get; set; }

        /// <inheritdoc />
        public override void Init(TagHelperContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (For == null)
            {
                var modelExplorer = ExpressionMetadataProvider.FromStringExpression(Name, ViewContext.ViewData, ViewContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>());
                var realModelType2 = modelExplorer.ModelType;
                _allowMultiple = typeof(string) != realModelType2 &&
                    typeof(IEnumerable).IsAssignableFrom(realModelType2);
                _currentValues = Generator.GetCurrentValues(ViewContext, null, Name, _allowMultiple);

                return;
            }

            // Note null or empty For.Name is allowed because TemplateInfo.HtmlFieldPrefix may be sufficient.
            // IHtmlGenerator will enforce name requirements.
            if (For.Metadata == null)
            {
                throw new InvalidOperationException("No metadata provided");
            }

            // Base allowMultiple on the instance or declared type of the expression to avoid a
            // "SelectExpressionNotEnumerable" InvalidOperationException during generation.
            // Metadata.IsEnumerableType is similar but does not take runtime type into account.
            var realModelType = For.ModelExplorer.ModelType;
            _allowMultiple = typeof(string) != realModelType &&
                typeof(IEnumerable).IsAssignableFrom(realModelType);
            _currentValues = Generator.GetCurrentValues(ViewContext, For.ModelExplorer, For.Name, _allowMultiple);

        }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }


            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;

            // Pass through attribute that is also a well-known HTML attribute. Must be done prior to any copying
            // from a TagBuilder.
            if (Name != null)
            {
                output.CopyHtmlAttribute(nameof(Name), context);
            }

            // Ensure GenerateSelect() _never_ looks anything up in ViewData.
            var items = Items ?? Enumerable.Empty<SelectListItem>();

            var labelAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            foreach (var attribute in output.Attributes)
            {
                labelAttributes.Add(attribute.Name, attribute.Value);
            }

            if (For == null)
            {
                var tagBuilder = SelectListHtmlGenerator.GenerateRadioValueList(
                         ViewContext,
                         For.ModelExplorer,
                         expression: Name,
                         inline: Inline,
                         selectList: items,
                         currentValues: _currentValues,
                         divHtmlAttributes: null,
                         inputHtmlAttributes: null,
                         labelAttributes);

                output.Content.SetHtmlContent(tagBuilder);

                return;
            }

            // Ensure Generator does not throw due to empty "fullName" if user provided a name attribute.
            IDictionary<string, object> inputAttributes = null;
            if (string.IsNullOrEmpty(For.Name) &&
                string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix) &&
                !string.IsNullOrEmpty(Name))
            {
                inputAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { "name", Name },
                };
            }

            var tagBuilder2 = SelectListHtmlGenerator.GenerateRadioValueList(
                    ViewContext,
                    For.ModelExplorer,
                    expression: For.Name,
                    inline: Inline,
                    selectList: items,
                    currentValues: _currentValues,
                    divHtmlAttributes: null,
                    inputHtmlAttributes: inputAttributes,
                    labelAttributes);

            output.Content.SetHtmlContent(tagBuilder2);
        }
    }
}
