using Microsoft.AspNetCore.Mvc.Formatters;
using Mdk.Shared;
using Northwind.WebApi.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Northwind.WebApi;
using System.Net;

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

builder.Services.AddHttpLogging(option =>{
    option.LoggingFields = HttpLoggingFields.All;
    option.RequestBodyLogLimit = 4096;
    option.ResponseBodyLogLimit = 4096;
});

// builder.Services.AddW3CLogging(options =>
// {
//     options.AdditionalRequestHeaders.Add("x-forwarded-for");
//     options.AdditionalRequestHeaders.Add("x-client-ssl-protocol");
// });

builder.Services.AddHealthChecks()
.AddDbContextCheck<NorthwindContext>();

builder.WebHost.ConfigureKestrel((context, option)=>{
    option.ListenAnyIP(5002,listenOptions =>{
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>{
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[] {
            SubmitMethod.Get,
            SubmitMethod.Post,
            SubmitMethod.Put,
            SubmitMethod.Delete
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHttpLogging();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.MapControllers();

app.UseHealthChecks(path: "/howyoudoin");
app.Run();
