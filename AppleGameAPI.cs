namespace AppleGameInfo
{
    internal class AppleGameAPI
    {
        private const string APPLE_LOOKUP_PROTOCOL = "http";
        private const string APPLE_LOOKUP_BASE_URL = "itunes.apple.com/lookup";
        private const string APPLE_LOOKUP_PARAM = "id";

        public static string GetLookupURL(string id) => $"{APPLE_LOOKUP_PROTOCOL}://{APPLE_LOOKUP_BASE_URL}?{APPLE_LOOKUP_PARAM}={id}";
    }
}