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
            var random = new Random();

            var client = new LmgtfyClient();
            LmgtfyResponse lmgtfyResponse = client.Load();

            while (true)
            {
                var index = random.Next(lmgtfyResponse.Response.Length);
                LmgtfyObject picked = lmgtfyResponse.Response[index];

                var bingService = new BingService();
                var results = bingService.ExecuteQuery(picked.q);

                if (results.Count == 0)
                    continue;

                index = random.Next(results.Count);
                var result = results[index];

                return new ImageData {Url = result.MediaUrl, Term = result.Title};
            }
        }
    }
}
