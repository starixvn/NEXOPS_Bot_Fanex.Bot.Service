namespace Fanex.Bot.Service.Tests.Conversations
{
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Conversations;
    using Fanex.Bot.Service.Models;
    using Fanex.Bot.Skynex.Tests.Fixtures;
    using Microsoft.Bot.Connector;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class ConversationTests : IClassFixture<BotConversationFixture>
    {
        private readonly BotConversationFixture conversationFixture;
        private readonly ILogger<Conversation> subLogger;
        private readonly ISkypeConversation subSkypeConversation;
        private readonly IConversation conversation;

        public ConversationTests(BotConversationFixture conversationFixture)
        {
            this.conversationFixture = conversationFixture;
            subLogger = Substitute.For<ILogger<Conversation>>();
            subSkypeConversation = Substitute.For<ISkypeConversation>();
            conversation = new Conversation(subLogger, subSkypeConversation);
        }

        [Fact]
        public async Task ReplyAsync_DefaultChannel_SendToSkype()
        {
            // Act
            await conversation.ReplyAsync(conversationFixture.Activity, "hello");

            // Assert
            await subSkypeConversation.Received().ReplyAsync(Arg.Is(conversationFixture.Activity), "hello");
        }

        [Fact]
        public async Task SendAsync_MessageIsNotNullOrEmpty_SendMessage()
        {
            // Arrange
            var message = "Hello";
            var conversationId = "1234";

            // Act
            var result = await conversation.SendAsync(conversationId, message);

            // Assert
            await subSkypeConversation.Received().SendAsync(Arg.Is<MessageInfo>(mf => mf.Text == message && mf.ConversationId == conversationId));
            Assert.True(result.IsOk);
        }

        [Fact]
        public async Task SendAsync_MessageIsNullOrEmpty_ReturnNotOkResult()
        {
            // Arrange
            var message = "";
            var conversationId = "1234";

            // Act
            var result = await conversation.SendAsync(conversationId, message);

            // Assert
            Assert.False(result.IsOk);
        }
    }
}