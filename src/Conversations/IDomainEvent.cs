namespace Conversations
{
    using System;

    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the aggregate root id of the domain event.
        /// </summary>
        Guid AggregateId { get;}

        /// <summary>
        /// Gets or sets the version of the aggregate which this event corresponds to.
        /// E.g. CreateNewCustomerCommand would map to (:NewCustomerCreated).Version = 1,
        /// as that event corresponds to the creation of the customer.
        /// </summary>
        uint Version { get; set; }
    }
}