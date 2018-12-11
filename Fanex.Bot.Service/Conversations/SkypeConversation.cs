namespace Fanex.Bot.Service.Conversations
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Extensions.Configuration;

    public interface ISkypeConversation
    {
        Task ReplyAsync(IMessageActivity activity, string message);

        Task SendAsync(MessageInfo messageInfo);
    }

    public class SkypeConversation : ISkypeConversation
    {
        private const string DefaultServiceUrl = "https://smba.trafficmanager.net/apis/";
        private readonly IConfiguration configuration;

        public SkypeConversation(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task ReplyAsync(IMessageActivity activity, string message)
        {
            var connector = await CreateConnectorClient(new Uri(activity.ServiceUrl));
            var reply = (activity as Activity).CreateReply(message);

            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        public async Task SendAsync(MessageInfo messageInfo)
        {
            var serviceUrl = configuration.GetSection("Bot:SkypeServiceUrl")?.Value ?? DefaultServiceUrl;
            var connector = await CreateConnectorClient(new Uri(serviceUrl));
            var message = CreateMessageActivity(messageInfo);

            if (!string.IsNullOrEmpty(messageInfo.Text))
            {
                message.Text = messageInfo.Text;
                await connector.Conversations.SendToConversationAsync((Activity)message);
            }
        }

        private async Task<ConnectorClient> CreateConnectorClient(Uri serviceUrl)
        {
            var account = new MicrosoftAppCredentials(
                configuration.GetSection("Bot:MicrosoftAppId").Value,
                configuration.GetSection("Bot:MicrosoftAppPassword").Value);

            var jwtToken = await account.GetTokenAsync(forceRefresh: true);

            return new ConnectorClient(
                 serviceUrl,
                 account,
                 handlers: new BotDelegatingHandler(jwtToken));
        }

        private static IMessageActivity CreateMessageActivity(MessageInfo messageInfo)
        {
            var userAccount = new ChannelAccount(messageInfo.ToId, messageInfo.ToName);
            var botAccount = new ChannelAccount(messageInfo.FromId, messageInfo.FromName);
            var message = Activity.CreateMessageActivity();
            message.ChannelId = messageInfo.ChannelId;
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: messageInfo.ConversationId);

            return message;
        }
    }
}