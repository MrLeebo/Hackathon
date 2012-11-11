using System;
using Hackathon.GameHost.Domain;
using Hackathon.GameHost.Tumblr;
using Hackathon.GameHost.Lmgtfy;

namespace Hackathon.GameHost
{
    public class RandomImagePicker : IImagePicker
    {
        public ImageData Pick()
        {
            LmgtfyClient client = new LmgtfyClient();
            LmgtfyResponse lmgtfyResponse = client.Load();

            var random = new Random();
            var index = random.Next(lmgtfyResponse.Response.Length);
            LmgtfyObject picked = lmgtfyResponse.Response[index];

            var bingService = new BingService();
            var results = bingService.ExecuteQuery(picked.q);

            index = random.Next(results.Count);
            var result = results[index];
            return new ImageData {Url = result.MediaUrl, Term = result.Title};
        }
    }
}
