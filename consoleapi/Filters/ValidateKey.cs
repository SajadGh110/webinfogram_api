using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace consoleapi.Filters
{
    public class ValidateKey : ActionFilterAttribute
    {
        private readonly string _key;
        public ValidateKey(string Key)
        {
            _key = Key;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string api_key = context.HttpContext.Request.Headers["api-key"].ToString();
            if (api_key.IsNullOrEmpty() || api_key != _key)
                context.Result = new BadRequestObjectResult("You Don't Have a Permission to Access This Api");
            base.OnResultExecuting(context);
        }
    }
}
