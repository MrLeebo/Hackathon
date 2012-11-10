using System.Runtime.Serialization;

namespace Hackathon.GameHost
{
    [DataContract]
    public class TumblrResponse
    {
        [DataMember(Name = "response")]
        public Blog[] Response { get; set; }

        [DataMember(Name = "meta")]
        public Meta Meta { get; set; }
    }

    [DataContract]
    public class Meta
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "msg")]
        public string Message { get; set; }
    }

    [DataContract]
    public class Blog
    {
        [DataMember(Name = "blog_name")]
        public string Name { get; set; }

        [DataMember(Name = "photos")]
        public Photo[] Photos { get; set; }
    }

    [DataContract]
    public class Photo
    {
        [DataMember(Name = "alt_sizes")]
        public AltSize[] AltSizes { get; set; }
    }

    [DataContract]
    public class AltSize
    {
        [DataMember(Name = "width")]
        public string Width { get; set; }

        [DataMember(Name = "height")]
        public string Height { get; set; }

        [DataMember(Name = "url")]
        public string URL { get; set; }
    }
}
