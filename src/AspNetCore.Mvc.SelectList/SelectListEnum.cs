using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListEnumAttribute : SelectListAttribute
    {
        public Type EnumType { get; set; }
        public SelectListEnumAttribute()
        {

        }

        public SelectListEnumAttribute(Type enumType)
        {
            EnumType = enumType;
        }

        protected override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            return Task.FromResult(context.Html.GetEnumSelectList(EnumType ?? context.Metadata.ElementType ?? context.ModelType));
        }
    }
}
