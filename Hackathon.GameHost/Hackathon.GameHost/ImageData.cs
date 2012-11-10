using System.Runtime.Serialization;

namespace Hackathon.GameHost
{
    [DataContract]
    public class ImageData
    {
        [DataMember]
        public string Url { get; set; }
    }
}
