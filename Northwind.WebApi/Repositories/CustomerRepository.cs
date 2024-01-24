using System.Collections.Concurrent;
using Mdk.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    //use a static thread-safe dictionary field to cache the customers
    //this should be replaced with a distributed cache system like Redis 
    private static ConcurrentDictionary<string, Customer>? _customerCache;
    private NorthwindContext _db;
    public CustomerRepository(NorthwindContext injectedContext)
    {
        _db = injectedContext;

        if(_customerCache is null){
            _customerCache = new ConcurrentDictionary<string, Customer>(_db.Customers.ToDictionary(c => c.CustomerId));
        }
    }

    public async Task<Customer?> CreateAsync(Customer newCustomer)
    {
        newCustomer.CustomerId = newCustomer.CustomerId.ToUpper();

        EntityEntry<Customer> added = await _db.Customers.AddAsync(newCustomer);
        int affected = await _db.SaveChangesAsync();

        if(affected == 1){
            if(_customerCache is null) return newCustomer;
            return _customerCache.AddOrUpdate(newCustomer.CustomerId, newCustomer, UpdateCache);
        }else{
            return null;
        }
    }

    public Task<Customer?> RetrieveAsync(string id)
    {
        id = id.ToUpper();
        if(_customerCache is null) return null!;

        _customerCache.TryGetValue(id, out Customer? customer);
        return Task.FromResult(customer);
    }

    public Task<IEnumerable<Customer>> RetrieveAllAsync()
    {
        return Task.FromResult(_customerCache is null ? Enumerable.Empty<Customer>() : _customerCache.Values);
    }

    public async Task<Customer?> UpdateAsync(string id, Customer updatedCustomer)
    {
       id = id.ToUpper();
       updatedCustomer.CustomerId = updatedCustomer.CustomerId.ToLower();

       _db.Customers.Update(updatedCustomer);
       int affected = await _db.SaveChangesAsync();

       if(affected == 1){
            return UpdateCache(id, updatedCustomer);
       }else{
        return null;
       }
    }

    public async Task<bool?> DeleteAsync(string id)
    {
       id = id.ToUpper();
       Customer? customer = _db.Customers.Find(id);

       if(customer is null) return null!;

        _db.Customers.Remove(customer);
        int affected = await _db.SaveChangesAsync();

        if(affected == 0){
            return false;
        }

        if(_customerCache is null) return null;
        return _customerCache.TryRemove(id, out _);
    }

    private Customer UpdateCache(string id, Customer updatedCustomer){
        Customer? oldCustomer;

        if(_customerCache is null) return null!;

        if(_customerCache.TryGetValue(id, out oldCustomer)){
            if(_customerCache.TryUpdate(id, updatedCustomer, oldCustomer)){
                return updatedCustomer;
            }
        }

        return null!;
    }
}
