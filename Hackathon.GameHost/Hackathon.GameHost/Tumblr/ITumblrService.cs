using System.ServiceModel;
using System.ServiceModel.Web;

namespace Hackathon.GameHost
{
    [ServiceContract(ConfigurationName = "ITumblrService")]
    public interface ITumblrService
    {
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare, 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "tagged?tag={tag}&api_key=NTgqTNie0kyo5ndDn9C3A8uOXMfxmOvhzc6Yli6SV9tML7APXK&limit=1")]
        TumblrResponse Tagged(string tag);
    }
}
