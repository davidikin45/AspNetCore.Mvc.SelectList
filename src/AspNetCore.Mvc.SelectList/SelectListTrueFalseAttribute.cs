using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListTrueFalseAttribute : SelectListOptionsAttribute
    {
        public SelectListTrueFalseAttribute()
        : base(new object[] { "True", "False" }, new object[] { "true", "false" })
        {
        }
    }
}
