using RestSharp;

namespace Catness.Services;

public class MakesweetAPIService : IRestAPIService
{
    public RestClient Client { get; }
    public string BaseUrl => "https://api.makesweet.com/make/";

    public MakesweetAPIService()
    {
        Client = new RestClient(BaseUrl);
    }
}