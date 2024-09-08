using System;
using AutoBiz.Adapters.HttpApi.Routing;
using Microsoft.AspNetCore.Builder;

namespace AutoBiz.Adapters.HttpApi.Middleware
{
  public static class ConfigureAutoBizMiddleware
  {
    public static WebApplication UseAutoBiz<TTenant>(this WebApplication app, Action<IRouteGroupBuilder<TTenant>> configure)
    {
      var builder = new RouteGroupBuilder<TTenant>(app, Array.Empty<string>());
      configure(builder);
      return app;
    }
  }
}