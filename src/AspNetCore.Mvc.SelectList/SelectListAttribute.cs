using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public abstract class SelectListAttribute : Attribute
    {
        public string SelectListId { get; set; }

        public bool Nullable { get; set; }

        public bool SetSelectedAutomatically { get; set; } = true;


        public async Task<IEnumerable<SelectListItem>> GetSelectListAsync(SelectListContext context)
        {
            var items = await GetSelectListItemsAsync(context);

            if (items == null)
                return null;

            var itemList = items.ToList();

            if (SetSelectedAutomatically)
            {
                if (context.CurrentValues != null)
                {
                    foreach (var item in itemList)
                    {
                        var value = item.Value ?? item.Text;
                        item.Selected = context.CurrentValues.Contains(value);
                    }
                }
            }

            if(context.SelectedOnly)
            {
                itemList.RemoveAll(item => !item.Selected);
            }

            if (context.ModelExplorer.Metadata.IsNullableValueType || Nullable)
            {
                if(!context.SelectedOnly)
                {
                    itemList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = SetSelectedAutomatically ? (context.CurrentValues == null || context.CurrentValues.Count == 0) : false});
                }
                else if(itemList.Count == 0)
                {
                    itemList.Insert(0, new SelectListItem { Text = "", Value = "", Selected = true });
                }
            }

            return itemList;
        }

        protected abstract Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context);
    }
}
