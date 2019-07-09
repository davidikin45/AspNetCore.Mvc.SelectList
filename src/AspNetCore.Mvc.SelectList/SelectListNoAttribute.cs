using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListNoAttribute : SelectListOptionsAttribute
    {
        public SelectListNoAttribute()
        : base(new object[] { "No" }, new object[] { "true" })
        {
        }
    }
}
