namespace Fanex.Bot.Service.Controllers
{
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService tokenService;

        public TokenController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Generate MS Token using in Fanex.Bot.Client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientPassword"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string clientId, string clientPassword)
        {
            var token = await tokenService.GetToken(clientId, clientPassword);

            if (string.IsNullOrEmpty(token))
            {
                return "Invalid client information";
            }

            return token;
        }
    }
}