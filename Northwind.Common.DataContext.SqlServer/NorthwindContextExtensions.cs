using Mdk.Shared.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.Common.DataContext.SqlServer;

public static class NorthwindContextExtensions
{
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services, string connectionString = "Data Source=.\\csdotnetbook;Initial Catalog=Northwind;Integrated Security=true;TrustServerCertificate=True"){

        services.AddDbContext<NorthwindContext>(options =>{
            options.UseSqlServer(connectionString);

            options.LogTo(Console.WriteLine,new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting } );
        });

        return services;
    }
}
