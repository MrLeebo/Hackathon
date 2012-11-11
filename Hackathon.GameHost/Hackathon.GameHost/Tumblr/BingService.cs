using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Hackathon.GameHost.Tumblr
{
    public class BingService
    {
        private static string accountKey_jug = "Xc7Qal1cSxvUfFpiPc1JmVDxdwCPymWIa0gUlervTHc=";
        private static string accountKey_ionut = "RT8E83E8IMsC0kwcB0vBIJNjNqEqNsNAQlEyxemvAS4=";

        public IList<ImageWebResult> ExecuteQuery(string query)
        {
            var webResults = new List<ImageWebResult>();
            try
            {
                //URL Base:
                Uri urlBase = new Uri("https://api.datamarket.azure.com");
                
                //Make the system recognizes your account key:
                NetworkCredential credentials = new NetworkCredential(accountKey_jug, accountKey_jug);
                System.Data.Services.Client.DataServiceContext dsc = new System.Data.Services.Client.DataServiceContext(urlBase);
                dsc.Credentials = new NetworkCredential(accountKey_jug, accountKey_jug);
                
                //Executes the query:
                string searchUri = this.CreateUrl(query);
                Uri urlSearch = new Uri(searchUri);
                dsc.IgnoreMissingProperties = true;
                webResults = dsc.Execute<ImageWebResult>(urlSearch).ToList<ImageWebResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return webResults;
        }

        private string CreateUrl(
            String Query, 
            Double? Latitude = null, 
            Double? Longitude = null, 
            String Market = "", 
            String Adult = "Off", 
            String WebFileType = "", 
            String ImageFilters = "Size:Medium", 
            String VideoFilters = "", 
            String VideoSortBy = "", 
            String NewsLocationOverride = "", 
            String NewsCategory = "", 
            String NewsSortBy = "")
        {
            string _Url = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Image?$format=atom&";
            
            _Url += (string.IsNullOrWhiteSpace(Query)) ? string.Empty : string.Format("Query='{0}'&", System.Uri.EscapeDataString(Query));
            _Url += (string.IsNullOrWhiteSpace(Market)) ? string.Empty : string.Format("Market='{0}'&", System.Uri.EscapeDataString(Market));
            _Url += (string.IsNullOrWhiteSpace(Adult)) ? string.Empty : string.Format("Adult='{0}'&", System.Uri.EscapeDataString(Adult));
            _Url += (!Latitude.HasValue) ? string.Empty : string.Format("Latitude='{0}'&", System.Uri.EscapeDataString(Latitude.ToString()));
            _Url += (!Longitude.HasValue) ? string.Empty : string.Format("Longitude='{0}'&", System.Uri.EscapeDataString(Longitude.ToString()));
            _Url += (string.IsNullOrWhiteSpace(WebFileType)) ? string.Empty : string.Format("WebFileType='{0}'&", System.Uri.EscapeDataString(WebFileType));
            _Url += (string.IsNullOrWhiteSpace(ImageFilters)) ? string.Empty : string.Format("ImageFilters='{0}'&", System.Uri.EscapeDataString(ImageFilters));
            _Url += (string.IsNullOrWhiteSpace(VideoFilters)) ? string.Empty : string.Format("VideoFilters='{0}'&", System.Uri.EscapeDataString(VideoFilters));
            _Url += (string.IsNullOrWhiteSpace(VideoSortBy)) ? string.Empty : string.Format("VideoSortBy='{0}'&", System.Uri.EscapeDataString(VideoSortBy));
            _Url += (string.IsNullOrWhiteSpace(NewsLocationOverride)) ? string.Empty : string.Format("NewsLocationOverride='{0}'&", System.Uri.EscapeDataString(NewsLocationOverride));
            _Url += (string.IsNullOrWhiteSpace(NewsCategory)) ? string.Empty : string.Format("NewsCategory='{0}'&", System.Uri.EscapeDataString(NewsCategory));
            _Url += (string.IsNullOrWhiteSpace(NewsSortBy)) ? string.Empty : string.Format("NewsSortBy='{0}'&", System.Uri.EscapeDataString(NewsSortBy));
            _Url = _Url.TrimEnd('&');

            return _Url;
        }
    }

    public class ImageWebResult
    {
        public Guid ID
        {
            get;
            set;
        }

        public String Title
        {
            get;
            set;
        }

        public String MediaUrl
        {
            get;
            set;
        }

        public String SourceUrl
        {
            get;
            set;
        }

        public string DisplayUrl { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string FileSize { get; set; }
        public string ContentType { get; set; }
    }

    public class WebResult
    {
        private Guid _ID;
        private String _Title;
        private String _Description;
        private String _DisplayUrl;

        private String _Url;
        public Guid ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public String Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        public String Description
        {
            get { return this._Description; }
            set { this._Description = value; }
        }

        public String DisplayUrl
        {
            get { return this._DisplayUrl; }
            set { this._DisplayUrl = value; }
        }

        public String Url
        {
            get { return this._Url; }
            set { this._Url = value; }
        }
    }
}
