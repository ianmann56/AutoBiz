using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AutoBiz.Adapters.HttpApi.Http
{
  public static class RequestProcessing
  {
    public static TRequest ParseQueryString<TRequest>(HttpRequest request)
    {
      var dict = HttpUtility.ParseQueryString(request.QueryString.Value ?? "");
      string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
      TRequest? respObj = JsonConvert.DeserializeObject<TRequest>(json);

      if (respObj == null)
      {
        throw new ArgumentException($"Could not convert query string to {typeof(TRequest).FullName}.");
      }

      return respObj;
    }

    public static TContextArguments ParseRouteArguments<TContextArguments>(HttpRequest request)
    {
      var dict = request.RouteValues;
      string json = JsonConvert.SerializeObject(dict);
      TContextArguments? respObj = JsonConvert.DeserializeObject<TContextArguments>(json);

      if (respObj == null)
      {
        throw new ArgumentException($"Could not convert route to {typeof(TContextArguments).FullName}.");
      }

      return respObj;
    }

    public static async Task<TRequest> ParseBody<TRequest>(HttpRequest request)
    {
      // Read the body as a string
      string body = await new StreamReader(request.Body).ReadToEndAsync();

      TRequest? respObj = JsonConvert.DeserializeObject<TRequest>(body);

      if (respObj == null)
      {
        throw new ArgumentException($"Could not deserialize query string to {typeof(TRequest).FullName}.");
      }

      return respObj;
    }
  }
}