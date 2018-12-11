namespace Fanex.Bot.Service.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Models;
    using Microsoft.Extensions.Configuration;
    using RestSharp;

    public interface ITokenService
    {
        Task<string> GetToken(string clientId, string clientPassword);
    }

    public class TokenService : ITokenService
    {
        private const string DefaultClientPassword = "Bult8TMrOurQs1JtkvbvCBjJypuSmNO8";
        private readonly IConfiguration configuration;
        private readonly IRestClient restClient;

        public TokenService(IConfiguration configuration, IRestClient restClient)
        {
            this.configuration = configuration;
            this.restClient = restClient;
        }

        public async Task<string> GetToken(string clientId, string clientPassword)
        {
            // TODO: Implement for checking client password if you want to have more password stored in DB
            if (clientPassword != DefaultClientPassword)
            {
                return null;
            }

            restClient.BaseUrl = new Uri(configuration.GetSection("Bot:TokenUrl").Value);

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", configuration.GetSection("Bot:MicrosoftAppId").Value);
            request.AddParameter("client_secret", configuration.GetSection("Bot:MicrosoftAppPassword").Value);
            request.AddParameter("scope", $"{configuration.GetSection("Bot:MicrosoftAppId").Value}/.default");
            request.AddHeader("Content-type", "application/x-www-form-urlencoded");
            var response = await restClient.ExecuteTaskAsync<Token>(request).ConfigureAwait(false);

            return response.Data.AccessToken;
        }
    }
}