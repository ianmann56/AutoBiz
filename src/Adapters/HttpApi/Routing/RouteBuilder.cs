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
    
    public IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TTenant, TDependencies, Task<object>> handler)
    {
      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async context =>
        {
          TRequest request = RequestProcessing.ParseQueryString<TRequest>(context.Request);
          TTenant tenant = default; // Get the tenant from authentication services.
          TContextArguments contextArguments = default; // Get the args from the URL
          TDependencies dependencies = default; // Use reflection to get the dependencies from the DI container for each property in TDependencies.
          object result = await handler(request, contextArguments, tenant, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies>(HttpMethod method, Func<TRequest, TContextArguments, TDependencies, Task<object>> handler)
    {
      throw new NotImplementedException();
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(HttpMethod method, Func<TRequest, TTenant, TDependencies, Task<object>> handler)
    {
      throw new NotImplementedException();
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(HttpMethod method, Func<TRequest, TDependencies, Task<object>> handler)
    {
      string url = GetUrl();
      if (method == HttpMethod.Get)
      {
        app.MapGet(url, async context =>
        {
          TRequest request = RequestProcessing.ParseQueryString<TRequest>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          object result = await handler(request, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      else if (method == HttpMethod.Get)
      {
        app.MapGet(url, async context =>
        {
          TRequest request = RequestProcessing.ParseQueryString<TRequest>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          object result = await handler(request, dependencies);
          await context.Response.WriteAsJsonAsync(result);
        });
      }
      return this;
    }

    public IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(HttpMethod method, Func<TRequest, TDependencies, Task> handler)
    {
      string url = GetUrl();
      if (method == HttpMethod.Post)
      {
        app.MapPost(url, async context =>
        {
          TRequest request = RequestProcessing.ParseQueryString<TRequest>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, dependencies);
        });
      }
      else if (method == HttpMethod.Get)
      {
        app.MapGet(url, async context =>
        {
          TRequest request = RequestProcessing.ParseQueryString<TRequest>(context.Request);
          TDependencies dependencies = DependencyInjection.GetRequiredServices<TDependencies>(context.RequestServices);
          await handler(request, dependencies);
        });
      }
      return this;
    }
  }
}