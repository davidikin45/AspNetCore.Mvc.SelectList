using AspNetCore.Mvc.SelectList;
using EntityFrameworkCore.Initialization.Converters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Data
{
    [SelectListDb(typeof(AppDbContext), typeof(Customer), OrderByProperty = nameof(Customer.Id))]
    public class Customer
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
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
        [SelectListFile("files")]
        public string File { get; set; }

        [Display(Name = "Folder")]
        [SelectListFolder("files")]
        public string Folder { get; set; }

        [Display(Name = "File2")]
        [SelectListFile("wwwroot")]
        public string File2 { get; set; }

        [Display(Name = "Status")]
        [SelectListDb(typeof(AppDbContext), typeof(Status), "{" + nameof(Status.Description) + "} - {" + nameof(Status.Id) + "}", OrderByType = "asc")]
        public int StatusId { get; set; }

        [Display(Name = "Mail Preference")]
        [SelectListEnum]
        public MailPreference MailPreference { get; set; }

        [Display(Name = "Country")]
        [SelectListCountry]
        public string CountryCode { get; set; }

        [CsvDb]
        [LimitOptionsMinMax(1,2)]
        [Display(Name = "Checkbox List")]
        [SelectListEnum]
        public List<MailPreference> CheckboxValues { get; set; } = new List<MailPreference>();

        [CsvDb]
        [LimitOptionsMin(1)]
        [Display(Name = "Checkbox Button List")]
        [SelectListCountry]
        public List<string> CheckboxButtonValues { get; set; } = new List<string>();

        [Required]
        [Display(Name = "Radio List")]
        [SelectListEnum]
        public MailPreference RadioValue { get; set; }

        [Required]
        [Display(Name = "Radio Button List")]
        [SelectListEnum]
        public MailPreference RadioButtonValue { get; set; }

        [Display(Name = "Yes/No Radio Button List with no default")]
        [SelectListYesNo]
        public bool? YesNo { get; set; }

        [Display(Name = "True/False Radio Button List with no default")]
        [SelectListTrueFalse]
        public bool? TrueFalse { get; set; }

        [Display(Name = "Yes/No Radio Button List with default")]
        [SelectListYesNo]
        public bool YesNoDefault { get; set; }

        [Display(Name = "True/False Radio Button List with default")]
        [SelectListTrueFalse]
        public bool TrueFalseDefault { get; set; }

        [Display(Name = "Yes Only")]
        [SelectListYes]
        public bool Yes { get; set; }

        [Display(Name = "No Only")]
        [SelectListNo]
        public bool No { get; set; }

        [Display(Name = "True Only")]
        [SelectListTrue]
        public bool True { get; set; }

        [Display(Name = "False Only")]
        [SelectListFalse]
        public bool False { get; set; }
    }
}
