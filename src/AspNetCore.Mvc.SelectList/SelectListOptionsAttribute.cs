using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListOptionsAttribute : SelectListAttribute
    {
        private readonly object[] _text;
        private readonly object[] _values;

        public SelectListOptionsAttribute(params object[] text)
            :this(text, text)
        {

        }

        public SelectListOptionsAttribute(object[] text, object[] values)
        {
            if (text.Length != values.Length)
                throw new Exception("text and values length must be the same");

            _text = text;
            _values = values;
        }

        protected override Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync(SelectListContext context)
        {
            var results = new List<SelectListItem>();

            for (int i = 0; i < _text.Length; i++)
            {
                results.Add(new SelectListItem()
                {
                    Text = _text[i].ToString(),
                    Value = _values[i].ToString()
                });
            }

            return Task.FromResult(results.AsEnumerable());
        }     
    }
}
