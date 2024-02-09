using System.Net.Mime;
using System.Net;
using Bogus;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Newtonsoft.Json;
using WireMock.Matchers;

namespace Ahsoka.TestsUtil;

public static class GeneralFixture
{
    public static Faker Faker { get; set; } = new Faker();

    public static string GetStringRightSize(int minLength, int maxLength)
    {
        var stringValue = Faker.Lorem.Random.Words(2);

        while (stringValue.Length < minLength)
        {
            stringValue += Faker.Lorem.Random.Words(2);
        }

        if (stringValue.Length > maxLength)
        {
            stringValue = stringValue[..maxLength];
        }

        return stringValue;
    }

    public static WireMockServer CreateWireMock()
    {
        return WireMockServer.Start(9999);
    }

    public static void ResetWireMock(this WireMockServer? _wireMockServer)
    {
        _wireMockServer?.Reset();
    }

    public static void RegisterWireMockResponse(this WireMockServer? _wireMockServer, IRequestBuilder request, IResponseBuilder response)
    {
        _wireMockServer?
        .Given(request)
            .RespondWith(response);
    }

    public static IRequestBuilder CreateWireMockRequest(HttpMethod method, string path)
    {
        return CreateWireMockRequest(method, path, new Dictionary<string, string>(), null!);
    }

    public static IRequestBuilder CreateWireMockRequest(HttpMethod method, string path, object body)
    {
        return CreateWireMockRequest(method, path, new Dictionary<string, string>(), body);
    }

    public static IRequestBuilder CreateWireMockRequest(HttpMethod method, string path, Dictionary<string, string> prams)
    {
        return CreateWireMockRequest(method, path, prams, null!);
    }

    public static IRequestBuilder CreateWireMockRequest(HttpMethod method, string path, Dictionary<string, string> prams, object body)
    {
        IRequestBuilder requestBuilder = Request.Create();
        requestBuilder.WithPath(path);

        foreach (KeyValuePair<string, string> keyValue in prams)
        {
            requestBuilder.WithParam(keyValue.Key, keyValue.Value);
        }

        if (body != null)
        {
            var jsonBody = JsonConvert.SerializeObject(body);

            requestBuilder.WithBody(new JsonMatcher(jsonBody, true));
        }

        return method.Method switch
        {
            "GET" => requestBuilder.UsingGet(),
            "POST" => requestBuilder.UsingPost(),
            "PUT" => requestBuilder.UsingPut(),
            "DELETE" => requestBuilder.UsingDelete(),
            _ => requestBuilder.UsingAnyMethod()
        };
    }

    public static IResponseBuilder CreateWireMockResponse(HttpStatusCode code)
    {
        IResponseBuilder responseBuilder = Response.Create();
        responseBuilder.WithStatusCode(code);
        responseBuilder.WithHeader("Content-Type", MediaTypeNames.Application.Json);

        return responseBuilder;
    }

    public static IResponseBuilder CreateWireMockResponse(HttpStatusCode code, object body)
    {
        IResponseBuilder responseBuilder = CreateWireMockResponse(code);

        responseBuilder.WithBodyAsJson(body);

        return responseBuilder;
    }

    public static T CreateInstanceAndSetProperties<T>(Dictionary<string, object> propertyValues) where T : class
    {
        Type type = typeof(T);

        var instance = (T)Activator.CreateInstance(type, true);

        foreach (var property in typeof(T).GetProperties())
        {
            if (propertyValues.TryGetValue(property.Name, out var value))
            {
                property.SetValue(instance, value);
            }
        }

        return instance;
    }

}
