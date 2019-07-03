using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
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
            if(context.Model != null)
                return Task.FromResult(new ModelMultiSelectList(context.Html, (IEnumerable)context.Model, DataValueField, DataTextFieldExpression, (IEnumerable)context.Model).AsEnumerable<SelectListItem>());
            else
                return Task.FromResult(Enumerable.Empty<SelectListItem>());
        }
    }
}
