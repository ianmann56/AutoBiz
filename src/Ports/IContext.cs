using System.Threading.Tasks;

namespace AutoBiz.Abstractions
{
  public interface IContext<TTenant>
  {
    Task<TTenant> GetTenant();

    TContextArguments GetContextArguments<TContextArguments>();
  }
}
