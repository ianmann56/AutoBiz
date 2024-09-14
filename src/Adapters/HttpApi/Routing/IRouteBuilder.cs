using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public interface IRouteBuilder<TTenant>
  {
    IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TTenant, TDependencies, Task<object>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TDependencies, Task<object>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task<object>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task<object>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task> handler);
  }
}