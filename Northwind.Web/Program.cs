var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

if(!app.Environment.IsDevelopment())
{
    app.UseHsts();
}else{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/hello", () => "Hello World!");

app.Run();
Console.WriteLine("This executes afer the web server has stopped!");