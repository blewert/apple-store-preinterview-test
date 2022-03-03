namespace AppleGameInfo
{
    /// <summary>
    /// Functions and variables dedicated to Apple's API 
    /// </summary>
    internal class AppleGameAPI
    {
        /// <summary>
        /// The protocol to use
        /// </summary>
        private const string APPLE_LOOKUP_PROTOCOL = "http";

        /// <summary>
        /// The base url, after the protocol
        /// </summary>
        private const string APPLE_LOOKUP_BASE_URL = "itunes.apple.com/lookup";

        /// <summary>
        /// The parameter to specify a game by 
        /// </summary>
        private const string APPLE_LOOKUP_PARAM = "id";


        /// <summary>
        /// Gets a URL to look-up from, from a game ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetLookupURL(string id) => $"{APPLE_LOOKUP_PROTOCOL}://{APPLE_LOOKUP_BASE_URL}?{APPLE_LOOKUP_PARAM}={id}";
    }
}