using System.Net;
using Catness.Enums;
using Catness.Persistence.Models;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Catness.Clients;

public class MakesweetClient : IRestClient
{
    public RestClient Client { get; }
    public string BaseUrl => "https://api.makesweet.com/make";

    public MakesweetClient(IOptions<BotConfiguration> botConfigurationOptions)
    {
        Client = new RestClient(BaseUrl);
        BotConfiguration botConfiguration = botConfigurationOptions.Value;
        Client.AddDefaultHeader("Authorization", botConfiguration.APIKeys.MakesweetKey);
    }

    public async Task<byte[]?> GetMakesweetGif(
        MakesweetTemplate template,
        byte[]? imageBytes,
        string? text,
        int textBorder,
        bool textFirst)
    {
        if (imageBytes is null && text is null)
        {
            return null;
        }

        RestRequest request = new RestRequest(template.GetMakesweetURL());

        if (imageBytes is not null)
        {
            request.AddFile("images", imageBytes, "image.png");
        }
        if (text is not null)
        {
            request.AddParameter("text", text, ParameterType.QueryString);
        }
        if (textBorder != 0)
        {
            request.AddParameter("textborder", textBorder.ToString(), ParameterType.QueryString);
        }
        if (textFirst)
        {
            request.AddParameter("textfirst", 1.ToString(), ParameterType.QueryString);
        }

        request.Timeout = 20000;

        RestResponse<byte[]?> response = await Client.ExecutePostAsync<byte[]?>(request);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException(response.ErrorMessage, null, response.StatusCode);
        }
        return response.RawBytes;
    }
}