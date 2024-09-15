using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using AutoBiz.Adapters.HttpApi.Host;
using AutoBiz.Adapters.HttpApi.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public class RouteBuilder<TTenant> : IRouteBuilder<TTenant>
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
          TTenant tenant = default; // Get the tenant from authentication services.
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TResult result = await handler(request, contextArguments, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
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
          TTenant tenant = default; // Get the tenant from authentication services.
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, contextArguments, tenant, dependencies);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, contextArguments, tenant, dependencies);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
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

    public IRouteBuilder<TTenant> AddCommand<TRequest, TContextArguments, TDependencies>(
      Func<TRequest, TContextArguments, TDependencies, Task> handler)
    {
      string url = GetUrl();
      
      app.MapPost(url, async (HttpContext context, [FromBody] TRequest request, IServiceProvider serviceProvider) =>
      {
        TContextArguments contextArguments = RequestProcessing.ParseRouteArguments<TContextArguments>(context.Request);
        TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(serviceProvider);
        await handler(request, contextArguments, dependencies);
      });
      
      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task<TResult>> handler)
    {
      RestrictMethods(method, [HttpMethod.Get, HttpMethod.Post]);
      
      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TResult result = await handler(request, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TResult result = await handler(request, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddCommand<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task> handler)
    {      
      string url = GetUrl();

      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, tenant, dependencies);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TTenant tenant = default; // Get the tenant from authentication services.
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, tenant, dependencies);
        });
      }

      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task<TResult>> handler)
    {
      RestrictMethods(method, [HttpMethod.Get, HttpMethod.Post]);
      
      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TResult result = await handler(request, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Post)
      {
        app.MapPost(url, async (HttpContext context, [AsParameters] TRequest request) =>
        {
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          TResult result = await handler(request, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
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