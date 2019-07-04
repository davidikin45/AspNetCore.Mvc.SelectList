using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;

namespace AspNetCore.Mvc.SelectList
{
    public class ModelSelectList : ModelMultiSelectList
    {
        public ModelSelectList(IHtmlHelper html, IEnumerable items)
            : this(html, items, selectedValue: null)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }
            
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
        }

        public ModelSelectList(IHtmlHelper html, IEnumerable items, object selectedValue)
            : this(html, items, dataValueFieldExpression: null, dataTextFieldExpression: null, selectedValue: selectedValue)
        {

            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
        }

        public ModelSelectList(IHtmlHelper html, IEnumerable items, string dataValueFieldExpression, string dataTextFieldExpression)
            : this(html, items, dataValueFieldExpression, dataTextFieldExpression, selectedValue: null)
        {

            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
        }

        public ModelSelectList(
            IHtmlHelper html,
            IEnumerable items,
            string dataValueFieldExpression,
            string dataTextFieldExpression,
            object selectedValue)
            : base(html, items, dataValueFieldExpression, dataTextFieldExpression, ToEnumerable(selectedValue))
        {

            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            SelectedValue = selectedValue;
        }

        public ModelSelectList(
            IHtmlHelper html,
            IEnumerable items,
            string dataValueFieldExpression,
            string dataTextFieldExpression,
            object selectedValue,
            string dataGroupFieldExpression)
            : base(html, items, dataValueFieldExpression, dataTextFieldExpression, ToEnumerable(selectedValue), dataGroupFieldExpression)
        {

            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            SelectedValue = selectedValue;
        }

        public object SelectedValue { get; }

        private static IEnumerable ToEnumerable(object selectedValue)
        {
            return (selectedValue != null) ? new[] { selectedValue } : null;
        }
    }
}
