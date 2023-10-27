using Mdk.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MdkFeatures.Pages;

public class EmployeesModel : PageModel
{
    private NorthwindContext _dbContext;
    public Employee[] Employees { get; set; } = null!;

    public EmployeesModel(NorthwindContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Employees";
        Employees = _dbContext.Employees.OrderBy(e=>e.LastName).ThenBy(e=>e.FirstName).ToArray();
    }
}
