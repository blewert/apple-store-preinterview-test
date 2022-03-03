using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppleGameInfo
{
    /// <summary>
    /// A custom exception to be thrown during the parsing of the HTTP response
    /// </summary>
    internal class HttpParseException : Exception
    {
        public HttpParseException() { }
        public HttpParseException(string message) : base(message) { }
        public HttpParseException(string message, Exception inner) : base(message, inner) { }
    }


    /// <summary>
    /// Contains a variety of methods for HTTP functionalities
    /// </summary>
    internal static class HTTPUtils
    {
        /// <summary>
        /// Performs a GET request to a specified URL, and returns the async handler
        /// </summary>
        /// <param name="url">The URL</param>
        /// <returns>The async handler</returns>
        internal static Task<HttpResponseMessage> Get(string url)
        {
            //Just return a new get request using httpclient
            return new HttpClient().GetAsync(url);
        }

        /// <summary>
        /// Gets the text from a given response, if possible.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The text (probably json) from the response</returns>
        internal static string GetResponseContent(in HttpResponseMessage response)
        {
            //Content length is 0? Something is wrong
            if (response.Content.Headers.ContentLength == 0)
                throw new HttpParseException("Content length received was zero, therefore, empty.");

            //Otherwise, get contents
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Checks if a HttpResponseMessage is valid.
        /// </summary>
        /// <param name="response">The response from the request</param>
        /// <returns>True if valid, with a reason, false otherwise.</returns>
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
            if (response.StatusCode == HttpStatusCode.RequestTimeout)
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
}