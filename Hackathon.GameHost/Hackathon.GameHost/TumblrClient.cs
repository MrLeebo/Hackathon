using System.ServiceModel;

namespace Hackathon.GameHost
{
    public class TumblrClient : ClientBase<ITumblrService>
    {
        public TumblrClient()
            : base("endpoint") {}

        public ImageData Tagged(string tag)
        {
            return this.Channel.Tagged(tag);
        }
    }
}
