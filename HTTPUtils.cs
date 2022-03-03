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