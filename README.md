# ASP.NET Core MVC Select List Attributes

[![nuget](https://img.shields.io/nuget/v/AspNetCore.Mvc.SelectList.svg)](https://www.nuget.org/packages/AspNetCore.Mvc.SelectList/)  ![Downloads](https://img.shields.io/nuget/dt/AspNetCore.Mvc.SelectList.svg "Downloads")

ASP.NET Core library which gives the ability to specify select lists via Model Attributes at both Type and Property levels. The select lists can be used to populate dropdowns but also as an IEnumerable collection to loop through in views.

## Installation

### NuGet
```
PM> Install-Package AspNetCore.Mvc.SelectList
```

### .Net CLI
```
> dotnet add package AspNetCore.Mvc.SelectList
```

## Usage

```
services.AddSelectListAttributes();
```

```
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
	[SelectListDbWhere(nameof(Subscription.Description), "Standard")]
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
```

```
@model Customer

<form asp-controller="Customer" asp-action="Update" method="post">
	<input asp-for="Id" />
	<label asp-for="Name"></label>
	<input asp-for="Name">
	<br />
	<label asp-for="Age"></label>
	<select asp-for="Age" asp-items="@(Html.SelectListFor(model => model.Age, "List2"))"></select>
	<br />
	<label asp-for="SubscriptionId"></label>
	<select asp-for="SubscriptionId"></select>
	<br />
	<label asp-for="SubscriptionId2"></label>
	<select asp-for="SubscriptionId2"></select>
	<br />
	<label asp-for="SubscriptionId3"></label>
	<select asp-for="SubscriptionId3"></select>
	<br />
	<label asp-for="File"></label>
	<select asp-for="File"></select>
	<br />
	<label asp-for="Folder"></label>
	<select asp-for="Folder"></select>
	<br />
	<label asp-for="File2"></label>
	<select asp-for="File2"></select>
	<br />
	<label asp-for="StatusId"></label>
    <select asp-for="StatusId"></select>
    <br />
	<button type="submit">Add/Update</button>
</form>
```

```
@foreach (var item in await Html.SelectListForModelTypeAsync<Customer>())
{
	@item.Html.DisplayFor(c => c.Name);
	<br />
}
```

```
{
	var item = await Html.SelectListForModelTypeAsync<Customer>(new object[]{"ecf1f87a-ce11-471d-abae-735d23c91256"}).FirstOrDefault();
	@item.Html.DisplayFor(c => c.Name);
}
```

```
[HttpGet]
public IActionResult Edit()
{
	//Override attribute select list
	ViewBag.File = new List<SelectListItem>() { };

    return View("Edit", _db.Customers.Find(_customerId));
}
```

## Attributes

| Attribute                  | Description                                                                        |
|:---------------------------|:-----------------------------------------------------------------------------------|
| SelectListAttribute        | Base class                                                                         |
| SelectListOptionsAttribute | Specify Options                                                                    |
| SelectListDbAttribute      | Specify DbContext and ModelType                                                    |
| SelectListDbWhereAttribute | Specify DbContext Where Clause                                                     |
| SelectListFileAttribute    | Specify physical, Content Root virtual or Web Root virtual path such as "~/files/" |
| SelectListFolderAttribute  | Specify physical, Content Root virtual or Web Root virtual path such as "~/files/" |

## Authors

* **Dave Ikin** - [davidikin45](https://github.com/davidikin45)


## License

This project is licensed under the MIT License