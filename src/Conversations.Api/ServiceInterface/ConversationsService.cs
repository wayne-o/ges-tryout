using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ServiceStack;

namespace Conversations.Api.ServiceInterface
{
    using System.Net;

    using Conversations.Commands;
    using Conversations.Dto;

    using MassTransit;

    using Raven.Client;
    using Raven.Client.Connection;

    public class ConversationsService : Service
    {

        private readonly IDocumentStore store;

        private readonly IServiceBus serviceBus;

        public ConversationsService(IDocumentStore store, IServiceBus serviceBus)
        {
            this.store = store;
            this.serviceBus = serviceBus;
        }

        public async Task<object> Post(PostConversation request)
        {
            if (string.IsNullOrEmpty(request.Data.Id))
            {
                throw new ArgumentException("message id cannot be null");
            }

            this.serviceBus.Publish(new CreateNewConversation(request.Data));

            base.Response.StatusCode = 201;

            return new { Success = true };
        }

        public async Task<object> Get(GetConversation request)
        {
            using (var session = store.OpenAsyncSession())
            {
                var result = await session.LoadAsync<DenormalizedConversation>(request.Id);

                return new GetConversationResponse
                           {
                               Data = result
                           };
            }
        }
    }

    [Route("/conversations", "GET")]
    [Route("/conversations/{Slug}", "GET")]
    public class GetConversation : IReturn<GetConversationResponse>
    {
        public string Id { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Slug { get; set; }
    }

    public class GetConversationResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }

        public DenormalizedConversation Data { get; set; }
    }

    [Route("/conversations", "POST")]
    public class PostConversation : IReturn<PostConversationResponse>
    {
        public ConversationDto Data { get; set; }
    }

    public class PostConversationResponse : IHasResponseStatus
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}