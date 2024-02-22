using Mdk.Shared;
using Microsoft.EntityFrameworkCore;

namespace Northwind.BlazorServer.Data;

public class NorthwindService : INorthwindService
{
    private readonly NorthwindContext _db;

    public NorthwindService(NorthwindContext db)
    {
        _db = db;
    }

    public Task<List<Customer>> GetCustomersAsync()
    {
        return _db.Customers.ToListAsync();
    }

    public Task<List<Customer>> GetCustomersAsync(string country)
    {
        return _db.Customers.Where(c => c.Country == country).ToListAsync();
    }

    public Task<Customer?> GetCustomerAsync(string id)
    {
        return _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public Task<Customer> CreateCustomerAsync(Customer newCustomer)
    {
        _db.Customers.Add(newCustomer);
        _db.SaveChangesAsync();
        return Task.FromResult(newCustomer);
    }

    public Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        _db.Entry(customer).State = EntityState.Modified;
        _db.SaveChangesAsync();
        return Task.FromResult(customer);
    }

    public Task DeleteCustomerAsync(string id)
    {
        Customer? customer = _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id).Result;

        if (customer == null)
        {
            return Task.CompletedTask;
        }

        _db.Customers.Remove(customer);
        return _db.SaveChangesAsync();
    }
}