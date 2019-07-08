using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Mvc.SelectList
{
    public class SelectListYesNoAttribute : SelectListOptionsAttribute
    {
        public SelectListYesNoAttribute()
        : base(new object[] { "Yes", "No" }, new object[] { "true", "false" })
        {
        }
    }
}
