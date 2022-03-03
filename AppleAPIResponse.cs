using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

[Serializable]
public struct AppleGameResult
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

    public AppleGameResult[] results;

    public static AppleAPIResponse FromJson(in string jsonText)
    {
        return JsonConvert.DeserializeObject<AppleAPIResponse>(jsonText);
    }
}