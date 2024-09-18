using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AutoBiz.Adapters.HttpApi.Tenants
{
  public delegate Task<TTenant> AuthenticatedTenentResolver<TTenant>(HttpContext context);

  public static class AuthenticatedTenentResolvers
  {
    public static AuthenticatedTenentResolver<object> Default => async context => await Task.FromResult<object>(new object());
  }
}