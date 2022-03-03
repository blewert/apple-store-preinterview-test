﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AppleGameInfo
{
    internal class AppleGameAPI
    {
        private const string APPLE_LOOKUP_PROTOCOL = "https";
        private const string APPLE_LOOKUP_BASE_URL = "itunes.apple.com/lookup";
        private const string APPLE_LOOKUP_PARAM    = "id";
        
        public static string GetLookupURL (string id) => $"{APPLE_LOOKUP_PROTOCOL}://{APPLE_LOOKUP_BASE_URL}?{APPLE_LOOKUP_PARAM}={id}";
    }

    [Serializable]
    public class AppleGameObject
    {

    }

    internal class HttpParseException : Exception
    {
        public HttpParseException() { }
        public HttpParseException(string message) : base(message) { }
        public HttpParseException(string message, Exception inner) : base(message, inner) { }
    }

    internal static class HTTPUtils
    {
        internal static async Task<HttpWebResponse> Get(string url)
        {
            //Make a web request
            HttpWebRequest req = WebRequest.CreateHttp(url);

            //Return response
            return (HttpWebResponse)(await req.GetResponseAsync());
        }

        internal static (bool valid, string reason) IsResponseValid(in HttpWebResponse response)
        {
            //Not found? Ok, tell them:
            if (response.StatusCode == HttpStatusCode.NotFound)
                return (false, "Request returned not found status. Are you sure the API url is valid?");

            //Forbidden? Lets get out of here
            if (response.StatusCode == HttpStatusCode.Forbidden)
                return (false, "Request returned forbidden -- have you entered the correct credentials?");

            //Something went kaput? Tell them to retry.
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return (false, "An internal server error occurred. Maybe try run the program again.");


            //Otherwise, at this point, we've filtered out the main 3 categories of 
            //response. Lets check if its a generic error:
            if (response.StatusCode != HttpStatusCode.OK)
                return (false, $"An error occurred during the request: ${response.StatusDescription}");

            //..

            //At this point, the status code has to be 200 OK. Lets check some other things.
            //.

            //Is the content type not text/json or application/json? Or weirdly, text/javascript? If so.. die
            if (!(response.ContentType.EndsWith("/json") || response.ContentType.StartsWith("text/javascript")))
                return (false, $"Unexpected content type '{response.ContentType}', I expected some kind of json.");

            //Everything at this point is all good, by our standards
            return (true, default);
        }
    }

    public class Game
    {
        /// <summary>
        /// The ID to look up, for this game
        /// </summary>
        private string lookupID;

        /// <summary>
        /// Constructs a new game with a given lookup id.
        /// </summary>
        /// <param name="lookupID">The lookup id.</param>
        public Game(string lookupID) 
        {
            this.lookupID = lookupID;
        }

        public AppleGameObject Lookup()
        {
            //Ok, first, check if the lookup id is valid. It's a string, anything could be in there.
            //But why are we using strings, why not a uint or int? Well two reasons:
            //
            // a) The ids are very large numbers and could overflow.
            // b) They are not used in any calculation, just to be shoved into a string
            //
            if (!int.TryParse(this.lookupID, out _))
                throw new FormatException($"Lookup ID {this.lookupID} is not numeric. It needs to be a number.");

            //Get lookup url
            string lookupURL = AppleGameAPI.GetLookupURL(this.lookupID);

            //Send GET request
            HttpWebResponse response = HTTPUtils.Get(lookupURL).Result;

            //Get validity of response
            var responseValidityCheck = HTTPUtils.IsResponseValid(in response);

            //It's not valid? uh-oh, get out of here, throw an exception
            if (!responseValidityCheck.valid)
                throw new HttpParseException(responseValidityCheck.reason);

            //Otherwise, we're all good.
            return new AppleGameObject();
        }
    }
}