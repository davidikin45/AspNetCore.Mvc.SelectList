using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListYesAttribute : SelectListOptionsAttribute
    {
        public SelectListYesAttribute()
        : base(new object[] { "Yes" }, new object[] { "true" })
        {
        }
    }
}
