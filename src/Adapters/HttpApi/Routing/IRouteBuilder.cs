using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoBiz.Adapters.HttpApi.Tenants;
using Microsoft.AspNetCore.Http;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public interface IRouteBuilder
  {
    IRouteBuilder AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TDependencies, Task<TResult>> handler);

    IRouteBuilder AddCommand<TRequest, TContextArguments, TDependencies>(
      Func<TRequest, TContextArguments, TDependencies, Task> handler);

    IRouteBuilder AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task<TResult>> handler);

    IRouteBuilder AddCommand<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task> handler);

    IRouteBuilder AddHandler<TRequest, TResult>(
      HttpMethod method,
      Func<TRequest, Task<TResult>> handler);

    IRouteBuilder AddCommand<TRequest>(
      HttpMethod method,
      Func<TRequest, Task> handler);

    IRouteBuilder<TTenant> AddTenant<TTenant>(AuthenticatedTenentResolver<TTenant> tenantResolver);
  }
}