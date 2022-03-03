﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppleGameInfo
{
    internal class AppleGameAPI
    {
        private const string APPLE_LOOKUP_PROTOCOL = "http";
        private const string APPLE_LOOKUP_BASE_URL = "itunes.apple.com/lookup";
        private const string APPLE_LOOKUP_PARAM    = "id";
        
        public static string GetLookupURL (string id) => $"{APPLE_LOOKUP_PROTOCOL}://{APPLE_LOOKUP_BASE_URL}?{APPLE_LOOKUP_PARAM}={id}";
    }

    [Serializable]
    public struct AppleAPIResult
    {
        public string[] appletvScreenshotUrls;
        public string[] screenshotUrls;
        public string[] ipadScreenshotUrls;
        public string artworkUrl60;
        public string artworkUrl512;
        public string artworkUrl100;
        public string[] features;
        public string[] supportedDevices;
        public string[] advisories;
        public bool isGameCenterEnabled;
        public string kind;
        public string minimumOsVersion;
        public string trackCensoredName;
        public string[] languageCodesISO2A;
        public string fileSizeBytes;
        public string sellerUrl;
        public string formattedPrice;
        public string contentAdvisoryRating;
        public string averageUserRatingForCurrentVersion;
        public string userRatingCountForCurrentVersion;
        public string averageUserRating;
        public string trackViewUrl;
        public string trackContentRating;
        public string currency;
        public string bundleId;
        public int trackId;
        public string trackName;
        public string releaseDate;
        public string sellerName;
        public string primaryGenreName;
        public string[] genreIds;
        public string isVppDeviceBasedLicensingEnabled;
        public string currentVersionReleaseDate;
        public string releaseNotes;
        public string primaryGenreId;
        public string description;
        public int artistId;
        public string artistName;
        public string[] genres;
        public float price;
        public string version;
        public string wrapperType;
        public int userRatingCount;
    }

    [Serializable]
    public class AppleAPIResponse
    {
        public int resultCount = 0;

        public AppleAPIResult[] results;

        public static AppleAPIResponse FromJson(in string jsonText)
        {
            return JsonConvert.DeserializeObject<AppleAPIResponse>(jsonText);
        }
    }

    internal class HttpParseException : Exception
    {
        public HttpParseException() { }
        public HttpParseException(string message) : base(message) { }
        public HttpParseException(string message, Exception inner) : base(message, inner) { }
    }

    internal static class HTTPUtils
    {
        internal static Task<HttpResponseMessage> Get(string url)
        {
            //Just return a new get request using httpclient
            return new HttpClient().GetAsync(url);
        }

        internal static string GetResponseContent(in HttpResponseMessage response)
        {
            //Content length is 0? Something is wrong
            if (response.Content.Headers.ContentLength == 0)
                throw new HttpParseException("Content length received was zero, therefore, empty.");

            //Otherwise, get contents
            return response.Content.ReadAsStringAsync().Result;
        }

        internal static (bool valid, string reason) IsResponseValid(in HttpResponseMessage response)
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

            //Timeout?
            if(response.StatusCode == HttpStatusCode.RequestTimeout)
                return (false, "Request timed out. Is the API up?");


            //Otherwise, at this point, we've filtered out the main  categories of 
            //response. Lets check if its a generic error:
            if (response.StatusCode != HttpStatusCode.OK)
                return (false, $"Got HTTP the status '{response.ReasonPhrase}'");

            //..

            //At this point, the status code has to be 200 OK. Lets check some other things.
            //.

            //Get content type
            var contentType = response.Content.Headers.ContentType.MediaType;

            //Is the content type not text/json or application/json? Or weirdly, text/javascript? If so.. die
            if (!(contentType.EndsWith("/json") || contentType.StartsWith("text/javascript")))
                return (false, $"Unexpected content type '{contentType}', I expected some kind of json.");

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

        public AppleAPIResponse Lookup()
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
            HttpResponseMessage response = HTTPUtils.Get(lookupURL).Result;

            //Get validity of response
            var responseValidityCheck = HTTPUtils.IsResponseValid(in response);

            //It's not valid? uh-oh, get out of here, throw an exception
            if (!responseValidityCheck.valid)
                throw new HttpParseException(responseValidityCheck.reason);

            //Otherwise, we're all good.
            var content = HTTPUtils.GetResponseContent(in response);

            //Return
            return AppleAPIResponse.FromJson(in content);
        }
    }
}
