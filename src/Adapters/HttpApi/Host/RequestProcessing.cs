using System;
using System.Linq;
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
        throw new ArgumentException($"Could not deserialize query string to {typeof(TRequest).FullName}.");
      }

      return respObj;
    }
  }
}