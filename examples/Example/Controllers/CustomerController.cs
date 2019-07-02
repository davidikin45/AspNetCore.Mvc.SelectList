using Example.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Example.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _db;
        private static Guid _customerId = Guid.Parse("ecf1f87a-ce11-471d-abae-735d23c91256");

        public CustomerController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            //Override attribute select list
            //ViewBag.File = new List<SelectListItem>() { };

            return View("Edit", _db.Customers.Find(_customerId));
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Customer customer)
        {
            if(ModelState.IsValid)
            {
                //Add or Update
                _db.Customers.Update(customer);
                await _db.SaveChangesAsync();
                RedirectToAction("Index", "Home");
            }

            return View("Edit", customer);
        }
    }
}
