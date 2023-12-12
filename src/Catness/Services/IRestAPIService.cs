using RestSharp;

namespace Catness.Services;

public interface IRestAPIService
{
    public RestClient Client { get; }
    public string BaseUrl { get; }
}