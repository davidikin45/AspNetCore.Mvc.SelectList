using AspNetCore.Mvc.SelectList;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Example.Data
{
    public class Customer
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Age")]
        [SelectListOptions("18-30","31-50","50+")]
        public string Age { get; set; }

        [Display(Name = "Susbcription")]
        [SelectListDb(typeof(AppDbContext), typeof(Subscription), "{" + nameof(Subscription.Description) + "} - {" + nameof(Subscription.Cost) + "}", OrderByProperty = nameof(Subscription.Order), OrderByType = "asc" )]
        public string SubscriptionId { get; set; }

        [Display(Name = "File")]
        [SelectListFile("~/files/")]
        public string File { get; set; }

        [Display(Name = "Folder")]
        [SelectListFolder("~/files/")]
        public string Folder { get; set; }
    }
}
