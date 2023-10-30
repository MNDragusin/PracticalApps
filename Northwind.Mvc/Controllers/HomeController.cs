using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Mdk.Shared;

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
