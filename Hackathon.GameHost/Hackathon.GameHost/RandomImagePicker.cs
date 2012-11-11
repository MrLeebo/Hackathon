using System;
using Hackathon.GameHost.Domain;
using Hackathon.GameHost.Tumblr;

namespace Hackathon.GameHost
{
    public class RandomImagePicker : IImagePicker
    {
        public ImageData Pick()
        {
            var bingService = new BingService();
            var results = bingService.ExecuteQuery("corn");

            var random = new Random();
            var index = random.Next(results.Count);

            var result = results[index];
            return new ImageData {Url = result.MediaUrl, Term = result.Title};
        }
    }
}
