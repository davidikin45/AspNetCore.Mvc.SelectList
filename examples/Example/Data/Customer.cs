using AspNetCore.Mvc.SelectList;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Example.Data
{
    [SelectListDb(typeof(AppDbContext), typeof(Customer), OrderByProperty = nameof(Customer.Id))]
    public class Customer
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Age")]
        [SelectListOptions("18-30","31-50","50+")]
        [SelectListOptions("35-70", "70-80", "80+", SelectListId = "List2")]
        public string Age { get; set; }

        [Display(Name = "Susbcription")]
        [SelectListDb(typeof(AppDbContext), typeof(Subscription), "{" + nameof(Subscription.Description) + "} - {" + nameof(Subscription.Cost) + "}", OrderByProperty = nameof(Subscription.Order), OrderByType = "asc" )]
        public string SubscriptionId { get; set; }

        [Display(Name = "Susbcription 2")]
        [SelectListDb(typeof(AppDbContext), typeof(Subscription), "SELECT * FROM Subscriptions WHERE Description = {0}", new object[] { "Standard" }, "{" + nameof(Subscription.Description) + "} - {" + nameof(Subscription.Cost) + "}")]
        public string SubscriptionId2 { get; set; }

        [Display(Name = "Susbcription")]
        [SelectListDb(typeof(AppDbContext), typeof(Subscription), "{" + nameof(Subscription.Description) + "} - {" + nameof(Subscription.Cost) + "}", OrderByProperty = nameof(Subscription.Order), OrderByType = "asc")]
        [SelectListDbWhereEquals(nameof(Subscription.Description), "Standard")]
        public string SubscriptionId3 { get; set; }

        [Display(Name = "File")]
        [SelectListFile("~/files/")]
        public string File { get; set; }

        [Display(Name = "Folder")]
        [SelectListFolder("~/files/")]
        public string Folder { get; set; }

        [Display(Name = "File2")]
        [SelectListFile("wwwroot")]
        public string File2 { get; set; }

        [Display(Name = "Status")]
        [SelectListDb(typeof(AppDbContext), typeof(Status), "{" + nameof(Status.Description) + "} - {" + nameof(Status.Id) + "}", OrderByType = "asc")]
        public int StatusId { get; set; }
    }
}
