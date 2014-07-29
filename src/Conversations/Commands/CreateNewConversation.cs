using System;

namespace Conversations.Commands
{
    using Conversations.Dto;

    public class CreateNewConversation
    {
        public CreateNewConversation(ConversationDto data)
        {
            this.Id = Guid.NewGuid();
            this.Conversation = data;
        }

        public CreateNewConversation()
        {

        }

        public ConversationDto Conversation { get; set; }

        public Guid Id { get; private set; }
    }
}
