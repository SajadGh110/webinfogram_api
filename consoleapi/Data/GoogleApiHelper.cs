using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Webmasters.v3;
using Google.Apis.Webmasters.v3.Data;

public class GoogleApiHelper
{
    public static WebmastersService GetService(string ClientID , string ClientSecret , string ApplicationName , string RefreshToken)
    {
        ClientSecrets secrets = new ClientSecrets
        {
            ClientId = ClientID,
            ClientSecret = ClientSecret
        };
        string[] scopes = new[] { WebmastersService.Scope.Webmasters };
        var datastore = new FileDataStore("./oauth", true);

        GoogleAuthorizationCodeFlow.Initializer initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = scopes,
            DataStore = datastore
        };

        GoogleAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(initializer);

        TokenResponse newtoken = flow.RefreshTokenAsync("user", RefreshToken, CancellationToken.None).Result;

        UserCredential credential = new UserCredential(flow, "user", newtoken);

        var service = new WebmastersService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });

        return service;
    }

    public static SearchAnalyticsQueryResponse QueryData(WebmastersService service , string SiteUrl , DateTime StartDate , DateTime EndDate , string query = "query")
    {
        var request = new SearchAnalyticsQueryRequest
        {
            StartDate = StartDate.ToString("yyyy-MM-dd"),
            EndDate = EndDate.ToString("yyyy-MM-dd"),
            Dimensions = new List<string> { query },
            RowLimit = 25000,
        };

        var response = service.Searchanalytics.Query(request, SiteUrl).Execute();

        return response;
    }
}
