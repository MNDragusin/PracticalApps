using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mdk.Shared.Sqlite;

public static class NorthwindContextExtensions{
    /// <summary>
    /// Adds NorthwindContext to the specified IServiceCollection. Uses the Sqlite database provider.
    /// </summary>
    /// <param name="servrices"></param>
    /// <param name="relativePath">Set to overide the default of ".."</param>
    /// <returns> An IServiceCollection that can be used to add more services.</returns>
    public static IServiceCollection AddNorthwindContext(this IServiceCollection servrices, string relativePath = ".."){
        string dbPath = Path.Combine(relativePath, "Northwind.db");

        servrices.AddDbContext<NorthwindContext>(options => 
        {
            options.UseSqlite($"Data Source={dbPath}");
            
            options.LogTo(Console.WriteLine, new[]{
                Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting
            });
        });

        return servrices;
    }
}