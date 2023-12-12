using RestSharp;

namespace Catness.Services;

public interface IRestAPIService
{
    public RestClient Client { get; }
    string BaseUrl { get; }
}