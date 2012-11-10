using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Hackathon.GameHost.Lmgtfy
{
    [ServiceContract(ConfigurationName = "ILmgtfyService")]
    public interface ILmgtfyService
    {
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "recent.json")]
        LmgtfyResponse Load();
    }
}
