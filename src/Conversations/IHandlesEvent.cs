namespace Conversations
{
    using MassTransit;

    public interface IHandlesEvent<T> : Consumes<T>.All
        where T : class, IDomainEvent
    {
    }
}