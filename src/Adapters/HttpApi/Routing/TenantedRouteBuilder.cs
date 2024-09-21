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
    public class RouteBuilder<TTenant> : IRouteBuilder<TTenant>
  {
    private WebApplication app;
    private IEnumerable<string> urlSegments;
    private AuthenticatedTenentResolver<TTenant> tenantResolver;

    public RouteBuilder(WebApplication app, IEnumerable<string> urlSegments, AuthenticatedTenentResolver<TTenant> tenantResolver)
    {
      this.app = app;
      this.urlSegments = urlSegments;
      this.tenantResolver = tenantResolver;
    }

    private string GetUrl()
    {
      return "/" + string.Join("/", urlSegments.Select(s => s.Trim('/')).ToList());
    }
    
    public IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TTenant, TDependencies, Task<TResult>> handler)
    {
      RestrictMethods(method, [HttpMethod.Get, HttpMethod.Post]);
      
      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          Task<TTenant> tenantTask = this.tenantResolver.Invoke(context);
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TTenant tenant = await tenantTask;

          TResult result = await handler(request, contextArguments, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          Task<TTenant> tenantTask = this.tenantResolver.Invoke(context);
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TTenant tenant = await tenantTask;

          TResult result = await handler(request, contextArguments, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddCommand<TRequest, TContextArguments, TDependencies>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TTenant, TDependencies, Task> handler)
    {      
      string url = GetUrl();
      
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          Task<TTenant> tenantTask = this.tenantResolver.Invoke(context);
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TTenant tenant = await tenantTask;

          await handler(request, contextArguments, tenant, dependencies);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          Task<TTenant> tenantTask = this.tenantResolver.Invoke(context);
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TTenant tenant = await tenantTask;
          
          await handler(request, contextArguments, tenant, dependencies);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TDependencies, Task<TResult>> handler)
    {
      return AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
        HttpMethod.Post,
        (request, contextArguments, _, dependencies) => handler(request, contextArguments, dependencies));
    }

    public IRouteBuilder<TTenant> AddCommand<TRequest, TContextArguments, TDependencies>(
      Func<TRequest, TContextArguments, TDependencies, Task> handler)
    {
      return AddCommand<TRequest, TContextArguments, TDependencies>(
        HttpMethod.Post,
        (request, contextArguments, _, dependencies) => handler(request, contextArguments, dependencies));
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task<TResult>> handler)
    {
      return AddHandler<TRequest, object, TDependencies, TResult>(
        method,
        (request, _, tenant, dependencies) => handler(request, tenant, dependencies));
    }

    public IRouteBuilder<TTenant> AddCommand<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task> handler)
    {
      return AddCommand<TRequest, object, TDependencies>(
        method,
        (request, _, tenant, dependencies) => handler(request, tenant, dependencies));
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task<TResult>> handler)
    {
      return AddHandler<TRequest, TDependencies, object, TResult>(
        method,
        (request, dependencies, _) => handler(request, dependencies));
    }

    public IRouteBuilder<TTenant> AddCommand<TRequest, TDependencies>(
      Func<TRequest, TDependencies, Task> handler)
    {
      string url = GetUrl();
      
      app.MapPost(url, async (HttpContext context, [FromBody] TRequest request) =>
      {
        TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
        await handler(request, dependencies);
      });

      return this;
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