using Microsoft.AspNetCore.Mvc.Formatters;
using Mdk.Shared;
using Northwind.WebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddNorthwindContext();
builder.Services.AddControllers(options =>
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters){
        OutputFormatter? mediaFormater = formatter as OutputFormatter;
        if(mediaFormater is null){
            WriteLine($" {formatter.GetType().Name}");
        }
        else{
            WriteLine($" {mediaFormater.GetType().Name} Media Type: {string.Join(", ", mediaFormater.SupportedMediaTypes)}");
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
