using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Mdk.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly NorthwindContext _dbContext;
    public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext)
    {
        _logger = logger;
        _dbContext = injectedContext;
    }

    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public IActionResult Index()
    {
        _logger.LogError("This is an error log.");
        _logger.LogWarning("This is a warrning!");
        _logger.LogInformation("This is a information log.");

        HomeIndexViewModel model = new HomeIndexViewModel(
            Random.Shared.Next(1,1001),
             _dbContext.Categories.ToList(),
             _dbContext.Products.ToList());

        return View(model);
    }

    [Route("private")]
    [Authorize(Roles = "Administrators")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ProductDetail(int? id){

        if(!id.HasValue){
            return BadRequest("You must pass a product ID in the route, for example, /Home/ProductDetail/21");
        }

        Product? model = _dbContext.Products.SingleOrDefault(p => p.ProductId == id);

        if(model is null){
            return NotFound($"Product {id} not found");
        }

        return View(model);
    }

    public IActionResult ModelBinding(){
        return View(); // a page with a form to submit
    }

    [HttpPost]
    public IActionResult ModelBinding(Thing thing){
        HomeModelBindingViewModel model = new HomeModelBindingViewModel(
            Thing: thing, 
            HasErrors: !ModelState.IsValid,
            ValidationErrors: ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage));

            return View(model);
    }
}
