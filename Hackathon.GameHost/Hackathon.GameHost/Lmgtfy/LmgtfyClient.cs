using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace Hackathon.GameHost.Lmgtfy
{
    public class LmgtfyClient
    {
        public LmgtfyResponse Load()
        {
            string result;
            var webRequest = WebRequest.Create("http://live.lmgtfy.com/recent.json");
            using (var response = webRequest.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                result = reader.ReadToEnd();

            var serializer = new JavaScriptSerializer();
            var data = serializer.Deserialize<LmgtfyObject[]>(result);

            return new LmgtfyResponse { Response = data };
        }
    }
}
