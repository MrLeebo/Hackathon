using System.ServiceModel;

namespace Hackathon.GameHost
{
    public class TumblrClient : ClientBase<ITumblrService>
    {
        public TumblrClient()
            : base("tumblrEndpoint") { }

        public TumblrResponse Tagged(string tag)
        {
            return this.Channel.Tagged(tag);
        }
    }
}
