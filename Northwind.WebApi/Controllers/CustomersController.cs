using Microsoft.AspNetCore.Mvc;
using Mdk.Shared;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    public CustomersController(ICustomerRepository customersRepository)
    {
        _repo = customersRepository;
    }

    // GET: api/customers
    // GET: api/customers/?country=[country]
    // this will always return a list of customers (but it might be empty)
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
    [ProducesResponseType(404)] //NotFound
    //public async Task<IEnumerable<Customer>> GetCustomers(string? country){
    public async Task<IActionResult> GetCustomers(string? country){

        if(string.IsNullOrWhiteSpace(country)){
            return Ok(await _repo.RetrieveAllAsync());
        }

        var result = (await _repo.RetrieveAllAsync()).Where(c => c.Country == country);

        if(result.Count() == 0){
            return NotFound("Try spell it with an upper case for the first letter or all uppercase in case of abreviation");
        }

        return Ok(result);
    }

    //GET: api/customer/id
    [HttpGet("{id}", Name = nameof(GetCustomer))]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id){
        var customer = await _repo.RetrieveAsync(id);

        if(customer is null){
            return NotFound();
        }

        return Ok(customer);
    }

    // POST: api/customers
    // BODY: Customer (JSON, XML)
    [HttpPost]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Customer newCustomer){
        if(newCustomer is null){
            return BadRequest(); //400 Bad request
        }

        Customer? addedCustomer = await _repo.CreateAsync(newCustomer);
        if(addedCustomer is null){
            return BadRequest("Repository failed to add new customer");
        }

        return CreatedAtRoute( //201 Created
            routeName: nameof(GetCustomer),
            routeValues: new {id = addedCustomer.CustomerId.ToLower()},
            value: addedCustomer
         );
    }

    // PUT: api/customers/[id]
    // BODY: Customer (JSON, XML)
    [HttpPut("{id}")]
    [ProducesResponseType(204)] //NoContentResult
    [ProducesResponseType(400)] //BadRequest
    [ProducesResponseType(404)] //NotFound
    public async Task<IActionResult> Update(string id, [FromBody] Customer customer){
        id = id.ToUpper();

        if(string.IsNullOrWhiteSpace(id) || customer is null){
            return BadRequest();
        }
        
        Customer? existing = await _repo.RetrieveAsync(id);
        if(existing is null){
            return BadRequest();
        }

        customer.CustomerId = customer.CustomerId.ToUpper();
        await _repo.UpdateAsync(id, customer);
        return new NoContentResult();
    }

    // DELETE: api/customers/[id]
    [HttpDelete("{id}")]
    [ProducesResponseType(204)] //NoContentResult
    [ProducesResponseType(400)] //BadRequest
    [ProducesResponseType(404)] //NotFound
    public async Task<IActionResult> Delete(string id){
        //take controller of problem details
        if(id == "bad"){
            ProblemDetails problemDetails = new(){
                Status = StatusCodes.Status400BadRequest,
                Type = "https://localhost:5001/customers/failed-to-delete",
                Title = $"Customer ID {id} found but failed to delete.",
                Detail = "More details like Company Name, Country and so on.",
                Instance = HttpContext.Request.Path
            };

            return BadRequest(problemDetails);
        }

        if(string.IsNullOrWhiteSpace(id)){
            return BadRequest();
        }

        Customer? existing = await _repo.RetrieveAsync(id);
        if(existing is null){
            return NotFound();
        }

        bool? deleted = await _repo.DeleteAsync(id);

        if(deleted.HasValue && !deleted.Value){
            return BadRequest($"Customer {id} was found but failed to delete");
        }

        return new NoContentResult();
    }
}
