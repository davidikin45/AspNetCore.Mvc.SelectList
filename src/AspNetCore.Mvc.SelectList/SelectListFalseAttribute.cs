using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListFalseAttribute : SelectListOptionsAttribute
    {
        public SelectListFalseAttribute()
        : base(new object[] { "False" }, new object[] { "true" })
        {
        }
    }
}
