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

            LmgtfyResponse response;
            try
            {
                var client = new LmgtfyClient();
                response = client.Load();
            }
            catch (Exception ex)
            {
                // Congratulations, you broke Google. Here's a picture of Obama.
                Console.WriteLine(ex.ToString());
                return new ImageData { Url = "//upload.wikimedia.org/wikipedia/commons/thumb/e/e9/Official_portrait_of_Barack_Obama.jpg/73px-Official_portrait_of_Barack_Obama.jpg", Term = "Barack Obama" };
            }

            while (true)
            {
                var index = random.Next(response.Response.Length);
                LmgtfyObject picked = response.Response[index];

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
