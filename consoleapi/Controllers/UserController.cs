using AuthenticationPlugin;
using consoleapi.Data;
using consoleapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace consoleapi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration _configuration;
        private ApiDbContext _dbContext;

        public UsersController(ApiDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            if (user.FirstName == null)
                return BadRequest("FirstName is Required!");
            if (user.LastName == null)
                return BadRequest("LastName is Required!");

            if (CheckUserNameExist(user.UserName))
                return BadRequest("User With Same UserName Already Exists!");

            if (user.Email != null)
                if (CheckEmailExist(user.Email))
                    return BadRequest("User With Same Email Already Exists!");

            string passcheck = CheckPasswordStrenght(user.Password);
            if (!string.IsNullOrEmpty(passcheck))
                return BadRequest(passcheck);

            var UserObject = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = "Normal",
            };
            _dbContext.Users.Add(UserObject);
            _dbContext.SaveChanges();
            return Ok("New User Created Successfully");
        }

        [HttpPost]
        public Task<IActionResult> Login(User user)
        {
            var CheckUserName = _dbContext.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (CheckUserName == null)
                return Task.FromResult<IActionResult>(NotFound("UserName NotFound!"));

            if (!SecurePasswordHasherHelper.Verify(user.Password, CheckUserName.Password))
                return Task.FromResult<IActionResult>(Unauthorized("Invalid!"));

            user.Token = CreateJwt(CheckUserName);
            CheckUserName.Token = user.Token;
            _dbContext.SaveChanges();

            return Task.FromResult<IActionResult>(Ok(new
            {
                message = "Welcome Back " + CheckUserName.FirstName + " !",
                Token = user.Token
            }));
        }

        [Authorize]
        [HttpPut]
        public Task<IActionResult> EditUser(User userObject)
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);

            if (!SecurePasswordHasherHelper.Verify(userObject.Password, user.Password))
                return Task.FromResult<IActionResult>(Unauthorized("Invalid!"));

            if (userObject.FirstName != null)
                user.FirstName = userObject.FirstName;
            if (userObject.LastName != null)
                user.LastName = userObject.LastName;
            if (userObject.Email != null)
                user.Email = userObject.Email;
            if (userObject.ClientId != null)
                user.ClientId = userObject.ClientId;
            if (userObject.ClientSecret != null)
                user.ClientSecret = userObject.ClientSecret;
            if (userObject.GoogleRefreshToken != null)
                user.GoogleRefreshToken = userObject.GoogleRefreshToken;
            if (userObject.SiteUrl != null)
                user.SiteUrl = userObject.SiteUrl;
            if (userObject.ApplicationName != null)
                user.ApplicationName = userObject.ApplicationName;
            if (userObject.PhoneNumber != null)
                user.PhoneNumber = userObject.PhoneNumber;
            if (userObject.City != null)
                user.City = userObject.City;
            if (userObject.PostalCode != null)
                user.PostalCode = userObject.PostalCode;
            if (userObject.Address != null)
                user.Address = userObject.Address;

            _dbContext.SaveChanges();
            return Task.FromResult<IActionResult>(Ok("User Updated Successfully"));
        }

        [Authorize]
        [HttpGet]
        public Task<IActionResult> GetUser()
        {
            int id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(id);
            return Task.FromResult<IActionResult>(Ok(user));
        }

        private bool CheckEmailExist(string email)
        {
            var UserSameEmail = _dbContext.Users.Where(x => x.Email == email).SingleOrDefault();
            if (UserSameEmail == null)
                return false;
            else
                return true;
        }

        private bool CheckUserNameExist(string UserName)
        {
            var UserSameUserName = _dbContext.Users.Where(x => x.UserName == UserName).SingleOrDefault();
            if (UserSameUserName == null)
                return false;
            else
                return true;
        }

        private string CheckPasswordStrenght(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum Password Should be 8" + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password Should be Alphanumeric" + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Tokens:Key"]);
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,
            };
            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            return JwtTokenHandler.WriteToken(token);
        }
        
    }
}