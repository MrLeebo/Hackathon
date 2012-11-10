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
            UriTemplate = "tagged?tag={tag}")]
        ImageData Tagged(string tag);
    }
}
