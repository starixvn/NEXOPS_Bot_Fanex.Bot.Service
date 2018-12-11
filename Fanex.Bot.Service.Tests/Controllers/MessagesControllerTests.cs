namespace Fanex.Bot.Service.Tests.Controllers
{
    using System.Threading.Tasks;
    using Fanex.Bot.Controllers;
    using Fanex.Bot.Service.Models;
    using Fanex.Bot.Skynex.Tests.Fixtures;
    using Microsoft.Bot.Connector;
    using NSubstitute;
    using Xunit;

    public class MessagesControllerTests : IClassFixture<BotConversationFixture>
    {
        private readonly BotConversationFixture conversationFixture;
        private readonly MessagesController messagesController;

        public MessagesControllerTests(BotConversationFixture conversationFixture)
        {
            this.conversationFixture = conversationFixture;
            messagesController = new MessagesController(this.conversationFixture.Conversation);
        }

        [Fact]
        public async Task Post_ActivityMessage_SendHelloMessage()
        {
            // Arrange
            var activity = new Activity { Type = ActivityTypes.Message };

            // Act
            await messagesController.Post(activity);

            // Asserts
            await conversationFixture
                .Conversation
                .Received()
                .ReplyAsync(Arg.Is(activity), "Hello");
        }

        [Fact]
        public async Task Forward_Always_SendToConversation_ReturnOk()
        {
            // Arrange
            var conversationId = "@#423424";
            var message = "hello";
            var expectedResult = Result.CreateSuccessfulResult();
            conversationFixture.Conversation.SendAsync(conversationId, message).Returns(expectedResult);

            // Act
            var result = await messagesController.Forward(message, conversationId);

            // Assert
            await conversationFixture.Conversation.Received(1).SendAsync(conversationId, message);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(expectedResult, result.Value);
        }
    }
}