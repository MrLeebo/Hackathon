namespace Hackathon.GameHost.Domain
{
    public class GameImage
    {
        public string URL { get; set; }
        public string Name { get; set; }

        public static GameImage FromImageData(ImageData imageData)
        {
            return new GameImage
                       {
                           Name = "Google",
                           URL = imageData.Url
                       };
        }
    }
}
