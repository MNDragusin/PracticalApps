using Mdk.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddNorthwindContext();

var app = builder.Build();


if(!app.Environment.IsDevelopment())
{
    app.UseHsts();
}else{
    app.UseDeveloperExceptionPage();
}

app.Use(async (HttpContext context, Func<Task> next) =>{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if(rep is not null){
        Console.WriteLine($"Endpoint name: {rep.DisplayName}");
        Console.WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
    }

    if(context.Request.Path == "/bonjour"){
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }

    await next();
});

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapRazorPages();
app.MapGet("/hello", () => "Hello World!");

app.Run();
Console.WriteLine("This executes afer the web server has stopped!");