using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// app.UseAuthorization();

app.MapGet("/", async httpContext => {
  byte[] data = System.Text.Encoding.UTF8.GetBytes("Hello World!");
  await httpContext.Response.Body.WriteAsync(data);
});

app.Run();
