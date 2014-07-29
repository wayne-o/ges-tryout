namespace Conversations.Installers
{
    using Conversations.Extensions;

    public static class Keys
    {
        public static string RmqServer = "localhost";

        public static readonly string WebEndpoint = "rabbitmq://{0}/Sonatribe.Web".With(RmqServer);
        public static readonly string DomainServiceEndpoint = "rabbitmq://{0}/Sonatribe.Domain".With(RmqServer);

        public static readonly string DomainBusName = "Domain";
        public static readonly string ReadModelBusName = "ReadModel";

        
    }
}