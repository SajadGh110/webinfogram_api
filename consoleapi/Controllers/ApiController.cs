using consoleapi.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.Webmasters.v3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace consoleapi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private ApiDbContext _dbContext;
        public ApiController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetData_date(DateTime StartDate, DateTime EndDate)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret) || string.IsNullOrWhiteSpace(user.ApplicationName) || string.IsNullOrWhiteSpace(user.SiteUrl) || string.IsNullOrWhiteSpace(user.GoogleRefreshToken))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have Prerequisite information"));
            else
            {
                var service = GoogleApiHelper.GetService(user.ClientId, user.ClientSecret, user.ApplicationName, user.GoogleRefreshToken);

                var response = GoogleApiHelper.QueryData(service, user.SiteUrl, StartDate, EndDate, "date");

                return Task.FromResult<IActionResult>(Ok(response));
            }
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetData_query(DateTime StartDate , DateTime EndDate)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret) || string.IsNullOrWhiteSpace(user.ApplicationName) || string.IsNullOrWhiteSpace(user.SiteUrl) || string.IsNullOrWhiteSpace(user.GoogleRefreshToken))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have Prerequisite information"));
            else
            {
                var service = GoogleApiHelper.GetService(user.ClientId, user.ClientSecret, user.ApplicationName, user.GoogleRefreshToken);

                var response = GoogleApiHelper.QueryData(service, user.SiteUrl, StartDate, EndDate, "query");

                return Task.FromResult<IActionResult>(Ok(response));
            }
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetData_country(DateTime StartDate, DateTime EndDate)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret) || string.IsNullOrWhiteSpace(user.ApplicationName) || string.IsNullOrWhiteSpace(user.SiteUrl) || string.IsNullOrWhiteSpace(user.GoogleRefreshToken))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have Prerequisite information"));
            else
            {
                var service = GoogleApiHelper.GetService(user.ClientId, user.ClientSecret, user.ApplicationName, user.GoogleRefreshToken);

                var response = GoogleApiHelper.QueryData(service, user.SiteUrl, StartDate, EndDate, "country");

                return Task.FromResult<IActionResult>(Ok(response));
            }
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetData_device(DateTime StartDate, DateTime EndDate)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret) || string.IsNullOrWhiteSpace(user.ApplicationName) || string.IsNullOrWhiteSpace(user.SiteUrl) || string.IsNullOrWhiteSpace(user.GoogleRefreshToken))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have Prerequisite information"));
            else
            {
                var service = GoogleApiHelper.GetService(user.ClientId, user.ClientSecret, user.ApplicationName, user.GoogleRefreshToken);

                var response = GoogleApiHelper.QueryData(service, user.SiteUrl, StartDate, EndDate, "device");

                return Task.FromResult<IActionResult>(Ok(response));
            }
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetData_page(DateTime StartDate, DateTime EndDate)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret) || string.IsNullOrWhiteSpace(user.ApplicationName) || string.IsNullOrWhiteSpace(user.SiteUrl) || string.IsNullOrWhiteSpace(user.GoogleRefreshToken))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have Prerequisite information"));
            else
            {
                var service = GoogleApiHelper.GetService(user.ClientId, user.ClientSecret, user.ApplicationName, user.GoogleRefreshToken);

                var response = GoogleApiHelper.QueryData(service, user.SiteUrl, StartDate, EndDate, "page");

                return Task.FromResult<IActionResult>(Ok(response));
            }
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> save_token(string code)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);

            if (string.IsNullOrWhiteSpace(user.ClientId) || string.IsNullOrWhiteSpace(user.ClientSecret))
                return Task.FromResult<IActionResult>(BadRequest("Please Complate Profile First! dont have ClientId or ClientSecret"));

            ClientSecrets secrets = new ClientSecrets
            {
                ClientId = user.ClientId,
                ClientSecret = user.ClientSecret
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

            try
            {
                TokenResponse token = flow.ExchangeCodeForTokenAsync("user", code, "http://localhost:4200/getcode", CancellationToken.None).Result;

                user.GoogleRefreshToken = token.RefreshToken;
                _dbContext.SaveChanges();

                return Task.FromResult<IActionResult>(Ok("Refresh Token Saved!"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(BadRequest(ex.Message));
            }
        }
    }
}

