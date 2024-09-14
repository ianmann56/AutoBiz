using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoBiz.Adapters.HttpApi.Routing
{
  public interface IRouteBuilder<TTenant>
  {
    IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TTenant, TDependencies, Task<TResult>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TContextArguments, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TContextArguments, TDependencies, Task<TResult>> handler);

    IRouteBuilder<TTenant> AddCommand<TRequest, TContextArguments, TDependencies>(
      Func<TRequest, TContextArguments, TDependencies, Task> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TTenant, TDependencies, Task<TResult>> handler);

    IRouteBuilder<TTenant> AddHandler<TRequest, TDependencies, TResult>(
      HttpMethod method,
      Func<TRequest, TDependencies, Task<TResult>> handler);

    IRouteBuilder<TTenant> AddCommand<TRequest, TDependencies>(
      Func<TRequest, TDependencies, Task> handler);
  }
}