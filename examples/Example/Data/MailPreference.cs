using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Example.Data
{
    public enum MailPreference
    {
        [Display(Name = "Email")]
        Email,
        [Display(Name = "Mail")]
        Mail,
        [Display(Name = "None")]
        None
    }
}
