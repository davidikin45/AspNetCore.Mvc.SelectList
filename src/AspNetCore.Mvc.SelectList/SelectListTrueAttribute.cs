using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListTrueAttribute : SelectListOptionsAttribute
    {
        public SelectListTrueAttribute()
        : base(new object[] { "True" }, new object[] { "true" })
        {
        }
    }
}
