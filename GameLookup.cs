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
    public class GameLookup
    {
        /// <summary>
        /// The ID to look up, for this game
        /// </summary>
        private string lookupID;

        /// <summary>
        /// Constructs a new game with a given lookup id.
        /// </summary>
        /// <param name="lookupID">The lookup id.</param>
        public GameLookup(string lookupID) 
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
