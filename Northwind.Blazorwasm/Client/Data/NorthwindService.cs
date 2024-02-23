using System.Net;
using System.Net.Http.Json;
using Mdk.Shared;

namespace Northwind.Blazorwasm.Data;

public class NorthwindService : INorthwindService
{
    private readonly HttpClient _http;

    public NorthwindService(HttpClient http)
    {
        _http = http;
    }
    
    public Task<List<Customer>> GetCustomersAsync()
    {
        return _http.GetFromJsonAsync<List<Customer>>("api/customers")!;
    }

    public Task<List<Customer>> GetCustomersAsync(string country)
    {
        return _http.GetFromJsonAsync<List<Customer>>($"api/Customers/in/{country}")!;
    }

    public Task<Customer?> GetCustomerAsync(string id)
    {
        return _http.GetFromJsonAsync<Customer>($"api/Customers/{id}")!;
    }

    public async Task<Customer> CreateCustomerAsync(Customer newCustomer)
    {
        HttpResponseMessage response = await _http.PostAsJsonAsync("api/customers", newCustomer);
        return (await response.Content.ReadFromJsonAsync<Customer>())!;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        HttpResponseMessage response = await _http.PutAsJsonAsync("api/customers", customer);
        return (await response.Content.ReadFromJsonAsync<Customer>())!;
    }

    public async Task DeleteCustomerAsync(string id)
    {
        HttpResponseMessage responseMessage = await _http.DeleteAsync($"api/Customers/{id}");
    }
}