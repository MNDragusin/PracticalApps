using Mdk.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Blazorwasm.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly NorthwindContext _db;

    public CustomersController(NorthwindContext dbContext)
    {
        _db = dbContext;
    }

    [HttpGet]
    public async Task<List<Customer>> GetCustomersAsync()
    {
        return await _db.Customers.ToListAsync();
    }

    [HttpGet("in/{country}")]
    public async Task<List<Customer>> GetCustomersAsync(string country)
    {
        return await _db.Customers.Where(c => c.Country == country).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<Customer?> GetCustomerAsync(string id)
    {
        return await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    [HttpPost]
    public async Task<Customer?> CreateCustomerAsync(Customer newCustomer)
    {
        Customer? existingCustomer = await GetCustomerAsync(newCustomer.CustomerId);

        if (existingCustomer != null)
        {
            return existingCustomer;
        }

        _db.Customers.Add(newCustomer);
        await _db.SaveChangesAsync();
        return newCustomer;
    }

    [HttpPut]
    public async Task<Customer?> UpdateCustomerAsync(Customer customer)
    {
        _db.Entry(customer).State = EntityState.Modified;
        if (1 != await _db.SaveChangesAsync())
        {
            return null;
        }

        return customer;
    }

    public async Task<int> DeleteCustomerAsync(string id)
    {
        Customer? customer = await GetCustomerAsync(id);

        if (customer == null)
        {
            return 0;
        }

        _db.Customers.Remove(customer);
        return await _db.SaveChangesAsync();
    }
}