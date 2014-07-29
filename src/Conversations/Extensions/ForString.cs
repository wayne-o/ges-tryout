namespace Conversations.Extensions
{
    using System.Globalization;

    public static class ForString
    {
        public static string With(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentUICulture, format, args);
        }
    }
}