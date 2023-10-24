using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mdk.Shared;

namespace MyApp.Namespace
{
    public class SuppliersModel : PageModel
    {
        [BindProperty]
        public Supplier? Supplier {get; set;}

        public IEnumerable<Supplier>? Suppliers {get; set;}
                
        private NorthwindContext _dbContext;
        
        public SuppliersModel(NorthwindContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        //called on page load
        public void OnGet()
        {
            ViewData["Title"] = "Northwind B2B - Suppliers";

            Suppliers = _dbContext.Suppliers.OrderBy(c=>c.CompanyName).ThenBy(c=>c.CompanyName);
        }

        //post listenr for adding new suppliers
        public IActionResult OnPost()
        {
            if((Supplier is not null) && ModelState.IsValid){
                _dbContext.Suppliers.Add(Supplier);
                _dbContext.SaveChanges();

                return RedirectToPage("/suppliers");
            }

            return Page();
        }
    }
}
