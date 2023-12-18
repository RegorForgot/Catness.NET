using RestSharp;

namespace Catness.Clients;

public interface IRestClient
{
    public RestClient Client { get; }
    public string BaseUrl { get; }
}