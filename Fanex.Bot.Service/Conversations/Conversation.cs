namespace Fanex.Bot.Service.Conversations
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Models;
    using Microsoft.Bot.Connector;
    using Microsoft.Extensions.Logging;

    public interface IConversation
    {
        Task ReplyAsync(IMessageActivity activity, string message);

        Task SendAsync(MessageInfo messageInfo);

        Task<Result> SendAsync(string conversationId, string message);

        Task SendAdminAsync(string message);
    }

    public class Conversation : IConversation
    {
        private readonly ILogger<Conversation> logger;
        private readonly ISkypeConversation skypeConversation;

        public Conversation(
            ILogger<Conversation> logger,
            ISkypeConversation skypeConversation)
        {
            this.logger = logger;
            this.skypeConversation = skypeConversation;
        }

        public async Task ReplyAsync(IMessageActivity activity, string message)
        {
            switch (activity.ChannelId)
            {
                default:
                    await skypeConversation.ReplyAsync(activity, message);
                    break;
            }
        }

        public async Task SendAdminAsync(string message)
        {
            // TODO: Implement sending to admin group
        }

        public async Task SendAsync(MessageInfo messageInfo)
        {
            try
            {
                if (!string.IsNullOrEmpty(messageInfo.Text))
                {
                    await ForwardMessage(messageInfo);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}\n{ex.StackTrace}");

                await SendAdminAsync(ex.Message).ConfigureAwait(false);
            }
        }

        public async Task<Result> SendAsync(string conversationId, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var messageInfo = new MessageInfo
                {
                    ConversationId = conversationId,
                    Text = message
                };

                await SendAsync(messageInfo);

                return Result.CreateSuccessfulResult();
            }

            return Result.CreateFailedResult("Message is not valid");
        }

        private async Task ForwardMessage(MessageInfo message)
        {
            switch (message.ChannelId)
            {
                default:
                    await skypeConversation.SendAsync(message);
                    break;
            }
        }
    }
}