using AspNetCore.Mvc.SelectList.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AspNetCore.Mvc.SelectList
{
    public class ModelMultiSelectList : IEnumerable<ModelSelectListItem>
    {
        private readonly IHtmlHelper _html;  
        private readonly IList<SelectListGroup> _groups;
        private IList<ModelSelectListItem> _selectListItems;

        public ModelMultiSelectList(IHtmlHelper html, IEnumerable items)
            : this(html, items, selectedValues: null)
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

        public ModelMultiSelectList(IHtmlHelper html, IEnumerable items, IEnumerable selectedValues)
            : this(html, items, dataValueFieldExpression: null, dataTextFieldExpression: null, selectedValues: selectedValues)
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

        public ModelMultiSelectList(IHtmlHelper html, IEnumerable items, string dataValueFieldExpression, string dataTextFieldExpression)
            : this(html, items, dataValueFieldExpression, dataTextFieldExpression, selectedValues: null)
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

        public ModelMultiSelectList(
            IHtmlHelper html,
            IEnumerable items,
            string dataValueFieldExpression,
            string dataTextFieldExpression,
            IEnumerable selectedValues)
            : this(html, items, dataValueFieldExpression, dataTextFieldExpression, selectedValues, dataGroupFieldExpression: null)
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


        public ModelMultiSelectList(
            IHtmlHelper html,
            IEnumerable items,
            string dataValueFieldExpression,
            string dataTextFieldExpression,
            IEnumerable selectedValues,
            string dataGroupFieldExpression)
        {
            if (html == null)
            {
                throw new ArgumentNullException(nameof(html));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _html = html;
            Items = items;
            DataValueFieldExpression = dataValueFieldExpression;
            DataTextFieldExpression = dataTextFieldExpression;
            SelectedValues = selectedValues;
            DataGroupFieldExpression = dataGroupFieldExpression;

            if (DataGroupFieldExpression != null)
            {
                _groups = new List<SelectListGroup>();
            }
        }

        /// <summary>
        /// Gets or sets the data group field.
        /// </summary>
        public string DataGroupFieldExpression { get; }

        public string DataTextFieldExpression { get; }

        public string DataValueFieldExpression { get; }

        public IEnumerable Items { get; }

        public IEnumerable SelectedValues { get; }

        public virtual IEnumerator<ModelSelectListItem> GetEnumerator()
        {
            if (_selectListItems == null)
            {
                _selectListItems = GetListItems();
            }

            return _selectListItems.GetEnumerator();
        }

        private IList<ModelSelectListItem> GetListItems()
        {
            return (!string.IsNullOrEmpty(DataValueFieldExpression)) ?
                GetListItemsWithValueField() :
                GetListItemsWithoutValueField();
        }

        private IList<ModelSelectListItem> GetListItemsWithValueField()
        {
            var selectedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (SelectedValues != null)
            {
                foreach (var value in SelectedValues)
                {
                    var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
                    selectedValues.Add(stringValue);
                }
            }

            var listItems = new List<ModelSelectListItem>();
            foreach (var item in Items)
            {
                var html = Internal.HtmlHelperExtensions.For(_html, (dynamic)item);

                var value = Eval(html, item, DataValueFieldExpression);
                var newListItem = new ModelSelectListItem
                {
                    Model = item,
                    Html = html,
                    Group = GetGroup(html, item),
                    Value = value,
                    Text = Eval(html, item, DataTextFieldExpression),
                    Selected = selectedValues.Contains(value),
                };

                listItems.Add(newListItem);
            }

            return listItems;
        }

        private IList<ModelSelectListItem> GetListItemsWithoutValueField()
        {
            var selectedValues = new HashSet<object>();
            if (SelectedValues != null)
            {
                selectedValues.UnionWith(SelectedValues.Cast<object>());
            }

            var listItems = new List<ModelSelectListItem>();
            foreach (var item in Items)
            {
                var html = Internal.HtmlHelperExtensions.For(_html, (dynamic)item);

                var text = Eval(html, item, DataTextFieldExpression);

                var newListItem = new ModelSelectListItem
                {
                    Model = item,
                    Html = html,
                    Group = GetGroup(html, item),
                    Text = text,
                    Value = text,
                    Selected = selectedValues.Contains(item),
                };

                listItems.Add(newListItem);
            }

            return listItems;
        }

        public static string Eval(IHtmlHelper html, object container, string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var stringValue = expression;

                if (!stringValue.Contains("{") && !stringValue.Contains(" "))
                {
                    stringValue = "{" + stringValue + "}";
                }

                var replacementTokens = GetReplacementTokens(stringValue);

                foreach (var token in replacementTokens)
                {
                    var propertyName = token.Substring(1, token.Length - 2);
                    var displayString = html.Display(propertyName).Render();
                    stringValue = stringValue.Replace(token, displayString);
                }

                return stringValue;
            }
            else
            {
                return html.Display(string.Empty).Render();
                //return Convert.ToString(value, CultureInfo.CurrentCulture);
            }
        }

        private static List<String> GetReplacementTokens(String str)
        {
            Regex regex = new Regex(@"{(.*?)}", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(str);

            // Results include braces (undesirable)
            return matches.Cast<Match>().Select(m => m.Value).Distinct().ToList();
        }

        private SelectListGroup GetGroup(IHtmlHelper html, object container)
        {
            if (_groups == null)
            {
                return null;
            }

            var groupName = Eval(html, container, DataGroupFieldExpression);
            if (string.IsNullOrEmpty(groupName))
            {
                return null;
            }

            // We use StringComparison.CurrentCulture because the group name is used to display as the value of
            // optgroup HTML tag's label attribute.
            SelectListGroup group = null;
            for (var index = 0; index < _groups.Count; index++)
            {
                if (string.Equals(_groups[index].Name, groupName, StringComparison.CurrentCulture))
                {
                    group = _groups[index];
                    break;
                }
            }

            if (group == null)
            {
                group = new SelectListGroup() { Name = groupName };
                _groups.Add(group);
            }

            return group;
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
