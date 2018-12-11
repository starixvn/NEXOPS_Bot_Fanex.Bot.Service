namespace Fanex.Bot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.Bot.Service.Conversations;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Connector;

    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly IConversation conversation;

        public MessagesController(IConversation conversation)
        {
            this.conversation = conversation;
        }

        /// <summary>
        /// API for interact with Microsoft Bot Service
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        [Authorize(Roles = "Bot")]
        [HttpPost]
        public async Task<OkResult> Post([FromBody] Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await conversation.ReplyAsync(activity, "Hello");
                    break;

                case ActivityTypes.ConversationUpdate:
                    // TODO: Handle conversation update
                    break;

                case ActivityTypes.ContactRelationUpdate:
                    // TODO: Handle contact relation update
                    break;

                default:
                    return Ok();
            }

            return Ok();
        }

        /// <summary>
        /// API for send message using in Fanex.Bot.Client
        /// </summary>
        /// <param name="message"></param>
        /// <param name="conversationId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Bot")]
        [HttpPost]
        [Route("Forward")]
        public async Task<OkObjectResult> Forward(string message, string conversationId)
        {
            var result = await conversation.SendAsync(conversationId, message);

            return Ok(result);
        }
    }
}