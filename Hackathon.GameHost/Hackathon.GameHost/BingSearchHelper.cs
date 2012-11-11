using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Data.Services.Client;

namespace Hackathon.GameHost
{
   /// <summary>
/// Search Bing Web Index
/// </summary>
///<see cref="https://datamarket.azure.com/dataset/5ba839f1-12ce-4cce-bf57-a49d98d29a44"/>
public class BingSearchHelper
{
    private string AccountKey;
    public BingSearchHelper(string accountKey)
    {
        this.AccountKey = accountKey;
    }
 
    /// <param name="Sources">Bing search sources Sample Values : web+image+video+news+spell</param>
    /// <param name="Query">Bing search query Sample Values : xbox</param>
    /// <param name="Market">Market. Note: Not all Sources support all markets. Sample Values : en-US</param>
    /// <param name="Adult">Adult setting is used for filtering sexually explicit content Sample Values : Moderate</param>
    /// <param name="Latitude">Latitude Sample Values : 47.603450</param>
    /// <param name="Longitude">Longitude Sample Values : -122.329696</param>
    /// <param name="WebFileType">File extensions to return Sample Values : XLS</param>
    /// <param name="ImageFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof Sample Values : Size:Small</param>
    /// <param name="VideoFilters">Array of strings that filter the response the API sends based on size, aspect, color, style, face or any combination thereof Sample Values : Duration:Short</param>
    /// <param name="VideoSortBy">The sort order of results returned Sample Values : Date</param>
    /// <param name="NewsLocationOverride">Overrides Bing location detection. This parameter is only applicable in en-US market. Sample Values : US.WA</param>
    /// <param name="NewsCategory">The category of news for which to provide results Sample Values : rt_Business</param>
    /// <param name="NewsSortBy">The sort order of results returned Sample Values : Date</param>
    //public async Task<Result> Search(String Query, String Sources = "web", Double? Latitude = null, Double? Longitude = null, String Market = "", String Adult = "Strict", String WebFileType = "", String ImageFilters = "", String VideoFilters = "", String VideoSortBy = "", String NewsLocationOverride = "", String NewsCategory = "", String NewsSortBy = "")
    //{
    //    //var _Template = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Composite?Query='{0}'&Sources='{1}'&Latitude='{2}'&Longitude='{3}'&Market='{4}&Adult='{5}'&WebFileType='{6}'&ImageFilters='{7}'&VideoFilters='{8}'&VideoSortBy='{9}'&NewsLocationOverride='{10}'&NewsCategory='{11}'&NewsSortBy='{12}'$format=JSON&$top={13}";
    //    //var _Url = string.Format(_Template, Query, Sources, Latitude, Longitude, Market, Adult, WebFileType, ImageFilters, VideoFilters, VideoSortBy, NewsLocationOverride, NewsCategory, NewsSortBy, maxResults));
    //    var _Url = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/Composite?$format=JSON&";
    //    _Url += (string.IsNullOrWhiteSpace(Sources)) ? string.Empty : string.Format("Sources='{0}'&", System.Uri.EscapeDataString(Sources));
    //    _Url += (string.IsNullOrWhiteSpace(Query)) ? string.Empty : string.Format("Query='{0}'&", System.Uri.EscapeDataString(Query));
    //    _Url += (string.IsNullOrWhiteSpace(Market)) ? string.Empty : string.Format("Market='{0}'&", System.Uri.EscapeDataString(Market));
    //    _Url += (string.IsNullOrWhiteSpace(Adult)) ? string.Empty : string.Format("Adult='{0}'&", System.Uri.EscapeDataString(Adult));
    //    _Url += (!Latitude.HasValue) ? string.Empty : string.Format("Latitude='{0}'&", System.Uri.EscapeDataString(Latitude.ToString()));
    //    _Url += (!Longitude.HasValue) ? string.Empty : string.Format("Longitude='{0}'&", System.Uri.EscapeDataString(Longitude.ToString()));
    //    _Url += (string.IsNullOrWhiteSpace(WebFileType)) ? string.Empty : string.Format("WebFileType='{0}'&", System.Uri.EscapeDataString(WebFileType));
    //    _Url += (string.IsNullOrWhiteSpace(ImageFilters)) ? string.Empty : string.Format("ImageFilters='{0}'&", System.Uri.EscapeDataString(ImageFilters));
    //    _Url += (string.IsNullOrWhiteSpace(VideoFilters)) ? string.Empty : string.Format("VideoFilters='{0}'&", System.Uri.EscapeDataString(VideoFilters));
    //    _Url += (string.IsNullOrWhiteSpace(VideoSortBy)) ? string.Empty : string.Format("VideoSortBy='{0}'&", System.Uri.EscapeDataString(VideoSortBy));
    //    _Url += (string.IsNullOrWhiteSpace(NewsLocationOverride)) ? string.Empty : string.Format("NewsLocationOverride='{0}'&", System.Uri.EscapeDataString(NewsLocationOverride));
    //    _Url += (string.IsNullOrWhiteSpace(NewsCategory)) ? string.Empty : string.Format("NewsCategory='{0}'&", System.Uri.EscapeDataString(NewsCategory));
    //    _Url += (string.IsNullOrWhiteSpace(NewsSortBy)) ? string.Empty : string.Format("NewsSortBy='{0}'&", System.Uri.EscapeDataString(NewsSortBy));
    //    _Url = _Url.TrimEnd('&');
 
    //    string _JsonString = string.Empty;
    //    try
    //    {
    //        // fetch from rest service
    //        var _HttpHandler = new System.Net.Http.HttpClientHandler
    //        {
    //            Credentials = new System.Net.NetworkCredential(AccountKey, AccountKey)
    //        };
    //        var _HttpClient = new System.Net.Http.HttpClient(_HttpHandler);
    //        var _HttpResponse = await _HttpClient.GetAsync(_Url);
    //        _JsonString = await _HttpResponse.Content.ReadAsStringAsync();
 
    //        // check for error
    //        if (_JsonString.Contains("401 - Unauthorized"))
    //            throw new InvalidCredentialsException { JSON = _JsonString };
    //        if (_JsonString.Contains("Parameter:"))
    //            throw new InvalidParameterException { JSON = _JsonString };
 
    //        // deserialize json to objects
    //        var _JsonBytes = Encoding.Unicode.GetBytes(_JsonString);
    //        using (MemoryStream _MemoryStream = new MemoryStream(_JsonBytes))
    //        {
    //            var _JsonSerializer = new DataContractJsonSerializer(typeof(RootObject));
    //            var _Result = (RootObject)_JsonSerializer.ReadObject(_MemoryStream);
    //            return _Result.d.results[0];
    //        }
    //    }
    //    catch (InvalidSomethingException) { throw; }
    //    catch (Exception e) { throw new InvalidSomethingException(e) { JSON = _JsonString }; }
    //}
 
    public class InvalidSomethingException : Exception
    {
        public InvalidSomethingException() { }
        public InvalidSomethingException(Exception e) : base(string.Empty, e) { }
        public string JSON { get; set; }
    }
    public class InvalidCredentialsException : InvalidSomethingException { }
    public class InvalidParameterException : InvalidSomethingException { }
 
    public enum SearchTypes
    {
        /// <summary>Web search results</summary>
        web,
        /// <summary>Image search results</summary>
        image,
        /// <summary>News search results</summary>
        video,
        /// <summary>Video search results</summary>
        news,
        /// <summary>Related search suggestions based on the query entered</summary>
        related,
        /// <summary>Spelling suggestions based on the query entered</summary>
        spell
    }
 
    public class Metadata
    {
        public string uri { get; set; }
        public string type { get; set; }
    }
 
    public class Metadata2
    {
        public string uri { get; set; }
        public string type { get; set; }
    }
 
    public class Web
    {
        public Metadata2 __metadata { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DisplayUrl { get; set; }
        public string Url { get; set; }
    }
 
    public class Metadata3
    {
        public string uri { get; set; }
        public string type { get; set; }
    }
 
    public class Metadata4
    {
        public string type { get; set; }
    }
 
    public class Thumbnail
    {
        public Metadata4 __metadata { get; set; }
        public string MediaUrl { get; set; }
        public string ContentType { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
    }
 
    public class Image
    {
        public Metadata3 __metadata { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string SourceUrl { get; set; }
        public string DisplayUrl { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
        public string ContentType { get; set; }
        public Thumbnail Thumbnail { get; set; }
    }
 
    public class Metadata5
    {
        public string uri { get; set; }
        public string type { get; set; }
    }
 
    public class Metadata6
    {
        public string type { get; set; }
    }
 
    public class Thumbnail2
    {
        public Metadata6 __metadata { get; set; }
        public string MediaUrl { get; set; }
        public string ContentType { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
    }
 
    public class Video
    {
        public Metadata5 __metadata { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public string DisplayUrl { get; set; }
        public string RunTime { get; set; }
        public Thumbnail2 Thumbnail { get; set; }
    }
 
    public class Metadata7
    {
        public string uri { get; set; }
        public string type { get; set; }
    }
 
    public class News
    {
        public Metadata7 __metadata { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }
 
    public class Result
    {
        public Metadata __metadata { get; set; }
        public string ID { get; set; }
        public List<Web> Web { get; set; }
        public List<Image> Image { get; set; }
        public List<Video> Video { get; set; }
        public List<News> News { get; set; }
        public List<object> RelatedSearch { get; set; }
        public List<object> SpellingSuggestions { get; set; }
    }
 
    public class D
    {
        public List<Result> results { get; set; }
    }
 
    public class RootObject
    {
        public D d { get; set; }
    }
}

}
