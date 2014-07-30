namespace Conversations
{
    using System;

    public interface ICommand
    {
        /// <summary>
        /// Gets or sets AggregateId.
        /// </summary>
        Guid AggregateId { get; set; }

        /// <summary>
        /// Gets or sets Version.
        /// </summary>
        uint Version { get; set; }
    }
}