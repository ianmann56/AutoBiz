using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoBiz.Adapters.HttpApi.Host;
using AutoBiz.Adapters.HttpApi.Http;
using AutoBiz.Adapters.HttpApi.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public class RouteBuilder : IRouteBuilder
  {
    private WebApplication app;
    private IEnumerable<string> urlSegments;

    public RouteBuilder(WebApplication app, IEnumerable<string> urlSegments)
    {
      this.app = app;
      this.urlSegments = urlSegments;
    }

    private string GetUrl()
    {
      return "/" + string.Join("/", urlSegments.Select(s => s.Trim('/')).ToList());
    }

    public IRouteBuilder AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TDependencies, Task<TResult>> handler)
    {
      RestrictMethods(method, [HttpMethod.Get, HttpMethod.Post]);

      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);

          TResult result = await handler(request, contextArguments, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);

          TResult result = await handler(request, contextArguments, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
    }

    public IRouteBuilder AddCommand<TRequest, TContextArguments, TDependencies>(Func<TRequest, TContextArguments, TDependencies, Task> handler)
    {
      string url = GetUrl();

      app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
      {
        TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
        TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);

        await handler(request, contextArguments, dependencies);
      });

      return this;
    }

    public IRouteBuilder AddHandler<TRequest, TDependencies, TResult>(HttpMethod method, Func<TRequest, TDependencies, Task<TResult>> handler)
    {
      return AddHandler<TRequest, object, TDependencies, TResult>(
        method,
        (request, _, dependencies) => handler(request, dependencies));
    }

    public IRouteBuilder AddCommand<TRequest, TDependencies>(HttpMethod method, Func<TRequest, TDependencies, Task> handler)
    {
      return AddCommand<TRequest, object, TDependencies>(
        (request, _, dependencies) => handler(request, dependencies));
    }

    public IRouteBuilder AddHandler<TRequest, TResult>(HttpMethod method, Func<TRequest, Task<TResult>> handler)
    {
      throw new NotImplementedException();
    }

    public IRouteBuilder AddCommand<TRequest>(HttpMethod method, Func<TRequest, Task> handler)
    {
      throw new NotImplementedException();
    }

    public IRouteBuilder<TTenant> AddTenant<TTenant>(AuthenticatedTenentResolver<TTenant> tenantResolver)
    {
      throw new NotImplementedException();
    }

    private static void RestrictMethods(HttpMethod method, IEnumerable<HttpMethod> allowedMethods)
    {
      if (!allowedMethods.Contains(method))
      {
        throw new NotImplementedException($"Expected one of the following methods: [{string.Join(", ", allowedMethods)}]. Got: {method}");
      }
    }
  }
}