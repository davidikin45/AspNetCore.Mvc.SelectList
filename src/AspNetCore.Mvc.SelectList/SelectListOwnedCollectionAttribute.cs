using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListOwnedCollectionAttribute : SelectListAttribute
    {
        public SelectListOwnedCollectionAttribute(string dataTextFieldExpression = "Id")
        {
            DataTextFieldExpression = dataTextFieldExpression;
            SetSelectedAutomatically = false;
        }

        public string DataTextFieldExpression { get; set; } = "Id";
        public string DataValueField { get; set; } = "Id";

        protected override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            var results = new List<SelectListItem>();
            foreach (var item in context.Model)
            {
                var itemObject = (object)item;

                results.Add(new ModelSelectListItem()
                {
                    Model = item,
                    Html = Internal.HtmlHelperExtensions.For(context.Html, (dynamic)item),
                    Text = context.Display(item, DataTextFieldExpression),
                    Value = item.GetPropValue(DataValueField) != null ? item.GetPropValue(DataValueField).ToString() : "",
                    Selected = true
                });     
            }

            return Task.FromResult(results.AsEnumerable());
        }
    }
}
