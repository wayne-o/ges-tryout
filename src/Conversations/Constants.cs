namespace Conversations
{
    using Newtonsoft.Json;

    public class Constants
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };
    }
}