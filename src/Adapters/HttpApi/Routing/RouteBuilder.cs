using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
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
      return string.Join("/", urlSegments.Select(s => s.Trim('/')).ToList());
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
          TRequest request = ParseQueryString<TRequest>(context.Request);
          TTenant tenant = null; // Get the tenant from authentication services.
          TContextArguments contextArguments = null; // Get the args from the URL
          TDependencies dependencies = null; // Use reflection to get the dependencies from the DI container for each property in TDependencies.
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
      throw new NotImplementedException();
    }

    private TRequest ParseQueryString<TRequest>(HttpRequest request)
    {
      var dict = HttpUtility.ParseQueryString(request.QueryString.Value ?? "");
      string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
      TRequest? respObj = JsonConvert.DeserializeObject<TRequest>(json);

      if (respObj == null)
      {
        throw new Exception($"Could not deserialize query string to {typeof(TRequest).FullName}.");
      }

      return respObj;
    }
  }
}