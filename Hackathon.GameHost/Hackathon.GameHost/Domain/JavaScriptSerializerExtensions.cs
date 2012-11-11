using System.Web.Script.Serialization;
using PusherClientDotNet;

namespace Hackathon.GameHost.Domain
{
    public static class JavaScriptSerializerExtensions
    {
        public static T ToObject<T>(this object data)
        {
            var json = Pusher.JSON.stringify(data);
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
