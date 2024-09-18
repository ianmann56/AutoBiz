using System;
using System.Collections.Generic;
using System.Linq;
using AutoBiz.Adapters.HttpApi.Tenants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public class RouteGroupBuilder<TTenant> : IRouteGroupBuilder<TTenant>
  {
    private WebApplication app;
    private IEnumerable<string> urlSegments;

    private List<IRouteGroupBuilder<TTenant>> groups = new List<IRouteGroupBuilder<TTenant>>();
    private List<IRouteBuilder<TTenant>> routes = new List<IRouteBuilder<TTenant>>();

    public RouteGroupBuilder(WebApplication app, IEnumerable<string> urlSegments)
    {
      this.app = app;
      this.urlSegments = urlSegments;
    }
    
    public IRouteGroupBuilder<TTenant> AddGroup(string route, Action<IRouteGroupBuilder<TTenant>> configure)
    {
      IEnumerable<string> nestedUrlSegments = this.urlSegments.Append(route);

      RouteGroupBuilder<TTenant> nestedGroupBuilder = new RouteGroupBuilder<TTenant>(this.app, nestedUrlSegments);
      configure(nestedGroupBuilder);

      groups.Add(nestedGroupBuilder);

      return this;
    }

    public IRouteGroupBuilder<TTenant> AddRoute(string route, Action<IRouteBuilder<TTenant>> configure)
    {
      IEnumerable<string> nestedUrlSegments = this.urlSegments.Append(route);

      AuthenticatedTenentResolver<TTenant>? authenticatedTenentResolver = this.app.Services.GetService<AuthenticatedTenentResolver<TTenant>>();
      if (authenticatedTenentResolver == null)
      {
        throw new InvalidOperationException($"No {typeof(AuthenticatedTenentResolver<TTenant>).FullName} registered for type {typeof(TTenant)}. Please add one to the application services.");
      }

      RouteBuilder<TTenant> routeBuilder = new RouteBuilder<TTenant>(this.app, nestedUrlSegments, authenticatedTenentResolver);
      configure(routeBuilder);

      routes.Add(routeBuilder);

      return this;
    }
  }
}