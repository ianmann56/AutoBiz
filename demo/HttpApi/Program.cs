using System.Net.Http;
using System.Threading.Tasks;
using AutoBiz.Adapters.HttpApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Todos.Business;
using Todos.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITodoRepository, TodoRepository>();

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

app.UseAutoBiz<ExampleTenant>(api =>
{
  api.AddGroup("todos", group =>
  {
    group.AddRoute("", route =>
    {
      route.AddCommand((CreateTodoCommandArguments req, CreateTodoCommandDeps deps) => CreateTodoCommand.Execute(req, deps));
    });
    group.AddRoute("", route =>
    {
      route.AddHandler(HttpMethod.Get, (ListTodosQueryArguments req, ListTodosQueryDeps deps) => ListTodosQuery.Query(req, deps));
    });
    group.AddRoute("{id}", route =>
    {
      route.AddCommand((object request, DeleteTodoContext context, DeleteTodoDeps deps) => DeleteTodoCommand.Execute(context, deps));
    });
  });
});

app.Run();
