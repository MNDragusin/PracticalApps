using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Mdk.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly NorthwindContext _dbContext;
    public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _dbContext = injectedContext;
        _clientFactory = httpClientFactory;
    }

    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Index()
    {
        _logger.LogError("This is an error log.");
        _logger.LogWarning("This is a warrning!");
        _logger.LogInformation("This is a information log.");

        HomeIndexViewModel model = new HomeIndexViewModel(
            Random.Shared.Next(1,1001),
            await _dbContext.Categories.ToListAsync(),
             await _dbContext.Products.ToListAsync());

        try
        {
            HttpClient client = _clientFactory.CreateClient(name: "Minimal.WebApi");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: "weatherforecast");
            HttpResponseMessage response = await client.SendAsync(request);

            ViewData["weather"] = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        }
        catch (System.Exception exception)
        {
            _logger.LogWarning($"Minimal.WebApi service is not responding. Exception: {exception.Message}");
            ViewData["weather"] = Enumerable.Empty<WeatherForecast>().ToArray();
        }

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

    public async  Task<IActionResult> ProductDetail(int? id, string alertStyle = "success"){

        ViewData["alertStyle"] = alertStyle;
        if(!id.HasValue){
            return BadRequest("You must pass a product ID in the route, for example, /Home/ProductDetail/21");
        }

        Product? model = await _dbContext.Products.SingleOrDefaultAsync(p => p.ProductId == id);

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

    public IActionResult ProductsThatCostMoreThan(decimal? price){
        if(!price.HasValue){
            return BadRequest("You must pass a product price in the query string, for example, /Home/ProductsThatCostMoreThan?price=50");
        }

        IEnumerable<Product> model = _dbContext.Products
        .Include(p=>p.Category)
        .Include(p=>p.Supplier)
        .Where(p=>p.UnitPrice > price);

        if(!model.Any())
        {
            return NotFound($"No products cost more than {price:C}");
        }

        ViewData["MaxPrice"] = price.Value.ToString("C");
        return View(model);
    }

    public async Task<IActionResult> Customers(string country){
        string uri = "api/Customers";

        if(string.IsNullOrEmpty(country)){
            ViewData["Title"] = "All Customers Worldwide";
        }else{
            ViewData["Title"] = $"Customers in {country}";
            uri += $"/?country={country}";
        }

        try
        {
            HttpClient client = _clientFactory.CreateClient(name: "Northwind.WebApi");
            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);
            IEnumerable<Customer>? model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();
            return View(model);
        }
        catch (Exception exception)
        {           
            _logger.LogWarning($"Northwind.WebApi service is not responding. Exception: {exception.Message}");
            return View("Error", new ErrorViewModel());
        } 
    }
}
