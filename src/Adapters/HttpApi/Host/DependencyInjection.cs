using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoBiz.Adapters.HttpApi.Host
{
  public static class DependencyInjection
  {
    /// <summary>
    /// Gets the services specified in type TDeps from the service provider. All properties of TDeps will
    /// be populated with the corresponding services from the service provider. If one of the properties
    /// cannot be resolved, an exception will be thrown.
    /// </summary>
    /// <typeparam name="TDeps"></typeparam>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static TDeps GetRequiredServices<TDeps>(this IServiceProvider serviceProvider)
    {
      ConstructorInfo? constructor = GetConstructor<TDeps>();

      if (constructor.GetParameters().Length == 0)
      {
        return ConstructViaParameterlessConstructor<TDeps>(constructor, serviceProvider);
      }
      else
      {
        return ConstructViaParameterizedConstructor<TDeps>(constructor, serviceProvider);
      }
    }

    private static TDeps ConstructViaParameterlessConstructor<TDeps>(ConstructorInfo constructor, IServiceProvider serviceProvider)
    {
      TDeps dependencies = (TDeps) constructor.Invoke([]);
      
      typeof(TDeps).GetProperties()
        .ToList()
        .ForEach(prop =>
        {
          object service = serviceProvider.GetRequiredService(prop.PropertyType);
          prop.SetValue(dependencies, service);
        });
      return dependencies;
    }

    private static TDeps ConstructViaParameterizedConstructor<TDeps>(ConstructorInfo constructor, IServiceProvider serviceProvider)
    {
      ParameterInfo[] parameters = constructor.GetParameters();
      object[] parameterValues = parameters.Select(p => serviceProvider.GetRequiredService(p.ParameterType)).ToArray();
      TDeps dependencies = (TDeps) constructor.Invoke(parameterValues);
      
      return dependencies;
    }

    private static ConstructorInfo GetConstructor<TDeps>()
    {
      if (typeof(TDeps).GetConstructors().Length > 1)
      {
        throw new InvalidOperationException($"Cannot determine which constructor to use to instantiate {typeof(TDeps).Name}. Please provide a parameterless constructor or a single constructor with parameters.");
      }

      ConstructorInfo? parameterizedConstructor = typeof(TDeps).GetConstructors().SingleOrDefault(c => c.GetParameters().Length > 0);

      if (parameterizedConstructor != null)
      {
        return parameterizedConstructor;
      }
      
      ConstructorInfo? emptyConstructor = typeof(TDeps).GetConstructors().SingleOrDefault(c => c.GetParameters().Length == 0);

      if (emptyConstructor != null)
      {
        return emptyConstructor;
      }

      throw new InvalidOperationException($"Cannot find a constructor with which to instantiate {typeof(TDeps).Name}. Please provide a parameterless constructor or a single constructor with parameters.");
    }
  }
}