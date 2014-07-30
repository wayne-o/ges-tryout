namespace Conversations
{
    using RabbitMQ.Client.Impl;

    public class DenormalizedConversation
    {
        public string Body { get; set; }

        public string Subject { get; set; }
    }
}